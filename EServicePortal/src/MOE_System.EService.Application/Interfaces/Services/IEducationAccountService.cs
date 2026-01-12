using MOE_System.EService.Application.DTOs;

namespace MOE_System.EService.Application.Interfaces.Services
{
    public interface IEducationAccountService
    {
        Task<BalanceResponse> GetBalanceAsync(string educationAccountId);
        Task<OutstandingFeeResponse> GetOutstandingFeeAsync(string educationAccountId);
    }
}
