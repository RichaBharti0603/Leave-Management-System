namespace LMS.Domain.Entities;

public class LeavePolicy
{
    public int Id { get; set; }
    public int LeaveTypeId { get; set; }
    public LeaveType LeaveType { get; set; } = null!;
    
    public int MinNoticeHours { get; set; } = 0;
    public int MaxConsecutiveDays { get; set; } = 365;
    public bool CountWeekends { get; set; } = false;
    public bool CountHolidays { get; set; } = false;
    public int AttachmentRequiredIfDaysGte { get; set; } = int.MaxValue;
    public bool RequiresHrApproval { get; set; } = false;
    
    public string? BlackoutRulesJson { get; set; } // Minimal structure for blackout dates
}
