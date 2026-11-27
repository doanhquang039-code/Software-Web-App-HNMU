using Microsoft.AspNetCore.SignalR;

namespace WebChamThi.API.Hubs;

public class GradingHub : Hub
{
    // Client (React) gọi hàm này để lắng nghe tiến trình chấm của 1 bài nộp cụ thể
    public Task JoinSubmissionGroup(int submissionId)
        => Groups.AddToGroupAsync(Context.ConnectionId, $"submission-{submissionId}");
}

