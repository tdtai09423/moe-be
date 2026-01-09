using Microsoft.EntityFrameworkCore;
using MOE_System.Application.EService.Interfaces.Repositories;
using MOE_System.Domain.Entities;
using MOE_System.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MOE_System.Infrastructure.Repositories
{
    public class EnrollmentRepository : GenericRepository<Enrollment>, IEnrollmentRepository
    {
        public EnrollmentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Enrollment>> GetActiveCoursesForAccountAsync(string accountHolderId)
        {   
            var enrollments = await _context.Enrollments
                .Include(e => e.EducationAccount)
                .Include(e => e.CourseOffering)
                    .ThenInclude(c => c.Course)
                .Where(e => e.EducationAccount.AccountHolderId == accountHolderId)
                .ToListAsync();

            return enrollments;
        }

        public async Task<IEnumerable<Enrollment>> GetOutstandingFeeForAccountAsync(string educationAccountId)
        {
            var enrollments = await _context.Enrollments
                .Include(e => e.Invoices)
                    .ThenInclude(i => i.Transactions)
                .Where(e => e.EducationAccountId == educationAccountId)
                .ToListAsync();
            return enrollments;
        }
    }
}
