namespace WebChamThi.API.Models;

public class TestCase
{
    public int Id { get; set; }
    public int QuestionId { get; set; }
    public string Input { get; set; } = string.Empty;
    public string ExpectedOutput { get; set; } = string.Empty;
    public bool IsHidden { get; set; } = true;
    public int TimeLimitMs { get; set; } = 2000;
    public int MemoryLimitMb { get; set; } = 256;

    public Question Question { get; set; } = null!;
}

