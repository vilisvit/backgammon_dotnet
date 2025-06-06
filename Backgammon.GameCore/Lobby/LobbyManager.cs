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
    
    public GameSession JoinLobby(string sessionId, string username)
    {
        if (!_lobbies.TryGetValue(sessionId, out var session))
        {
            throw new LobbyException($"Lobby with session ID {sessionId} does not exist.");
        }

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
    
    public GameSession GetLobby(string sessionId)
    {
        _lobbies.TryGetValue(sessionId, out var session);
        if (session == null)
        {
            throw new LobbyException($"Lobby with session ID {sessionId} does not exist.");
        }
        return session;
    }
    
    public void RemoveLobby(string sessionId) 
    {
        if (!_lobbies.Remove(sessionId))
        {
            throw new LobbyException($"Lobby with session ID {sessionId} does not exist.");
        }
    }
    
    public GameSession FindSessionByPlayer(string username)
    {
        var lobby = _lobbies.Values.FirstOrDefault(session => session.Players.Any(p => p.Name == username));
        if (lobby == null)
        {
            throw new LobbyException($"No lobby found for player {username}.");
        }
        return lobby;
    }
}