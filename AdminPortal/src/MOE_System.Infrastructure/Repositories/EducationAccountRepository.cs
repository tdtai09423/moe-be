using Microsoft.EntityFrameworkCore;
using MOE_System.Application.EService.Interfaces.Repositories;
using MOE_System.Domain.Entities;
using MOE_System.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MOE_System.Infrastructure.Repositories
{
    public class EducationAccountRepository : GenericRepository<EducationAccount>, IEducationAccountRepository
    {
        public EducationAccountRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<EducationAccount> GetEducationAccountAsync(string educationAccountId)
        {
            var res = await _context.EducationAccounts
                .Where(e => e.Id == educationAccountId)
                .FirstOrDefaultAsync();
            return res;
        }
    }
}
