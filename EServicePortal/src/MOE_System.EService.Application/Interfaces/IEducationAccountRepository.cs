using MOE_System.Domain.Entities;

namespace MOE_System.EService.Application.Interfaces
{
    public interface IEducationAccountRepository
    {
        Task<EducationAccount?> GetEducationAccountAsync(string educationAccountId);
    }
}
