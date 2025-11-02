using System.Collections.Concurrent;

namespace JwtTokenService.Services;

/// <summary>
/// Thread-safe registry for mapping connectionId &lt;-&gt; userId.
/// Ensures that if a user reconnects with a new connectionId, the mapping is updated.
/// </summary>
public class ConnectionRegistry
{
    // connectionId -> userId
    private readonly ConcurrentDictionary<string, string> _connToUser = new();
    // userId -> connectionId
    private readonly ConcurrentDictionary<string, string> _userToConn = new();

    public void Register(string connectionId, string userId)
    {
        // If this connectionId was previously mapped to another user, remove that reverse mapping
        if (_connToUser.TryGetValue(connectionId, out var previousUser))
        {
            if (previousUser != userId)
            {
                _userToConn.TryRemove(previousUser, out _);
            }
        }

        // If this user was previously mapped to another connectionId, remove that connection mapping
        if (_userToConn.TryGetValue(userId, out var previousConn))
        {
            if (previousConn != connectionId)
            {
                _connToUser.TryRemove(previousConn, out _);
            }
        }

        _connToUser[connectionId] = userId;
        _userToConn[userId] = connectionId;
    }

    public bool TryGetUser(string connectionId, out string userId)
        => _connToUser.TryGetValue(connectionId, out userId!);

    public bool TryGetConnection(string userId, out string connectionId)
        => _userToConn.TryGetValue(userId, out connectionId!);

    public bool RemoveByConnection(string connectionId)
    {
        if (_connToUser.TryRemove(connectionId, out var userId))
        {
            _userToConn.TryRemove(userId, out _);
            return true;
        }
        return false;
    }

    public bool RemoveByUser(string userId)
    {
        if (_userToConn.TryRemove(userId, out var conn))
        {
            _connToUser.TryRemove(conn, out _);
            return true;
        }
        return false;
    }
}
