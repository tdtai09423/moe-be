using MOE_System.Application.Common.Interfaces;
using MOE_System.Application.EService.DTOs;
using MOE_System.Application.EService.Interfaces.Repositories;
using MOE_System.Application.EService.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace MOE_System.Application.EService.Services
{
    public class EducationAccountService : IEducationAccountService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EducationAccountService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<EducationAccountBalanceResponse> GetEducationAccountBalanceAsync(string educationAccountId)
        {
            var educationAccount = await _unitOfWork.EducationAccounts
                .GetEducationAccountAsync(educationAccountId);

            var res = new EducationAccountBalanceResponse
            {
                Id = educationAccount.Id,
                Balance = educationAccount.Balance,
            };

            return res;
        }

    }
}
