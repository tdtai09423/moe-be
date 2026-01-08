using System;
using System.Collections.Generic;
using System.Text;

namespace MOE_System.Application.EService.Interfaces.Services
{
    public interface IEducationAccountService
    {
        Task<decimal> GetEducationAccountBalanceAsync(string educationAccountId);
    }
}
