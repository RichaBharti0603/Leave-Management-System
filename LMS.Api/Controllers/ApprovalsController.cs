using LMS.Application.Interfaces;
using LMS.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Api.Controllers;

[Authorize(Roles = "Manager,HR,Admin")]
[ApiController]
[Route("api/[controller]")]
public class ApprovalsController : ControllerBase
{
    private readonly IApprovalService _approvalService;

    public ApprovalsController(IApprovalService approvalService)
    {
        _approvalService = approvalService;
    }

    private int GetCurrentUserId() => int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");

    [HttpGet("inbox")]
    public IActionResult GetInbox()
    {
        return Ok(new { message = "Inbox requests will be retrieved here based on role and manager mapping." });
    }

    [HttpPost("{requestId}/approve")]
    public async Task<IActionResult> Approve(int requestId, [FromBody] ApprovalRequestDto dto)
    {
        await _approvalService.ApproveAsync(GetCurrentUserId(), requestId, dto.Comments);
        return Ok(new { message = "Request approved." });
    }

    [HttpPost("{requestId}/reject")]
    public async Task<IActionResult> Reject(int requestId, [FromBody] ApprovalRequestDto dto)
    {
        await _approvalService.RejectAsync(GetCurrentUserId(), requestId, dto.Comments);
        return Ok(new { message = "Request rejected." });
    }

    [HttpPost("{requestId}/send-back")]
    public async Task<IActionResult> SendBack(int requestId, [FromBody] ApprovalRequestDto dto)
    {
        await _approvalService.SendBackAsync(GetCurrentUserId(), requestId, dto.Comments);
        return Ok(new { message = "Request sent back to employee." });
    }
}

public class ApprovalRequestDto
{
    public string? Comments { get; set; }
}
