using LMS.Domain.Enums;

namespace LMS.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public Role Role { get; set; }
    public bool IsActive { get; set; } = true;
    
    public EmployeeProfile? EmployeeProfile { get; set; }
}
