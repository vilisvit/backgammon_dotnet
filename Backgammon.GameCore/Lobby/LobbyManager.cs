using Backgammon.GameCore.Game;

namespace Backgammon.GameCore.Lobby;

public class LobbyManager
{
    private readonly Dictionary<string, GameSession> _lobbies = new();

    public GameSession CreateLobby(string username)
    {
        var sessionId = Guid.NewGuid().ToString();
        var player = new Player(ColorExtensions.GetRandom(), username);
        var session = new GameSession(sessionId, player);
        _lobbies[sessionId] = session;
        return session;
    }
    
    public GameSession? JoinLobby(string sessionId, string username)
    {
        if (!_lobbies.TryGetValue(sessionId, out var session)) return null;

        switch (session.Players?.Count)
        {
            case 1:
            {
                var newPlayerColor = session.Players[0].Color == Color.White ? Color.Black : Color.White;
                session.AddPlayer(new Player(newPlayerColor, username));
                break;
            }
            case 0:
            {
                var newPlayerColor = ColorExtensions.GetRandom();
                session.AddPlayer(new Player(newPlayerColor, username));
                break;
            }
            default:
                throw new LobbyException(
                    $"Cannot join session {sessionId}, it is already full.");
        }
        return session;
    }

    public void LeaveLobby(string sessionId, string username)
    {
        if (!_lobbies.TryGetValue(sessionId, out var session))
        {
            throw new LobbyException($"Lobby with session ID {sessionId} does not exist.");
        }

        var player = session.Players.FirstOrDefault(p => p.Name == username);
        if (player == null)
        {
            throw new LobbyException($"Player {username} is not in the lobby {sessionId}.");
        }

        session.RemovePlayer(player);
        
        if (session.Players.Count == 0)
        {
            _lobbies.Remove(sessionId);
        }
    }
    
    public GameSession? GetLobby(string sessionId)
    {
        _lobbies.TryGetValue(sessionId, out var session);
        return session;
    }
    
    public void RemoveLobby(string sessionId) 
    {
        if (!_lobbies.Remove(sessionId))
        {
            throw new LobbyException($"Lobby with session ID {sessionId} does not exist.");
        }
    }
    
    public GameSession? FindSessionByPlayer(string username)
    {
        return _lobbies.Values.FirstOrDefault(session => session.Players.Any(p => p.Name == username));
    }
}