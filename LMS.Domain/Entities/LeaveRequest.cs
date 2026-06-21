using LMS.Domain.Enums;

namespace LMS.Domain.Entities;

public class LeaveRequest
{
    public int Id { get; set; }
    
    public int EmployeeId { get; set; }
    public EmployeeProfile Employee { get; set; } = null!;
    
    public int LeaveTypeId { get; set; }
    public LeaveType LeaveType { get; set; } = null!;
    
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public decimal Duration { get; set; } // e.g., 1.5 days
    
    public string? Reason { get; set; }
    
    public LeaveRequestStatus Status { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    public ICollection<ApprovalHistory> ApprovalHistories { get; set; } = new List<ApprovalHistory>();
    public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
}
