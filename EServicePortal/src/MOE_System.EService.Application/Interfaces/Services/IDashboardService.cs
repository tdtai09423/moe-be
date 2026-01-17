using MOE_System.EService.Application.DTOs.Dashboard;
using System;
using System.Collections.Generic;
using System.Text;

namespace MOE_System.EService.Application.Interfaces.Services
{
    public interface IDashboardService
    {
        Task<DashboardResponse> GetAccountDashboardAsync(string accountHolderId);
    }
}
