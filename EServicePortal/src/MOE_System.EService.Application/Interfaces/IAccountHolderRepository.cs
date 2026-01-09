using MOE_System.Domain.Entities;

namespace MOE_System.EService.Application.Interfaces
{
    public interface IAccountHolderRepository
    {
        Task<AccountHolder?> GetAccountHolderAsync(string accountHolderId);
    }
}
