using FluentValidation;
using Konteh.BackOfficeApi.Features.Questions.Models;
using Konteh.Domain;
using Konteh.Domain.Enum;
using Konteh.Infrastructure.Exceptions;
using Konteh.Infrastructure.Repository;
using MediatR;

namespace Konteh.BackOfficeApi.Features.Questions;

public static class GetById
{
    public class Query : IRequest<Response>
    {
        public int Id { get; set; }
    }
    public class Response
    {
        public int Id { get; set; }
        public Category Category { get; set; }
        public TypeButton TypeButton { get; set; }
        public string Text { get; set; } = string.Empty;

        public List<AnswerItem> Answers { get; set; } = [];
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(query => query.Id)
                .GreaterThan(0)
                .WithMessage("Id must be a positive integer.");
        }
    }
    public class RequestHandler : IRequestHandler<Query, Response>
    {
        private readonly IRepository<Question> _questionRepository;
        private readonly IRepository<Answer> _answerRepository;

        public RequestHandler(IRepository<Question> repositoryQuestion, IRepository<Answer> repositoryAnswer)
        {
            _questionRepository = repositoryQuestion;
            _answerRepository = repositoryAnswer;
        }

        public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
        {
            var question = await _questionRepository.GetById(request.Id);

            if (question == null)
            {
                throw new EntityNotFoundException();
            }

            return new Response
            {
                Id = question.Id,
                Text = question.Text,
                Category = question.Category,
                TypeButton = question.Type,
                Answers = question.Answers.Select(x => new AnswerItem
                {
                    Id = x.Id,
                    IsCorrect = x.IsCorrect,
                    Text = x.Text
                }).ToList()
            };
        }
    }
}
