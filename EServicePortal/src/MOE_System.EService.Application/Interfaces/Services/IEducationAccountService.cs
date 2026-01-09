using MOE_System.EService.Application.DTOs;

namespace MOE_System.EService.Application.Interfaces.Services
{
    public interface IEducationAccountService
    {
        Task<EducationAccountBalanceResponse> GetEducationAccountBalanceAsync(string educationAccountId);
    }
}
