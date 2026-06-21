using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Domain.Entities;
using LMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Services;

public class LeaveRequestService : ILeaveRequestService
{
    private readonly IApplicationDbContext _context;

    public LeaveRequestService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<LeaveRequestResponseDto> CreateRequestAsync(int employeeId, CreateLeaveRequestDto dto)
    {
        // Calculate duration (simple version: excluding weekends if policy says so)
        var policy = await _context.LeavePolicies.FirstOrDefaultAsync(p => p.LeaveTypeId == dto.LeaveTypeId);
        if (policy == null) throw new Exception("Leave policy not found");

        decimal duration = CalculateDuration(dto.StartDateTime, dto.EndDateTime, policy);

        // Validation: Overlap
        var hasOverlap = await _context.LeaveRequests
            .AnyAsync(r => r.EmployeeId == employeeId && 
                           r.Status != LeaveRequestStatus.Cancelled && 
                           r.Status != LeaveRequestStatus.Rejected &&
                           r.StartDateTime < dto.EndDateTime && 
                           dto.StartDateTime < r.EndDateTime);
                           
        if (hasOverlap) throw new Exception("Overlapping leave request exists.");

        // Validation: Notice Period
        var noticeHours = (dto.StartDateTime - DateTime.UtcNow).TotalHours;
        if (noticeHours < policy.MinNoticeHours) throw new Exception($"Minimum notice period of {policy.MinNoticeHours} hours not met.");

        // Validation: Balance
        var currentYear = dto.StartDateTime.Year;
        var balance = await _context.LeaveBalances
            .FirstOrDefaultAsync(b => b.EmployeeId == employeeId && b.LeaveTypeId == dto.LeaveTypeId && b.Year == currentYear);

        if (balance == null) throw new Exception("No leave balance found for this year.");

        var available = balance.Opening + balance.Accrued - balance.Used - balance.Pending;
        if (duration > available) throw new Exception($"Insufficient leave balance. Available: {available}, Requested: {duration}");

        // Create Request
        var request = new LeaveRequest
        {
            EmployeeId = employeeId,
            LeaveTypeId = dto.LeaveTypeId,
            StartDateTime = dto.StartDateTime,
            EndDateTime = dto.EndDateTime,
            Duration = duration,
            Reason = dto.Reason,
            Status = LeaveRequestStatus.Draft
        };

        _context.LeaveRequests.Add(request);
        await _context.SaveChangesAsync();

        return new LeaveRequestResponseDto
        {
            Id = request.Id,
            LeaveTypeId = request.LeaveTypeId,
            StartDateTime = request.StartDateTime,
            EndDateTime = request.EndDateTime,
            Duration = request.Duration,
            Status = request.Status.ToString(),
            CreatedAt = request.CreatedAt
        };
    }

    public async Task SubmitRequestAsync(int employeeId, int requestId)
    {
        var request = await _context.LeaveRequests.FirstOrDefaultAsync(r => r.Id == requestId && r.EmployeeId == employeeId);
        if (request == null) throw new Exception("Request not found");
        
        if (request.Status != LeaveRequestStatus.Draft && request.Status != LeaveRequestStatus.SentBack)
            throw new Exception("Only Draft or SentBack requests can be submitted.");

        var currentYear = request.StartDateTime.Year;
        var balance = await _context.LeaveBalances
            .FirstOrDefaultAsync(b => b.EmployeeId == employeeId && b.LeaveTypeId == request.LeaveTypeId && b.Year == currentYear);

        if (balance == null) throw new Exception("Balance not found");

        // Transaction for safety
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Reserve balance
            balance.Pending += request.Duration;
            
            request.Status = LeaveRequestStatus.Pending;
            request.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            await transaction.RollbackAsync();
            throw new Exception("A concurrency error occurred while updating balance. Please try again.");
        }
    }

    public async Task CancelRequestAsync(int employeeId, int requestId)
    {
        var request = await _context.LeaveRequests.FirstOrDefaultAsync(r => r.Id == requestId && r.EmployeeId == employeeId);
        if (request == null) throw new Exception("Request not found");

        if (request.Status == LeaveRequestStatus.Approved || request.Status == LeaveRequestStatus.Pending)
        {
            var currentYear = request.StartDateTime.Year;
            var balance = await _context.LeaveBalances
                .FirstOrDefaultAsync(b => b.EmployeeId == employeeId && b.LeaveTypeId == request.LeaveTypeId && b.Year == currentYear);

            if (balance != null)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    if (request.Status == LeaveRequestStatus.Pending)
                        balance.Pending -= request.Duration;
                    else if (request.Status == LeaveRequestStatus.Approved)
                        balance.Used -= request.Duration;

                    request.Status = LeaveRequestStatus.Cancelled;
                    request.UpdatedAt = DateTime.UtcNow;

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return;
                }
                catch (DbUpdateConcurrencyException)
                {
                    await transaction.RollbackAsync();
                    throw new Exception("Concurrency error.");
                }
            }
        }
        
        request.Status = LeaveRequestStatus.Cancelled;
        request.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<LeaveRequestResponseDto>> GetMyRequestsAsync(int employeeId)
    {
        return await _context.LeaveRequests
            .Where(r => r.EmployeeId == employeeId)
            .Select(r => new LeaveRequestResponseDto
            {
                Id = r.Id,
                LeaveTypeId = r.LeaveTypeId,
                StartDateTime = r.StartDateTime,
                EndDateTime = r.EndDateTime,
                Duration = r.Duration,
                Status = r.Status.ToString(),
                CreatedAt = r.CreatedAt
            }).ToListAsync();
    }

    private decimal CalculateDuration(DateTime start, DateTime end, LeavePolicy policy)
    {
        // Simplified calculation. Real app would check holidays table and weekends precisely
        int days = (end.Date - start.Date).Days + 1;
        if (!policy.CountWeekends)
        {
            int weekends = 0;
            for (DateTime d = start.Date; d <= end.Date; d = d.AddDays(1))
            {
                if (d.DayOfWeek == DayOfWeek.Saturday || d.DayOfWeek == DayOfWeek.Sunday)
                    weekends++;
            }
            days -= weekends;
        }
        return Math.Max(days, 0); // Assuming full days for simplicity, but duration is decimal
    }
}
