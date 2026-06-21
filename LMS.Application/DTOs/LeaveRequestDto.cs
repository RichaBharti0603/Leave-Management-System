namespace LMS.Application.DTOs;

public class CreateLeaveRequestDto
{
    public int LeaveTypeId { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public string? Reason { get; set; }
}

public class LeaveRequestResponseDto
{
    public int Id { get; set; }
    public int LeaveTypeId { get; set; }
    public string LeaveTypeName { get; set; } = null!;
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public decimal Duration { get; set; }
    public string Status { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
