namespace Konteh.Infrastructure.Repository;

public class ExamRepository : BaseRepository<Exam>
{
    public ExamRepository(KontehContext context) : base(context)
    {
    }
}