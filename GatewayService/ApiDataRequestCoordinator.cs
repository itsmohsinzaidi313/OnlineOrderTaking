using System.Collections.Concurrent;

namespace GatewayService;

public class ApiDataRequestCoordinator
{
    private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> _pending = new();

    public Task<string> WaitForResponseAsync(string correlationId, CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
        if (!_pending.TryAdd(correlationId, tcs))
        {
            throw new InvalidOperationException($"A pending request already exists for correlation id '{correlationId}'.");
        }

        cancellationToken.Register(() =>
        {
            if (_pending.TryRemove(correlationId, out var source))
            {
                source.TrySetCanceled(cancellationToken);
            }
        }, useSynchronizationContext: false);

        return tcs.Task;
    }

    public bool TryResolve(string correlationId, string payload)
    {
        if (_pending.TryRemove(correlationId, out var tcs))
        {
            tcs.TrySetResult(payload);
            return true;
        }

        return false;
    }
}
