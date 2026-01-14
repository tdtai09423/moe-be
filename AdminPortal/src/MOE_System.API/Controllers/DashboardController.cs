using Microsoft.AspNetCore.Mvc;
using MOE_System.Application.Common.Dashboard;
using MOE_System.Application.DTOs.Dashboard.Response;
using MOE_System.Application.Interfaces.Services;

namespace MOE_System.API.Controllers;

[ApiController]
[Route("api/v1/admin/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("scheduled-topups")]
    public async Task<ActionResult<IReadOnlyList<ScheduledTopUpResponse>>> GetScheduledTopUpsAsync(CancellationToken cancellationToken, [FromQuery] ScheduledTopUpTypes type = ScheduledTopUpTypes.Batch)
    {
        var result = await _dashboardService.GetTopUpTypesAsync(type, cancellationToken);
        return Ok(result);
    }

    [HttpGet("recent-activities")]
    public async Task<ActionResult<IReadOnlyList<RecentActivityResponse>>> GetRecentActivitiesAsync(CancellationToken cancellationToken)
    {
        var result = await _dashboardService.GetRecentActivitiesAsync(cancellationToken);
        return Ok(result);
    }
}