using Microsoft.EntityFrameworkCore.Storage;
using MOE_System.EService.Application.Common.Interfaces;
using MOE_System.EService.Domain.Common;
using MOE_System.EService.Infrastructure.Data;

namespace MOE_System.EService.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private bool disposed = false;
        
        // Add custom repositories here
        // public ICustomRepository CustomRepository { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            // Initialize custom repositories here
            // CustomRepository = new CustomRepository(_context);
        }

        public IGenericRepository<T> GetRepository<T>() where T : class
        {
            return new GenericRepository<T>(_context);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void BeginTransaction()
        {
            _context.Database.BeginTransaction();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public void CommitTransaction()
        {
            _context.Database.CommitTransaction();
        }

        public void RollBack()
        {
            _context.Database.RollbackTransaction();
        }

        public bool IsValid<T>(string id) where T : BaseEntity
        {
            var entity = GetRepository<T>().GetById(id);
            return (entity is not null && entity.DeletedBy is null);
        }
    }
}
