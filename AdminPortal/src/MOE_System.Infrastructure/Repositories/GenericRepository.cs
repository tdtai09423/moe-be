using MOE_System.Application.Common;
using MOE_System.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using MOE_System.Application.Common.Interfaces;

namespace MOE_System.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public IQueryable<T> Entities => _context.Set<T>();

        public void Delete(object entity)
        {
            _dbSet.Remove((T)entity);
        }

        public async Task DeleteAsync(object entity)
        {
            _dbSet.Remove((T)entity);
            await Task.CompletedTask;
        }

        public T? Find(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.FirstOrDefault(predicate);
        }

        public T? GetById(object id)
        {

            return _dbSet.Find(id);
        }

        public async Task<T?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<PaginatedList<T>> GetPagging(IQueryable<T> query, int index, int pageSize)
        {
            return await query.GetPaginatedList(index, pageSize);
        }

        public void Insert(T obj)   
        {
            _dbSet.Add(obj);
        }

        public async Task InsertAsync(T obj)
        {
            await _dbSet.AddAsync(obj);
        }
        public void InsertRange(List<T> obj)
        {
            _dbSet.AddRange(obj);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Update(T obj)
        {
            _context.Entry(obj).State = EntityState.Modified;
        }

        public Task UpdateAsync(T obj)
        {
            return Task.FromResult(_dbSet.Update(obj));
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task InsertRangeAsync(List<T> obj)
        {
            await _dbSet.AddRangeAsync(obj);
        }

        public Task<List<T>> ToListAsync(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IQueryable<T>>? include = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            int take = 0,
            CancellationToken cancellationToken = default
        )
        {
            IQueryable<T> query = _dbSet;
            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (take > 0)
            {
                query = query.Take(take);
            }
            return query.ToListAsync(cancellationToken);
        }

        public Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>>? include = null, bool asTracking = false, CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = asTracking ? _dbSet : _dbSet.AsNoTracking();

            if (include != null)
            {
                query = include(query);
            }

            return query.FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public Task<List<T>> ToListAsync(Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IQueryable<T>>? include = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, int take = 0, bool asTracking = false, CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = asTracking ? _dbSet : _dbSet.AsNoTracking();

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (take > 0)
            {
                query = query.Take(take);
            }

            return query.ToListAsync(cancellationToken);
        }
    }
}
