using Microsoft.EntityFrameworkCore.Storage;
using MOE_System.Domain.Common;

namespace MOE_System.Application.Common.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> GetRepository<T>() where T : class;
        void Save();
        Task SaveAsync();
        void BeginTransaction();
        Task<IDbContextTransaction> BeginTransactionAsync();
        void CommitTransaction();
        void RollBack();
        bool IsValid<T>(string id) where T : BaseEntity;
    }
}
