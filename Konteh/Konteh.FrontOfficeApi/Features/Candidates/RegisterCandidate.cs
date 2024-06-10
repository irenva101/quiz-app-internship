using FluentValidation;
using Konteh.Infrastructure.Repository;
using MediatR;

namespace Konteh.FrontOfficeApi.Features.Candidates;

public static class RegisterCandidate
{
    public class Command : IRequest
    {
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Faculty { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
    }

    public class RegisterCandidateValidator : AbstractValidator<Command>
    {
        private readonly IRepository<Candidate> _candidateRepository;
        public RegisterCandidateValidator(IRepository<Candidate> candidateRepository)
        {
            _candidateRepository = candidateRepository;

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MustAsync(BeUniqueEmail)
                .WithMessage("Email is already in use");

            RuleFor(x => x.FirstName)
                .NotEmpty();

            RuleFor(x => x.LastName)
                .NotEmpty();

            RuleFor(x => x.Faculty)
                .NotEmpty();

            RuleFor(x => x.Course)
                .NotEmpty();

        }
        private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
        {
            var existingCandidates = await _candidateRepository.Find(c => c.Email == email);
            return !existingCandidates.Any();
        }
    }

    public class RequestHandler : IRequestHandler<Command>
    {
        private readonly IRepository<Candidate> _candidateRepository;
        public RequestHandler(IRepository<Candidate> candidateRepository)
        {
            _candidateRepository = candidateRepository;
        }

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var newCandidate = new Candidate
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Faculty = request.Faculty,
                Course = request.Course
            };
            await _candidateRepository.Add(newCandidate);
            await _candidateRepository.SaveChanges();
        }
    }
}