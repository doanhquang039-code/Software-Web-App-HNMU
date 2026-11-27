using Microsoft.EntityFrameworkCore;
using WebChamThi.API.Models;

namespace WebChamThi.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Exam> Exams => Set<Exam>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<TestCase> TestCases => Set<TestCase>();
    public DbSet<Submission> Submissions => Set<Submission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Username).IsUnique();
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Username).HasMaxLength(50);
            entity.Property(u => u.FullName).HasMaxLength(150);
            entity.Property(u => u.Email).HasMaxLength(150);
            entity.Property(u => u.Role).HasConversion<string>().HasMaxLength(30);
        });

        modelBuilder.Entity<Exam>(entity =>
        {
            entity.Property(e => e.Title).HasMaxLength(250);
            entity.HasOne(e => e.CreatedBy)
                  .WithMany(u => u.CreatedExams)
                  .HasForeignKey(e => e.CreatedById)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.Property(q => q.Title).HasMaxLength(250);
            entity.Property(q => q.Type).HasConversion<string>().HasMaxLength(30);
            entity.Property(q => q.MaxScore).HasPrecision(5, 2);
            entity.HasOne(q => q.Exam)
                  .WithMany(e => e.Questions)
                  .HasForeignKey(q => q.ExamId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TestCase>(entity =>
        {
            entity.HasOne(t => t.Question)
                  .WithMany(q => q.TestCases)
                  .HasForeignKey(t => t.QuestionId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Submission>(entity =>
        {
            entity.Property(s => s.Language).HasMaxLength(30);
            entity.Property(s => s.Status).HasConversion<string>().HasMaxLength(30);
            entity.Property(s => s.Score).HasPrecision(5, 2);

            entity.HasOne(s => s.Question)
                  .WithMany(q => q.Submissions)
                  .HasForeignKey(s => s.QuestionId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(s => s.Student)
                  .WithMany(u => u.Submissions)
                  .HasForeignKey(s => s.StudentId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}

