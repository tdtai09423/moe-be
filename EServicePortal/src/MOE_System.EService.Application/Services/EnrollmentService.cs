using MOE_System.EService.Application.DTOs;
using MOE_System.EService.Application.Interfaces.Services;
using MOE_System.EService.Application.Interfaces;
using MOE_System.Domain.Entities;
using static MOE_System.Domain.Common.BaseException;

namespace MOE_System.EService.Application.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IEnrollmentRepository _enrollmentRepository;

        public EnrollmentService(IEnrollmentRepository enrollmentRepository)
        {
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<List<ActiveCourseForAccountResponse>> GetActiveCoursesForAccountAsync(string accountHolderId)
        {
            var activeEnrollments = await _enrollmentRepository.GetActiveCoursesForAccountAsync(accountHolderId);

            if (!activeEnrollments.Any())
            {
                return new List<ActiveCourseForAccountResponse>();
            }

            var response = activeEnrollments.Select(enrollment => new ActiveCourseForAccountResponse
            {
                EnrollmentId = enrollment.Id,
                CourseName = enrollment.CourseOffering?.Course?.CourseName ?? string.Empty,
                CourseCode = enrollment.CourseOffering?.Course?.CourseCode ?? string.Empty,
                ProviderName = enrollment.CourseOffering?.Course?.Provider?.Name ?? string.Empty,
                EnrollDate = enrollment.EnrollDate,
                Status = enrollment.Status
            }).ToList();

            return response;
        }

        public async Task<EducationAccountOutstandingFeeResponse> GetOutstandingFeeForAccountAsync(string accountHolderId)
        {
            var enrollments = await _enrollmentRepository.GetOutstandingFeeForAccountAsync(accountHolderId);

            if (!enrollments.Any())
            {
                return new EducationAccountOutstandingFeeResponse
                {
                    AccountHolderId = accountHolderId,
                    TotalOutstandingFee = 0,
                    OutstandingInvoices = new List<OutstandingInvoiceInfo>()
                };
            }

            var outstandingInvoices = enrollments
                .SelectMany(e => e.Invoices ?? new List<Invoice>())
                .Where(i => i.Status == "Outstanding")
                .Select(i => new OutstandingInvoiceInfo
                {
                    InvoiceId = i.Id,
                    EnrollmentId = i.EnrollmentId,
                    CourseName = i.Enrollment?.CourseOffering?.Course?.CourseName ?? string.Empty,
                    Amount = i.Amount,
                    DueDate = i.DueDate,
                    Status = i.Status
                })
                .ToList();

            var totalOutstanding = outstandingInvoices.Sum(x => x.Amount);

            var response = new EducationAccountOutstandingFeeResponse
            {
                AccountHolderId = accountHolderId,
                TotalOutstandingFee = totalOutstanding,
                OutstandingInvoices = outstandingInvoices
            };

            return response;
        }
    }
}
