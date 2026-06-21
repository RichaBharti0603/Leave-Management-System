using LMS.Domain.Enums;

namespace LMS.Domain.Entities;

public class AuditLog
{
    public int Id { get; set; }
    
    public int ActorUserId { get; set; }
    public User Actor { get; set; } = null!;
    
    public string EntityType { get; set; } = null!;
    public string EntityId { get; set; } = null!;
    public ActionType Action { get; set; }
    
    public string? BeforeJson { get; set; }
    public string? AfterJson { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
