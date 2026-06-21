using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Api.Controllers;

[Authorize(Roles = "Admin,HR")]
[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    [HttpGet("leave-types")]
    public IActionResult GetLeaveTypes()
    {
        return Ok(new { message = "List leave types." });
    }

    [HttpGet("policies")]
    public IActionResult GetPolicies()
    {
        return Ok(new { message = "List policies." });
    }

    [HttpPost("balances/adjust")]
    public IActionResult AdjustBalance()
    {
        return Ok(new { message = "Balance adjusted." });
    }

    [HttpGet("reports/leaves")]
    public IActionResult GetReports()
    {
        return Ok(new { message = "Leave report data." });
    }
}
