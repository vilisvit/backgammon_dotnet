using Backgammon.GameCore.Game;
using Backgammon.Infrastructure.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Backgammon.GameCore.Lobby;

public class LobbyManager(IServiceProvider serviceProvider)
{
    private readonly Dictionary<string, GameSession> _lobbies = new();

    public GameSession CreateLobby(Guid userId)
    {
        using var scope = serviceProvider.CreateScope();
        var userRepository = scope.ServiceProvider.GetRequiredService<UserRepository>();

        if (!userRepository.ExistsById(userId)) 
        {
            throw new LobbyException($"User with ID {userId} does not exist.");
        }
        
        var username = userRepository.FindById(userId).UserName;
        
        var sessionId = Guid.NewGuid().ToString();
        var player = new Player(ColorExtensions.GetRandom(), username!, userId);
        var session = new GameSession(sessionId, player);
        _lobbies.Add(sessionId, session);
        Console.WriteLine($"Added new lobby with ID {sessionId} for player {username} (ID: {userId}).");
        Console.WriteLine($"HashCode: {this.GetHashCode()}");
        return session;
    }
    
    public GameSession JoinLobby(string sessionId, Guid userId)
    {
        using var scope = serviceProvider.CreateScope();
        var userRepository = scope.ServiceProvider.GetRequiredService<UserRepository>();
        
        // print all lobbies for debugging
        Console.WriteLine("Current Lobbies:");
        Console.WriteLine($"HashCode: {this.GetHashCode()}");
        
        foreach (var lobby in _lobbies)
        {
            Console.WriteLine($"Lobby ID: {lobby.Key}, Players: {lobby.Value.Players.Count}");
        }
        
        if (!_lobbies.TryGetValue(sessionId, out var session))
        {
            throw new LobbyException($"Lobby with session ID {sessionId} does not exist.");
        }
        
        if (!userRepository.ExistsById(userId)) 
        {
            throw new LobbyException($"User with ID {userId} does not exist.");
        }
        
        var username = userRepository.FindById(userId).UserName;

        switch (session.Players?.Count)
        {
            case 1:
            {
                var newPlayerColor = session.Players[0].Color == Color.White ? Color.Black : Color.White;
                session.AddPlayer(new Player(newPlayerColor, username!, userId));
                break;
            }
            case 0:
            {
                var newPlayerColor = ColorExtensions.GetRandom();
                session.AddPlayer(new Player(newPlayerColor, username!, userId));
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
        Console.WriteLine("Leaving lobby with session ID: " + sessionId);
        Console.WriteLine($"HashCode: {this.GetHashCode()}");
        if (!_lobbies.Remove(sessionId))
        {
            throw new LobbyException($"Lobby with session ID {sessionId} does not exist.");
        }
    }
    
    public GameSession FindSessionByPlayer(Guid userId)
    {
        var lobby = _lobbies.Values.FirstOrDefault(session => session.Players.Any(p => p.UserId == userId));
        if (lobby == null)
        {
            throw new LobbyException($"No lobby found for user ID {userId}.");
        }
        return lobby;
    }
}