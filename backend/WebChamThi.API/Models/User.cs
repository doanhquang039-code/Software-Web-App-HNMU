namespace WebChamThi.API.Models;

public enum UserRole
{
    Student = 0,
    Teacher = 1,
    Admin = 2
}

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Student;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Exam> CreatedExams { get; set; } = new List<Exam>();
    public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
}

