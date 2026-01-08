using Microsoft.EntityFrameworkCore;
using MOE_System.Application.EService.Interfaces.Repositories;
using MOE_System.Domain.Entities;
using MOE_System.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MOE_System.Infrastructure.Repositories
{
    public class AccountHolderRepository : GenericRepository<AccountHolder>, IAccountHolderRepository
    {
        public AccountHolderRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<AccountHolder> GetAccountHolderAsync(string accountHolderId)
        {
            var res = await _context.AccountHolders
                .Include(a => a.EducationAccount)
                .Where(a => a.Id == accountHolderId)
                .FirstOrDefaultAsync();

            return res;
        }
    }
}
