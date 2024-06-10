using FluentValidation;
using Konteh.BackOfficeApi.Features.Questions.Models;
using Konteh.Domain;
using Konteh.Domain.Enum;
using Konteh.Infrastructure.Exceptions;
using Konteh.Infrastructure.Repository;
using MediatR;

namespace Konteh.BackOfficeApi.Features.Questions;

public static class CreateOrUpdateQuestion
{
    public class Command : IRequest
    {
        public int? Id { get; set; }
        public Category Category { get; set; }
        public TypeButton TypeButton { get; set; }
        public string Text { get; set; } = string.Empty;
        public List<AnswerItem> Answers { get; set; } = [];
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(question => question.Text)
                .NotEmpty();

            RuleFor(question => question.TypeButton)
                .IsInEnum();

            RuleFor(question => question.Category)
                .IsInEnum();

            RuleFor(question => question.Answers)
                .NotEmpty();

            RuleForEach(question => question.Answers)
                .SetValidator(new AnswerItem.Validator());

            RuleFor(x => x.Answers)
                .Must(HaveValidAnswerCount)
                .When(x => x.Answers != null && x.Answers.Count > 0)
                .WithMessage("{answers}");
        }

        private bool HaveValidAnswerCount(Command command, List<AnswerItem> answers, ValidationContext<Command> context)
        {
            if (command.TypeButton == TypeButton.Radiobutton)
            {
                if (answers.Count(x => x.IsCorrect) != 1)
                {
                    context.MessageFormatter.AppendArgument("answers", "RadioButton questions can only have one correct answer");
                    return false;
                }
            }
            if (command.TypeButton == TypeButton.Checkbox)
            {
                if (!answers.Any(x => x.IsCorrect))
                {
                    context.MessageFormatter.AppendArgument("answers", "Checkbox questions must contain at least one correct answer");
                    return false;
                }
            }
            return true;
        }
    }

    public class RequestHandler : IRequestHandler<Command>
    {
        private readonly IRepository<Question> _questionRepositry;
        public RequestHandler(IRepository<Question> repositoryQuestion)
        {
            _questionRepositry = repositoryQuestion;
        }
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            if (!request.Id.HasValue)
            {
                var newQuestion = new Question
                {
                    IsDeleted = false,
                    Text = request.Text,
                    Category = request.Category,
                    Type = request.TypeButton,
                    Answers = request.Answers.Select(x => new Answer
                    {
                        IsCorrect = x.IsCorrect,
                        Text = x.Text,
                        IsDeleted = false

                    }).ToList()
                };
                await _questionRepositry.Add(newQuestion);
            }
            else
            {
                var existingQuestion = await _questionRepositry.GetById(request.Id.Value);
                if (existingQuestion == null)
                {
                    throw new EntityNotFoundException();
                }
                else
                {
                    existingQuestion.Text = request.Text;
                    existingQuestion.Category = request.Category;
                    existingQuestion.Type = request.TypeButton;

                    foreach (var existingAnswer in existingQuestion.Answers)
                    {
                        var answerInRequest = request.Answers.SingleOrDefault(x => x.Id == existingAnswer.Id);
                        if (answerInRequest == null)
                        {
                            existingAnswer.IsDeleted = true;
                        }
                    }

                    foreach (var answer in request.Answers)
                    {
                        var existingAnswer = existingQuestion.Answers.SingleOrDefault(x => x.Id == answer.Id);
                        if (existingAnswer == null)
                        {
                            existingQuestion.Answers.Add(new Answer
                            {
                                IsCorrect = answer.IsCorrect,
                                Text = answer.Text,
                                IsDeleted = false
                            });
                        }
                        else
                        {
                            existingAnswer.Text = answer.Text;
                            existingAnswer.IsCorrect = answer.IsCorrect;
                            existingAnswer.IsDeleted = false;
                        }
                    }
                }
            }
            await _questionRepositry.SaveChanges();
        }
    }
}
