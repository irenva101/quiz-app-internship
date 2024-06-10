namespace Konteh.FrontOfficeApi.Features.Questions.Models;

public class AnswerDTO
{
    public int? Id { get; init; }
    public bool IsCorrect { get; set; }
    public string Text { get; set; } = string.Empty;
}
