namespace LMS.Domain.Entities;

public class Holiday
{
    public int Id { get; set; }
    public int? LocationId { get; set; } // Null implies global holiday
    public DateTime Date { get; set; }
    public string Name { get; set; } = null!;
}
