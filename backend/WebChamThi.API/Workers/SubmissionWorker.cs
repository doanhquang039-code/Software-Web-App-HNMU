using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WebChamThi.API.Data;
using WebChamThi.API.Hubs;
using WebChamThi.API.Models;
using WebChamThi.API.Services;

namespace WebChamThi.API.Workers;

public class SubmissionWorker : BackgroundService
{
    private readonly SubmissionQueue _queue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IHubContext<GradingHub> _hubContext;
    private readonly ILogger<SubmissionWorker> _logger;

    public SubmissionWorker(
        SubmissionQueue queue,
        IServiceScopeFactory scopeFactory,
        IHubContext<GradingHub> hubContext,
        ILogger<SubmissionWorker> logger)
    {
        _queue = queue;
        _scopeFactory = scopeFactory;
        _hubContext = hubContext;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var submissionId in _queue.DequeueAllAsync(stoppingToken))
        {
            try
            {
                await ProcessSubmissionAsync(submissionId, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi chấm bài nộp {SubmissionId}", submissionId);
            }
        }
    }

    private async Task ProcessSubmissionAsync(int submissionId, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var submission = await db.Submissions
            .Include(s => s.Question)
            .ThenInclude(q => q.TestCases)
            .FirstOrDefaultAsync(s => s.Id == submissionId, ct);

        if (submission is null) return;

        // 1. Cập nhật trạng thái đang chấm & báo realtime về Frontend
        submission.Status = SubmissionStatus.Grading;
        await db.SaveChangesAsync(ct);
        await NotifyAsync(submission, ct);

        // 2. Phân luồng chấm Code (Docker Sandbox) hoặc Tự luận (AI Rubric)
        if (submission.Question.Type == QuestionType.Code)
        {
            // TODO: Tích hợp Docker.DotNet chạy test cases thực tế
            await Task.Delay(500, ct);
            submission.Status = SubmissionStatus.Accepted;
            submission.Score = submission.Question.MaxScore;
            submission.ExecutionTimeMs = 45;
            submission.MemoryUsedKb = 10240;
        }
        else
        {
            // TODO: Gọi AI LLM chấm theo Question.RubricJson
            await Task.Delay(500, ct);
            submission.Status = SubmissionStatus.AiGradedPendingReview;
            submission.Score = submission.Question.MaxScore * 0.8m;
            submission.AiFeedbackJson = """{"summary":"Đáp án đúng ý chính, chờ giáo viên duyệt."}""";
        }

        await db.SaveChangesAsync(ct);
        await NotifyAsync(submission, ct);
    }

    private Task NotifyAsync(Submission s, CancellationToken ct)
        => _hubContext.Clients
            .Group($"submission-{s.Id}")
            .SendAsync("SubmissionUpdated", new
            {
                s.Id,
                Status = s.Status.ToString(),
                s.Score,
                s.ExecutionTimeMs,
                s.MemoryUsedKb,
                s.AiFeedbackJson
            }, ct);
}

