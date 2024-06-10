using Konteh.Domain;
using Konteh.Infrastructure.Exceptions;
using Konteh.Infrastructure.Repository;
using MediatR;

namespace Konteh.BackOfficeApi.Features.Questions;

public static class DeleteQuestion
{
    public class Command : IRequest
    {
        public int Id { get; set; }
    }

    public class RequestHandler : IRequestHandler<Command>
    {
        private readonly IRepository<Question> _questionRepository;
        public RequestHandler(IRepository<Question> questionRepository)
        {
            _questionRepository = questionRepository;
        }
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var question = await _questionRepository.GetById(request.Id);
            if (question == null)
            {
                throw new EntityNotFoundException();
            }

            question.IsDeleted = true;

            foreach (var item in question.Answers)
            {
                item.IsDeleted = true;
            }
            await _questionRepository.SaveChanges();
        }


    }
}
