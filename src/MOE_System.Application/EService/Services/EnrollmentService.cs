using MOE_System.Application.EService.DTOs;
using MOE_System.Application.EService.Interfaces.Repositories;
using MOE_System.Application.EService.Interfaces.Services;
using MOE_System.Domain.Common;
using MOE_System.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MOE_System.Application.EService.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IEnrollmentRepository _enrollmentRepository;

        public EnrollmentService(IEnrollmentRepository enrollmentRepository)
        {
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<IEnumerable<ActiveCourseForAccountResponse>> GetActiveCoursesForAccountAsync(string accountHolderId)
        {
            var enrollments = await _enrollmentRepository.GetActiveCoursesForAccountAsync(accountHolderId);

            if (enrollments == null || !enrollments.Any())
            {
                throw new BaseException.NotFoundException("Enrollments not found!");
            }

            var res = enrollments.Select(
                p => new ActiveCourseForAccountResponse
                {
                    EnrollmentId = p.Id,
                    CourseName = p.CourseOffering.Course.CourseName,
                    FeeAmount = p.CourseOffering.Course.FeeAmount,
                    DueDate = p.CourseOffering.EndDate,
                    Status = p.CourseOffering.Status,   
                }
                );

            return res;
        }

        public async Task<decimal> GetOutstandingFeeAsync(string educationAccountId)
        {
            var enrollments = await _enrollmentRepository.GetOutstandingFeeForAccountAsync(educationAccountId);

            if (educationAccountId == null || !enrollments.Any())
            {
                throw new BaseException.NotFoundException("Enrollments not found!");
            }

            var totalFee = enrollments.Sum(e => e.CourseOffering.Course.FeeAmount);

            var totalPaid = enrollments.SelectMany(e => e.Invoices)
                                       .SelectMany(i => i.Transactions)
                                       .Sum(t => t.Amount);

            return totalFee - totalPaid;
        }
    }
}
