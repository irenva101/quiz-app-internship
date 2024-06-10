using Konteh.Domain;

namespace Konteh.Infrastructure.Repository;
public class ExamQuestionRepository : BaseRepository<ExamQuestion>
{
    public ExamQuestionRepository(KontehContext context) : base(context)
    {
    }
}
