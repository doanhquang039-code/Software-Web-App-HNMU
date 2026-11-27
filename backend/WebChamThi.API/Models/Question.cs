namespace WebChamThi.API.Models;

public enum QuestionType
{
    Code = 0,
    Essay = 1
}

public class Question
{
    public int Id { get; set; }
    public int ExamId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public QuestionType Type { get; set; }
    public decimal MaxScore { get; set; } = 10m;

    // Lưu Rubric chấm tự luận dạng JSON (tiêu chí + thang điểm)
    public string? RubricJson { get; set; }

    public Exam Exam { get; set; } = null!;
    public ICollection<TestCase> TestCases { get; set; } = new List<TestCase>();
    public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
}

