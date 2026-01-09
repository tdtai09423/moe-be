using Microsoft.EntityFrameworkCore;
using MOE_System.Domain.Entities;
using MOE_System.EService.Application.Interfaces;
using MOE_System.EService.Infrastructure.Data;

namespace MOE_System.EService.Infrastructure.Repositories
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly ApplicationDbContext _context;

        public EnrollmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Enrollment>> GetActiveCoursesForAccountAsync(string accountHolderId)
        {
            return await _context.Enrollments
                .Include(e => e.CourseOffering)
                    .ThenInclude(co => co!.Course)
                        .ThenInclude(c => c!.Provider)
                .Include(e => e.EducationAccount)
                .Where(e => e.EducationAccount!.AccountHolderId == accountHolderId)
                .ToListAsync();
        }

        public async Task<List<Enrollment>> GetOutstandingFeeForAccountAsync(string accountHolderId)
        {
            return await _context.Enrollments
                .Include(e => e.Invoices)
                    .ThenInclude(i => i.Transactions)
                .Include(e => e.CourseOffering)
                    .ThenInclude(co => co!.Course)
                .Include(e => e.EducationAccount)
                .Where(e => e.EducationAccount!.AccountHolderId == accountHolderId)
                .ToListAsync();
        }
    }
}
