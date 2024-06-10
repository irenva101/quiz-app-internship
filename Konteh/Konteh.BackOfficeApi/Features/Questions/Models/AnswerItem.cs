using FluentValidation;

namespace Konteh.BackOfficeApi.Features.Questions.Models;

public class AnswerItem
{
    public int? Id { get; init; }
    public bool IsCorrect { get; set; }
    public string Text { get; set; } = string.Empty;

    public class Validator : AbstractValidator<AnswerItem>
    {
        public Validator()
        {
            RuleFor(x => x.Id).GreaterThan(0).When(x => x.Id.HasValue);
            RuleFor(x => x.Text).NotEmpty();
        }
    }
}
