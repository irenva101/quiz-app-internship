namespace Konteh.Infrastructure.Repository;

public class CandidateRepository : BaseRepository<Candidate>
{
    public CandidateRepository(KontehContext context) : base(context)
    {
    }
}