using Konteh.Domain.Enum;

namespace Konteh.Domain;

public class Question
{
    public int Id { get; set; }
    public TypeButton Type { get; set; }
    public Category Category { get; set; }
    public string Text { get; set; } = string.Empty;
    public List<Answer> Answers { get; set; } = [];
    public bool IsDeleted { get; set; }
}