using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Konteh.Infrastructure.Repository;

public abstract class BaseRepository<T> : IRepository<T> where T : class
{
    private readonly KontehContext _context;
    protected readonly DbSet<T> DbSet;

    protected BaseRepository(KontehContext context)
    {
        _context = context;
        DbSet = context.Set<T>();
    }

    public virtual async Task<IEnumerable<T>> GetAll() => await DbSet.ToListAsync();

    public virtual async Task<T?> GetById(int id) => await DbSet.FindAsync(id);

    public void Delete(T entity) => DbSet.Remove(entity);

    public async Task Add(T entity) => await DbSet.AddAsync(entity);

    public virtual async Task<IEnumerable<T>> Find(Expression<Func<T, bool>> predicate, int? pageSize, int? pageNumber)
    {
        var query = DbSet.Where(predicate);

        if (pageSize.HasValue && pageNumber.HasValue)
        {
            query = query.Skip((pageNumber.Value - 1) * pageSize.Value).Take(pageSize.Value);
        }

        return await query.ToListAsync();
    }
    public async Task<IEnumerable<T>> Find(Expression<Func<T, bool>> predicate) => await DbSet.Where(predicate).ToListAsync();

    public virtual async Task<int> Count(Expression<Func<T, bool>> predicate)
    {
        return await DbSet.CountAsync(predicate);
    }

    public async Task SaveChanges() => await _context.SaveChangesAsync();
}