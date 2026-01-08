using MOE_System.Application.EService.Interfaces.Repositories;
using MOE_System.Application.EService.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace MOE_System.Application.EService.Services
{
    public class EducationAccountService : IEducationAccountService
    {
        private readonly IEducationAccountRepository _educationAccountRepo;

        public EducationAccountService(IEducationAccountRepository educationAccountRepo)
        {
            _educationAccountRepo = educationAccountRepo;
        }

        public async Task<decimal> GetEducationAccountBalanceAsync(string educationAccountId)
        {
            var educationAccount = await _educationAccountRepo
                .GetEducationAccountAsync(educationAccountId);

            var balance = educationAccount.Balance;
            return balance;
        }

        public Task<decimal> GetOutstandingFeeAsync(string educationAccountId)
        {
            throw new NotImplementedException();
        }
    }
}
