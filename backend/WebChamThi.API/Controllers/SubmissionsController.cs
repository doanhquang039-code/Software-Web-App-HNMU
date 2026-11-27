using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebChamThi.API.Data;
using WebChamThi.API.Models;
using WebChamThi.API.Services;

namespace WebChamThi.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubmissionsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly SubmissionQueue _queue;

    public SubmissionsController(AppDbContext db, SubmissionQueue queue)
    {
        _db = db;
        _queue = queue;
    }

    public record CreateSubmissionRequest(
        int QuestionId,
        int StudentId,
        string? Language,
        string? SourceCode,
        string? EssayAnswer);

    [HttpPost]
    public async Task<IActionResult> Submit([FromBody] CreateSubmissionRequest req, CancellationToken ct)
    {
        var questionExists = await _db.Questions.AnyAsync(q => q.Id == req.QuestionId, ct);
        if (!questionExists) return NotFound("Không tìm thấy câu hỏi.");

        var submission = new Submission
        {
            QuestionId = req.QuestionId,
            StudentId = req.StudentId,
            Language = req.Language,
            SourceCode = req.SourceCode,
            EssayAnswer = req.EssayAnswer,
            Status = SubmissionStatus.Pending
        };

        _db.Submissions.Add(submission);
        await _db.SaveChangesAsync(ct);

        await _queue.EnqueueAsync(submission.Id, ct);

        return Accepted(new { submission.Id, Status = submission.Status.ToString() });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var s = await _db.Submissions
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new
            {
                x.Id,
                x.QuestionId,
                x.StudentId,
                x.Language,
                Status = x.Status.ToString(),
                x.Score,
                x.ExecutionTimeMs,
                x.MemoryUsedKb,
                x.AiFeedbackJson,
                x.IsApprovedByTeacher,
                x.SubmittedAt
            })
            .FirstOrDefaultAsync(ct);

        return s is null ? NotFound() : Ok(s);
    }
}

