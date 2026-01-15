using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MOE_System.EService.Application.Common;
using MOE_System.EService.Application.DTOs.Dashboard;
using MOE_System.EService.Application.Interfaces.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MOE_System.EService.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/dashboard")]
    public class DashboardController : BaseApiController
    {
        private readonly IDashboardService _dashboardService;
        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<DashboardResponse>>> GetAccountDashboard()
        {
            var accountHolderId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(accountHolderId))
            {
                 Unauthorized("Invalid account holder ID");
            }

            var dashboardData = await _dashboardService.GetAccountDashboardAsync(accountHolderId);

            return Success(dashboardData, "Get Account's dashboard information successfully");
        }
    }
}
