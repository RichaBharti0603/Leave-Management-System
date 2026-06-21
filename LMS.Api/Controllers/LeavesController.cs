using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class LeavesController : ControllerBase
{
    private readonly ILeaveRequestService _leaveRequestService;

    public LeavesController(ILeaveRequestService leaveRequestService)
    {
        _leaveRequestService = leaveRequestService;
    }

    private int GetCurrentUserId() => int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");

    [HttpGet("balances")]
    public IActionResult GetBalances()
    {
        // For simplicity, returning a placeholder. Should use a BalanceService in real app.
        return Ok(new { message = "Balances will be retrieved from DB here." });
    }

    [HttpPost("requests")]
    public async Task<IActionResult> CreateRequest([FromBody] CreateLeaveRequestDto dto)
    {
        var result = await _leaveRequestService.CreateRequestAsync(GetCurrentUserId(), dto);
        return Ok(result);
    }

    [HttpPost("requests/{id}/submit")]
    public async Task<IActionResult> SubmitRequest(int id)
    {
        await _leaveRequestService.SubmitRequestAsync(GetCurrentUserId(), id);
        return Ok(new { message = "Request submitted successfully" });
    }

    [HttpGet("requests")]
    public async Task<IActionResult> GetRequests([FromQuery] bool mine = false)
    {
        if (mine)
        {
            var requests = await _leaveRequestService.GetMyRequestsAsync(GetCurrentUserId());
            return Ok(requests);
        }
        return Forbid();
    }

    [HttpPost("requests/{id}/cancel")]
    public async Task<IActionResult> CancelRequest(int id)
    {
        await _leaveRequestService.CancelRequestAsync(GetCurrentUserId(), id);
        return Ok(new { message = "Request cancelled successfully" });
    }
}
