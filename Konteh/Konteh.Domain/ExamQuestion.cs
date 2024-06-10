namespace Konteh.Domain;
public class ExamQuestion
{
    public int Id { get; set; }
    public Question Question { get; set; } = null!;
    public Exam Exam { get; set; } = null!;
}