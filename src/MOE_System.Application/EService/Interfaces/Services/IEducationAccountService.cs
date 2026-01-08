using MOE_System.Application.EService.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace MOE_System.Application.EService.Interfaces.Services
{
    public interface IEducationAccountService
    {
        Task<EducationAccountBalanceResponse> GetEducationAccountBalanceAsync(string educationAccountId);
    }
}
