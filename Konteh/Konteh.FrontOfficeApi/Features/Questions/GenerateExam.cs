using Konteh.Domain;
using Konteh.Domain.Enum;
using Konteh.Infrastructure.Exceptions;
using Konteh.Infrastructure.Repository;
using MediatR;

namespace Konteh.FrontOfficeApi.Features.Questions;

public static class GenerateExam
{
    public class Command : IRequest<Response>
    {
        public int CandidateId { get; set; }
    }

    public class Response
    {
        public int ExamId { get; set; }
        public IEnumerable<QuestionResponse> Questions { get; set; } = new List<QuestionResponse>();
    }

    public class QuestionResponse
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public Category Category { get; set; }
        public TypeButton Type { get; set; }
        public List<AnswerResponse> Answers { get; set; } = [];
    }

    public class AnswerResponse
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
    }

    public class RequestHandler : IRequestHandler<Command, Response>
    {
        private readonly IRepository<Question> _questionRepository;
        private readonly IConfiguration _configuration;
        private readonly IRepository<Exam> _examRepository;
        private readonly IRepository<Candidate> _candidateRepository;

        public RequestHandler(IRepository<Question> questionRepository,
            IConfiguration configuration,
            IRepository<Exam> examRepository,
            IRepository<Candidate> candidateRepository)
        {
            _questionRepository = questionRepository;
            _configuration = configuration;
            _examRepository = examRepository;
            _candidateRepository = candidateRepository;
        }

        public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            var candidate = await _candidateRepository.GetById(request.CandidateId);
            if (candidate == null)
            {
                throw new EntityNotFoundException();
            }

            int defaultNumberOfQuestions = _configuration.GetValue<int>("DefaultNumberOfQuestions");
            var allQuestions = await _questionRepository.GetAll();
            var allAnswers = allQuestions.SelectMany(x => x.Answers);

            var questionsByCategory = allQuestions.GroupBy(q => q.Category).ToDictionary(g => g.Key, g => g.ToList());

            int questionsPerCategory = defaultNumberOfQuestions / questionsByCategory.Count;
            int questionsLeft = defaultNumberOfQuestions % questionsByCategory.Count;

            List<Question> randomQuestions = new List<Question>();

            Random random = new Random();

            foreach (var categoryQuestions in questionsByCategory.Values)
            {
                int questionsToTake = questionsPerCategory + (questionsLeft-- > 0 ? 1 : 0);

                var shuffledQuestions = categoryQuestions.OrderBy(q => random.Next()).ToList();
                randomQuestions.AddRange(shuffledQuestions.Take(questionsToTake));
            }
            randomQuestions = randomQuestions.OrderBy(q => random.Next()).ToList();


            var existingExam = await _examRepository.Find(e => e.Candidate.Id == request.CandidateId);
            if (existingExam.Any())
            {
                throw new Exception("Candidate has already taken an exam.");
            }

            var newExam = new Exam
            {
                Candidate = candidate,
                ExamQuestions = randomQuestions.Select(q => new ExamQuestion { Question = q }).ToList()
            };

            await _examRepository.Add(newExam);
            await _examRepository.SaveChanges();

            var selectedQuestions = randomQuestions.Select(q => new QuestionResponse
            {
                Id = q.Id,
                Text = q.Text,
                Category = q.Category,
                Type = q.Type,
                Answers = q.Answers.Select(a => new AnswerResponse
                {
                    Id = a.Id,
                    Text = a.Text
                }).ToList()
            }).ToList();

            return new Response
            {
                ExamId = newExam.Id,
                Questions = selectedQuestions
            };
        }
    }
}
