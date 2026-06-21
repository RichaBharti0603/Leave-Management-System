namespace LMS.Domain.Entities;

public class LeaveType
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Unit { get; set; } = "Days"; // Days, Hours
    public bool Paid { get; set; } = true;
    public bool RequiresAttachment { get; set; } = false;
    public bool IsActive { get; set; } = true;
    
    public ICollection<LeavePolicy> Policies { get; set; } = new List<LeavePolicy>();
}
