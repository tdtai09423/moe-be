using MOE_System.Application.Common;
using System.Linq.Expressions;

namespace MOE_System.Application.Common.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        //queryable
        IQueryable<T> Entities { get; }

        //void
        T? GetById(object id);

        void Insert(T obj);
        void InsertRange(List<T> obj);
        Task InsertRangeAsync(List<T> obj);

        void Update(T obj);
        void Delete(object entity);
        void Save();

        //Task
        Task<T?> GetByIdAsync(object id);
        Task<IEnumerable<T>> GetAllAsync();
        Task InsertAsync(T obj);
        Task UpdateAsync(T obj);
        Task DeleteAsync(object entity);
        Task SaveAsync();

        //another
        T? Find(Expression<Func<T, bool>> predicate);
        Task<PaginatedList<T>> GetPagging(IQueryable<T> query, int index, int pageSize);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>>? include = null, bool asTracking = false, CancellationToken cancellationToken = default);
        Task<List<T>> ToListAsync(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IQueryable<T>>? include = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            int take = 0,
            bool asTracking = false,
            CancellationToken cancellationToken = default
        );
    }
}
