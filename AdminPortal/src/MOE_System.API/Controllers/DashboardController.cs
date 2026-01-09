using Microsoft.AspNetCore.Mvc;
using MOE_System.Application.DTOs.Dashboard.Request;
using MOE_System.Application.DTOs.Dashboard.Response;
using MOE_System.Application.Interfaces.Services;

namespace MOE_System.API.Controllers;

[ApiController]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("overview")]
    public async Task<ActionResult<DashboardOverviewResponse>> GetDashboardOverview(
        [FromQuery] DashboardOverviewRequest request,
        CancellationToken cancellationToken = default)
    {
        var overview = await _dashboardService.GetDashboardOverviewAsync(
            request.DateRangeType,
            request.From,
            request.To,
            cancellationToken
        );
            
        return Ok(overview);
    }
}