using Common.Models.Enum;

namespace Common.Models;

public class Question
{
    public int Id { get; set; }
    public TypeButton Type { get; set; }
    public Category Category { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public List<Answer> Answers { get; set; } = [];
}