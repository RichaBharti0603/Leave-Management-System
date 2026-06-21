using LMS.Domain.Entities;
using LMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text.Json;

namespace LMS.Infrastructure.Data;

public class AuditInterceptor : SaveChangesInterceptor
{
    // In a real application, you'd inject an ICurrentUserService to get the ActorUserId.
    // For now, we will assume a generic system user (Id = 1) if we can't resolve it,
    // or just leave it for the service layer to explicitly set.
    // To keep it simple and robust per requirements, we'll intercept and log.
    
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        var context = eventData.Context;
        if (context == null) return base.SavingChanges(eventData, result);

        var auditEntries = new List<AuditLog>();

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is AuditLog || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                continue;

            // Only tracking specific entities for brevity, but could track all
            if (entry.Entity is LeaveRequest || entry.Entity is LeaveBalance || entry.Entity is LeavePolicy)
            {
                var auditLog = new AuditLog
                {
                    ActorUserId = 1, // Hardcoded for interceptor; in production, use HttpContextAccessor -> UserId
                    EntityType = entry.Entity.GetType().Name,
                    Action = entry.State switch
                    {
                        EntityState.Added => ActionType.Create,
                        EntityState.Modified => ActionType.Update,
                        EntityState.Deleted => ActionType.Delete,
                        _ => ActionType.Create // Default fallback
                    },
                    CreatedAt = DateTime.UtcNow
                };

                // For primary key, we might not have it if it's Added (unless using GUIDs).
                // If it's Modified/Deleted, we have it.
                var primaryKey = entry.Properties.FirstOrDefault(p => p.Metadata.IsPrimaryKey());
                auditLog.EntityId = primaryKey?.CurrentValue?.ToString() ?? "0";

                if (entry.State == EntityState.Modified)
                {
                    var before = new Dictionary<string, object?>();
                    var after = new Dictionary<string, object?>();
                    
                    foreach (var prop in entry.Properties)
                    {
                        if (prop.IsModified)
                        {
                            before[prop.Metadata.Name] = prop.OriginalValue;
                            after[prop.Metadata.Name] = prop.CurrentValue;
                        }
                    }
                    auditLog.BeforeJson = JsonSerializer.Serialize(before);
                    auditLog.AfterJson = JsonSerializer.Serialize(after);
                }
                else if (entry.State == EntityState.Added)
                {
                    var after = entry.Properties.ToDictionary(p => p.Metadata.Name, p => p.CurrentValue);
                    auditLog.AfterJson = JsonSerializer.Serialize(after);
                }
                
                auditEntries.Add(auditLog);
            }
        }

        if (auditEntries.Any())
        {
            context.Set<AuditLog>().AddRange(auditEntries);
        }

        return base.SavingChanges(eventData, result);
    }
    
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        SavingChanges(eventData, result);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
