using LMS.Application.Interfaces;
using LMS.Domain.Entities;
using LMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Services;

public class ApprovalService : IApprovalService
{
    private readonly IApplicationDbContext _context;

    public ApprovalService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task ApproveAsync(int approverId, int requestId, string? comments)
    {
        var request = await _context.LeaveRequests
            .Include(r => r.LeaveType)
            .FirstOrDefaultAsync(r => r.Id == requestId);

        if (request == null || request.Status != LeaveRequestStatus.Pending)
            throw new Exception("Invalid request or state");

        var approverProfile = await _context.EmployeeProfiles.FirstOrDefaultAsync(e => e.UserId == approverId);
        if (approverProfile == null) throw new Exception("Approver not found");

        // Simple auth check: if manager, must be manager of the employee
        var employee = await _context.EmployeeProfiles.FirstOrDefaultAsync(e => e.UserId == request.EmployeeId);
        
        var approverUser = await _context.Users.FindAsync(approverId);
        if (approverUser?.Role == Role.Manager && employee?.ManagerEmployeeId != approverId)
        {
            throw new Exception("Unauthorized: You are not the manager of this employee.");
        }

        var policy = await _context.LeavePolicies.FirstOrDefaultAsync(p => p.LeaveTypeId == request.LeaveTypeId);
        
        // Determine next status
        var nextStatus = LeaveRequestStatus.Approved;
        int approvalLevel = 1;

        if (approverUser?.Role == Role.Manager && policy?.RequiresHrApproval == true)
        {
            // Just marking as HR approval needed, for simplicity we keep it Pending and add a history record
            // Or we could have a "PendingHR" status. For now, keep it Pending.
            nextStatus = LeaveRequestStatus.Pending;
        }

        // Only deduct balance if fully approved
        if (nextStatus == LeaveRequestStatus.Approved)
        {
            var balance = await _context.LeaveBalances
                .FirstOrDefaultAsync(b => b.EmployeeId == request.EmployeeId && b.LeaveTypeId == request.LeaveTypeId && b.Year == request.StartDateTime.Year);

            if (balance != null)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    balance.Pending -= request.Duration;
                    balance.Used += request.Duration;

                    request.Status = nextStatus;
                    request.UpdatedAt = DateTime.UtcNow;

                    var history = new ApprovalHistory
                    {
                        LeaveRequestId = request.Id,
                        ApproverEmployeeId = approverId,
                        Level = approvalLevel,
                        Action = ApprovalAction.Approved,
                        Comments = comments
                    };
                    _context.ApprovalHistories.Add(history);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return;
                }
                catch (DbUpdateConcurrencyException)
                {
                    await transaction.RollbackAsync();
                    throw new Exception("Concurrency error while updating balance.");
                }
            }
        }
        else
        {
            var history = new ApprovalHistory
            {
                LeaveRequestId = request.Id,
                ApproverEmployeeId = approverId,
                Level = approvalLevel,
                Action = ApprovalAction.Approved,
                Comments = comments
            };
            _context.ApprovalHistories.Add(history);
            await _context.SaveChangesAsync();
        }
    }

    public async Task RejectAsync(int approverId, int requestId, string? comments)
    {
        await ProcessRejectionOrSendBack(approverId, requestId, comments, LeaveRequestStatus.Rejected, ApprovalAction.Rejected);
    }

    public async Task SendBackAsync(int approverId, int requestId, string? comments)
    {
        await ProcessRejectionOrSendBack(approverId, requestId, comments, LeaveRequestStatus.SentBack, ApprovalAction.SentBack);
    }

    private async Task ProcessRejectionOrSendBack(int approverId, int requestId, string? comments, LeaveRequestStatus newStatus, ApprovalAction actionType)
    {
        var request = await _context.LeaveRequests.FirstOrDefaultAsync(r => r.Id == requestId);
        if (request == null || request.Status != LeaveRequestStatus.Pending) throw new Exception("Invalid request or state");

        var balance = await _context.LeaveBalances
            .FirstOrDefaultAsync(b => b.EmployeeId == request.EmployeeId && b.LeaveTypeId == request.LeaveTypeId && b.Year == request.StartDateTime.Year);

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            if (balance != null)
            {
                balance.Pending -= request.Duration;
            }

            request.Status = newStatus;
            request.UpdatedAt = DateTime.UtcNow;

            var history = new ApprovalHistory
            {
                LeaveRequestId = request.Id,
                ApproverEmployeeId = approverId,
                Level = 1,
                Action = actionType,
                Comments = comments
            };
            _context.ApprovalHistories.Add(history);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            await transaction.RollbackAsync();
            throw new Exception("Concurrency error.");
        }
    }
}
