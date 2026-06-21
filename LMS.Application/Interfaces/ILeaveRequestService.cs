using LMS.Application.DTOs;

namespace LMS.Application.Interfaces;

public interface ILeaveRequestService
{
    Task<LeaveRequestResponseDto> CreateRequestAsync(int employeeId, CreateLeaveRequestDto dto);
    Task SubmitRequestAsync(int employeeId, int requestId);
    Task CancelRequestAsync(int employeeId, int requestId);
    Task<IEnumerable<LeaveRequestResponseDto>> GetMyRequestsAsync(int employeeId);
}
