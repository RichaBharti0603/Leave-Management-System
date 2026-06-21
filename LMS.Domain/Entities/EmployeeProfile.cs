namespace LMS.Domain.Entities;

public class EmployeeProfile
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public string EmployeeCode { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public int? DepartmentId { get; set; }
    public int? ManagerEmployeeId { get; set; }
    public EmployeeProfile? Manager { get; set; }
    public DateTime JoinDate { get; set; }
    public int? LocationId { get; set; }
}
