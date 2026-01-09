using Microsoft.EntityFrameworkCore;
using MOE_System.Domain.Entities;
using MOE_System.EService.Application.Interfaces;
using MOE_System.EService.Infrastructure.Data;

namespace MOE_System.EService.Infrastructure.Repositories
{
    public class EducationAccountRepository : IEducationAccountRepository
    {
        private readonly ApplicationDbContext _context;

        public EducationAccountRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<EducationAccount?> GetEducationAccountAsync(string educationAccountId)
        {
            return await _context.EducationAccounts
                .FirstOrDefaultAsync(ea => ea.Id == educationAccountId);
        }
    }
}
