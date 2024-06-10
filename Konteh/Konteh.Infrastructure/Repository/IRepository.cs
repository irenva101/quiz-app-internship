using System.Linq.Expressions;

namespace Konteh.Infrastructure.Repository;

public interface IRepository<T>
{
    public Task<IEnumerable<T>> GetAll();
    public Task<T?> GetById(int id);
    public void Delete(T entity);
    public Task Add(T entity);
    public Task<IEnumerable<T>> Find(Expression<Func<T, bool>> predicate, int? pageSize, int? pageNumber);
    public Task<IEnumerable<T>> Find(Expression<Func<T, bool>> predicate);
    public Task<int> Count(Expression<Func<T, bool>> predicate);
    public Task SaveChanges();
}