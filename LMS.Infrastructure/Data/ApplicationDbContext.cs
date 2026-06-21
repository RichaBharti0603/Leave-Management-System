using LMS.Domain.Entities;
using LMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace LMS.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<EmployeeProfile> EmployeeProfiles => Set<EmployeeProfile>();
    public DbSet<LeaveType> LeaveTypes => Set<LeaveType>();
    public DbSet<LeavePolicy> LeavePolicies => Set<LeavePolicy>();
    public DbSet<Holiday> Holidays => Set<Holiday>();
    public DbSet<LeaveBalance> LeaveBalances => Set<LeaveBalance>();
    public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();
    public DbSet<ApprovalHistory> ApprovalHistories => Set<ApprovalHistory>();
    public DbSet<Attachment> Attachments => Set<Attachment>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User - EmployeeProfile (1:1)
        modelBuilder.Entity<User>()
            .HasOne(u => u.EmployeeProfile)
            .WithOne(e => e.User)
            .HasForeignKey<EmployeeProfile>(e => e.UserId);

        // EmployeeProfile Manager Self-Referencing (1:N)
        modelBuilder.Entity<EmployeeProfile>()
            .HasOne(e => e.Manager)
            .WithMany()
            .HasForeignKey(e => e.ManagerEmployeeId);

        // LeaveBalance Concurrency Token (PostgreSQL xmin)
        modelBuilder.Entity<LeaveBalance>()
            .UseXminAsConcurrencyToken()
            .HasIndex(lb => new { lb.EmployeeId, lb.LeaveTypeId, lb.Year })
            .IsUnique();

        // LeaveRequest indexes
        modelBuilder.Entity<LeaveRequest>()
            .HasIndex(lr => new { lr.EmployeeId, lr.StartDateTime, lr.EndDateTime });

        // ApprovalHistory
        modelBuilder.Entity<ApprovalHistory>()
            .HasOne(ah => ah.Approver)
            .WithMany()
            .HasForeignKey(ah => ah.ApproverEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ApprovalHistory>()
            .HasOne(ah => ah.LeaveRequest)
            .WithMany(lr => lr.ApprovalHistories)
            .HasForeignKey(ah => ah.LeaveRequestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
