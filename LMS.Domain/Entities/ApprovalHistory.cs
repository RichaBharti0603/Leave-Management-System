using LMS.Domain.Enums;

namespace LMS.Domain.Entities;

public class ApprovalHistory
{
    public int Id { get; set; }
    
    public int LeaveRequestId { get; set; }
    public LeaveRequest LeaveRequest { get; set; } = null!;
    
    public int Level { get; set; } // 1 for Manager, 2 for HR
    
    public int ApproverEmployeeId { get; set; }
    public EmployeeProfile Approver { get; set; } = null!;
    
    public ApprovalAction Action { get; set; }
    public string? Comments { get; set; }
    
    public DateTime ActionAt { get; set; } = DateTime.UtcNow;
}
