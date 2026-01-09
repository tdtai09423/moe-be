using MOE_System.Application.Common.Interfaces;
using MOE_System.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MOE_System.Application.EService.Interfaces.Repositories
{
    public interface IEducationAccountRepository : IGenericRepository<EducationAccount>
    {
        Task<EducationAccount> GetEducationAccountAsync(string educationAccountId);
    }
}
