using MOE_System.Application.Common.Interfaces;
using MOE_System.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MOE_System.Application.EService.Interfaces.Repositories
{
    public interface IEnrollmentRepository : IGenericRepository<Enrollment>
    {
        Task<IEnumerable<Enrollment>> GetActiveCoursesForAccountAsync(string accountHolderId);
        Task<IEnumerable<Enrollment>> GetOutstandingFeeForAccountAsync(string educationAccountId);
    }
}
