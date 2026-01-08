using MOE_System.Application.Common.Interfaces;
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
        private readonly IUnitOfWork _unitOfWork;

        public EnrollmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ActiveCourseForAccountResponse>> GetActiveCoursesForAccountAsync(string accountHolderId)
        {
            var enrollments = await _unitOfWork.Enrollments.GetActiveCoursesForAccountAsync(accountHolderId);

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
            var enrollments = await _unitOfWork.Enrollments.GetOutstandingFeeForAccountAsync(educationAccountId);

            if (educationAccountId == null || !enrollments.Any())
            {
                throw new BaseException.NotFoundException("Enrollments not found!");
            }
            decimal totalInvoiceAmount = enrollments
                .SelectMany(e => e.Invoices).Sum(i => i.Amount);

            decimal totalTransactionAmount = enrollments
                .SelectMany(e => e.Invoices)
                .SelectMany(i => i.Transactions)
                .Sum(t => t.Amount);

            return totalInvoiceAmount - totalTransactionAmount;
        }
    }
}
