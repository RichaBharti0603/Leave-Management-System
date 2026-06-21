namespace LMS.Domain.Entities;

public class Attachment
{
    public int Id { get; set; }
    
    public int LeaveRequestId { get; set; }
    public LeaveRequest LeaveRequest { get; set; } = null!;
    
    public string FileName { get; set; } = null!;
    public string FilePath { get; set; } = null!; // Local disk path or abstract URI
    public string ContentType { get; set; } = null!;
    
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
