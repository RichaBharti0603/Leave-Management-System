using LMS.Application.Interfaces;

namespace LMS.Application.Interfaces;

public interface IApprovalService
{
    // Task<IEnumerable<LeaveRequestResponseDto>> GetInboxAsync(int approverId);
    Task ApproveAsync(int approverId, int requestId, string? comments);
    Task RejectAsync(int approverId, int requestId, string? comments);
    Task SendBackAsync(int approverId, int requestId, string? comments);
}
