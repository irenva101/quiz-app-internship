using Konteh.Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Konteh.Infrastructure.Repository;

public class QuestionRepository : BaseRepository<Question>
{
    public QuestionRepository(KontehContext context) : base(context)
    {

    }

    public override async Task<IEnumerable<Question>> GetAll() => await DbSet.Include(x => x.Answers).ToListAsync();

    public override async Task<int> Count(Expression<Func<Question, bool>> predicate)
    {
        return await DbSet.Where(x => !x.IsDeleted).CountAsync(predicate);
    }

    public override async Task<IEnumerable<Question>> Find(Expression<Func<Question, bool>> predicate, int? pageSize, int? pageNumber)
    {
        var query = DbSet.Where(x => !x.IsDeleted).Where(predicate);

        if (pageSize.HasValue && pageNumber.HasValue)
        {
            query = query.Skip((pageNumber.Value - 1) * pageSize.Value).Take(pageSize.Value);
        }

        return await query.ToListAsync();
    }

    public override async Task<Question?> GetById(int id) =>
        await DbSet
        .Include(x => x.Answers
        .Where(a => !a.IsDeleted))
        .Where(x => !x.IsDeleted)
        .SingleOrDefaultAsync(x => x.Id == id);
}