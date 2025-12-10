namespace SchedulePlanner.Domain.Interfaces;

public interface IRpcClient : IDisposable
{
    Task<string> CallAsync(string message, CancellationToken cancellationToken = default);
}
