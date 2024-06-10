namespace Konteh.Infrastructure.Repository;

public class AnswerRepository : BaseRepository<Answer>
{
    public AnswerRepository(KontehContext context) : base(context)
    {
    }
}