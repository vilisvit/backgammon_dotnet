using Backgammon.GameCore.Game;

namespace Backgammon.GameCore.Lobby;

public class GameSession
{
    public string SessionId { get; }
    public List<Player> Players { get; } = [];
    public Board? Board { get; private set; }

    public GameSession(string sessionId, Player player1)
    {
        SessionId = sessionId;
        Players.Add(player1);
    }

    public void AddPlayer(Player player)
    {
        if (Players.Contains(player))
        {
            throw new GameSessionException(
                $"Player {player.Name} is already in the session {SessionId}.");
        }
        
        if (Players.Count >= 2)
        {
            throw new GameSessionException(
                $"Cannot add player {player.Name} to session {SessionId}, maximum players reached.");
        }
        
        Players.Add(player);
    }
    
    public bool IsReadyToStart()
    {
        return Players.Count == 2;
    }
    
    public void StartGame()
    {
        if (IsReadyToStart())
        {
            Console.WriteLine($"Starting game in session {SessionId} with players: {string.Join(", ", GetPlayerNames())}");
            Board = new Board(Players[0], Players[1]);
        }
        else
        {
            throw new GameSessionException(
                $"Cannot start game in session {SessionId}, not enough players.");
        }
    }
    
    public void RemovePlayer(Guid userId)
    {
        // print all players for debugging
        Console.WriteLine($"Removing player with ID {userId} from session {SessionId}. Current players:");
        foreach (var player in Players)
        {
            Console.WriteLine($"- {player.Name} (ID: {player.UserId})");
        }
        
        if (!Players.Any(p => p.UserId.Equals(userId)))
        {
            throw new GameSessionException(
                $"User with ID {userId} is not in the session {SessionId}.");
        }
        
        Players.RemoveAll(p => p.UserId.Equals(userId));
        
        Board = null;
    }
    
    public bool HasPlayer(Guid userId)
    {
        return Players.Any(p => p.UserId.Equals(userId));
    }
    
    public bool IsEmpty => Players.Count == 0;
    
    public bool IsGameStarted => Board != null;
    
    public List<string> GetPlayerNames() 
    {
        return Players.Select(p => p.Name).ToList();
    }
    
    public void CancelGame()
    {
        if (Board == null)
        {
            throw new GameSessionException(
                $"Cannot cancel game in session {SessionId}, game has not started.");
        }
        
        Board.ForceFinishGame();
        Board = null;
    }
    
    public void FinishGame()
    {
        Board = null;
    }
}