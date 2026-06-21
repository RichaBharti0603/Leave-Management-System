namespace LMS.Domain.Entities;

public class LeaveBalance
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public EmployeeProfile Employee { get; set; } = null!;
    
    public int LeaveTypeId { get; set; }
    public LeaveType LeaveType { get; set; } = null!;
    
    public int Year { get; set; }
    public decimal Opening { get; set; }
    public decimal Accrued { get; set; }
    public decimal Used { get; set; }
    public decimal Pending { get; set; }
    public decimal Closing { get; set; } // Calculated: Opening + Accrued - Used - Pending maybe? Or just informational
    
    // Concurrency token mapping to PostgreSQL's internal xmin column
    public uint Version { get; set; }
}
