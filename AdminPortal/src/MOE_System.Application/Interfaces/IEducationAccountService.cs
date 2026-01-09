using MOE_System.Application.DTOs;

namespace MOE_System.Application.Interfaces;

public interface IEducationAccountService
{
    Task<AccountBalanceDto?> GetAccountBalanceAsync(string accountId);
    Task<OutstandingFeeDto?> GetOutstandingFeesAsync(string accountId);
}
