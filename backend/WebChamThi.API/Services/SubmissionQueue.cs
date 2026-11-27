using System.Threading.Channels;

namespace WebChamThi.API.Services;

public class SubmissionQueue
{
    private readonly Channel<int> _queue = Channel.CreateUnbounded<int>();

    public ValueTask EnqueueAsync(int submissionId, CancellationToken cancellationToken = default)
        => _queue.Writer.WriteAsync(submissionId, cancellationToken);

    public IAsyncEnumerable<int> DequeueAllAsync(CancellationToken cancellationToken)
        => _queue.Reader.ReadAllAsync(cancellationToken);
}

