namespace WebChamThi.API.Models;

public enum SubmissionStatus
{
    Pending = 0,
    Grading = 1,
    Accepted = 2,
    WrongAnswer = 3,
    TimeLimitExceeded = 4,
    MemoryLimitExceeded = 5,
    RuntimeError = 6,
    CompileError = 7,
    AiGradedPendingReview = 8,
    Completed = 9
}

public class Submission
{
    public int Id { get; set; }
    public int QuestionId { get; set; }
    public int StudentId { get; set; }

    // Dành cho bài Code
    public string? Language { get; set; }
    public string? SourceCode { get; set; }
    public int? ExecutionTimeMs { get; set; }
    public int? MemoryUsedKb { get; set; }

    // Dành cho bài Tự luận
    public string? EssayAnswer { get; set; }
    public string? AiFeedbackJson { get; set; }
    public bool IsApprovedByTeacher { get; set; } = false;

    public SubmissionStatus Status { get; set; } = SubmissionStatus.Pending;
    public decimal? Score { get; set; }
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    public Question Question { get; set; } = null!;
    public User Student { get; set; } = null!;
}

