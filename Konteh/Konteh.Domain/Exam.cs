using Konteh.Domain;

public class Exam
{
    public int Id { get; set; }
    public Candidate Candidate { get; set; } = new Candidate();
    public List<ExamQuestion>? ExamQuestions { get; set; }
}