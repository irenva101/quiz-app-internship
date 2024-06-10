using Konteh.Domain;
using Konteh.Domain.Enum;
using Konteh.Infrastructure.Repository;
using MediatR;
using System.Linq.Expressions;

namespace Konteh.BackOfficeApi.Features.Questions;

public static class GetAllQuestions
{
    public class Query : IRequest<Response>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string? FilterText { get; set; }
    }

    public class ResponseItem
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public TypeButton Type { get; set; }
        public Category Category { get; set; }
    }

    public class Response
    {
        public IEnumerable<ResponseItem> Questions { get; set; } = [];
        public int TotalCount { get; set; }
    }

    public class RequestHandler : IRequestHandler<Query, Response>
    {
        private readonly IRepository<Question> _repository;

        public RequestHandler(IRepository<Question> repository)
        {
            _repository = repository;
        }

        public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
        {

            Expression<Func<Question, bool>> filterExpression = string.IsNullOrEmpty(request.FilterText)
                 ? (_ => true)
                 : (question => question.Text.Contains(request.FilterText));

            var totalQuestions = await _repository.Count(filterExpression);

            var questions = await _repository.Find(filterExpression, request.PageSize, request.Page);

            var questionResponses = questions.Select(x => new ResponseItem
            {
                Id = x.Id,
                Text = x.Text,
                Category = x.Category,
                Type = x.Type
            }).ToList();

            return new Response
            {
                Questions = questionResponses,
                TotalCount = totalQuestions
            };
        }
    }
}