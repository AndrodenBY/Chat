
using Chat.Domain.ValueObjects;

namespace Chat.Domain.Contracts;

public interface IConnectionTracker
{
    Task SetConnection(UserConnection connection, CancellationToken cancellationToken);
    Task<UserConnection?> GetConnection(string connectionId, CancellationToken cancellationToken);
    Task RemoveConnection(string connectionId, CancellationToken cancellationToken);
}
