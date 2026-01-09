using Microsoft.EntityFrameworkCore;
using MOE_System.Domain.Entities;
using MOE_System.EService.Application.Interfaces;
using MOE_System.EService.Infrastructure.Data;

namespace MOE_System.EService.Infrastructure.Repositories
{
    public class AccountHolderRepository : IAccountHolderRepository
    {
        private readonly ApplicationDbContext _context;

        public AccountHolderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AccountHolder?> GetAccountHolderAsync(string accountHolderId)
        {
            return await _context.AccountHolders
                .Include(a => a.EducationAccount)
                .FirstOrDefaultAsync(a => a.Id == accountHolderId);
        }
    }
}
