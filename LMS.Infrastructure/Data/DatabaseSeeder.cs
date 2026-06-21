using LMS.Domain.Entities;
using LMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LMS.Infrastructure.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context, ILogger logger)
    {
        try
        {
            if (context.Database.IsNpgsql())
            {
                await context.Database.MigrateAsync();
            }

            if (!await context.Users.AnyAsync())
            {
                logger.LogInformation("Seeding Users...");
                
                var admin = new User { Email = "admin@lms.com", PasswordHash = "hash", Role = Role.Admin };
                var hr = new User { Email = "hr@lms.com", PasswordHash = "hash", Role = Role.HR };
                var manager = new User { Email = "manager@lms.com", PasswordHash = "hash", Role = Role.Manager };
                var employee = new User { Email = "employee@lms.com", PasswordHash = "hash", Role = Role.Employee };

                context.Users.AddRange(admin, hr, manager, employee);
                await context.SaveChangesAsync();

                logger.LogInformation("Seeding Employee Profiles...");
                
                var managerProfile = new EmployeeProfile { UserId = manager.Id, EmployeeCode = "M001", FirstName = "Manager", LastName = "One", JoinDate = DateTime.UtcNow.AddYears(-2) };
                context.EmployeeProfiles.Add(managerProfile);
                await context.SaveChangesAsync();

                var employeeProfile = new EmployeeProfile { UserId = employee.Id, EmployeeCode = "E001", FirstName = "Employee", LastName = "One", JoinDate = DateTime.UtcNow.AddYears(-1), ManagerEmployeeId = managerProfile.UserId };
                context.EmployeeProfiles.Add(employeeProfile);
                await context.SaveChangesAsync();
            }

            if (!await context.LeaveTypes.AnyAsync())
            {
                logger.LogInformation("Seeding Leave Types & Policies...");
                
                var sick = new LeaveType { Name = "Sick Leave", RequiresAttachment = true };
                var casual = new LeaveType { Name = "Casual Leave" };
                var earned = new LeaveType { Name = "Earned Leave" };
                var unpaid = new LeaveType { Name = "Unpaid Leave", Paid = false };

                context.LeaveTypes.AddRange(sick, casual, earned, unpaid);
                await context.SaveChangesAsync();

                var sickPolicy = new LeavePolicy { LeaveTypeId = sick.Id, AttachmentRequiredIfDaysGte = 2 };
                var casualPolicy = new LeavePolicy { LeaveTypeId = casual.Id, MaxConsecutiveDays = 3, MinNoticeHours = 24 };
                var earnedPolicy = new LeavePolicy { LeaveTypeId = earned.Id, RequiresHrApproval = true, MinNoticeHours = 168 }; // 7 days

                context.LeavePolicies.AddRange(sickPolicy, casualPolicy, earnedPolicy);
                await context.SaveChangesAsync();
                
                // Seed initial balance for the employee
                var employeeProfile = await context.EmployeeProfiles.FirstOrDefaultAsync(e => e.EmployeeCode == "E001");
                if (employeeProfile != null)
                {
                    var balance = new LeaveBalance
                    {
                        EmployeeId = employeeProfile.UserId,
                        LeaveTypeId = sick.Id,
                        Year = DateTime.UtcNow.Year,
                        Opening = 0,
                        Accrued = 10,
                        Used = 0,
                        Pending = 0,
                        Closing = 10
                    };
                    context.LeaveBalances.Add(balance);
                    await context.SaveChangesAsync();
                }
            }
            
            logger.LogInformation("Seeding completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }
}
