using Backgammon.GameCore.Game;

namespace Backgammon.GameCore.Lobby;

public class GameSession
{
    public string SessionId { get; init; } = null!;
    public List<Player> Players { get; init; } = [];
    private Board? _board;

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
            _board = new Board(Players[0], Players[1]);
        }
        else
        {
            throw new GameSessionException(
                $"Cannot start game in session {SessionId}, not enough players.");
        }
    }
    
    public void RemovePlayer(string player)
    {
        if (!Players.Any(p => p.Name.Equals(player)))
        {
            throw new GameSessionException(
                $"Player {player} is not in the session {SessionId}.");
        }
        
        Players.RemoveAll(p => p.Name.Equals(player));
        
        _board = null;
    }
    
    public bool HasPlayer(string username)
    {
        return Players.Any(p => p.Name.Equals(username));
    }
    
    public bool IsEmpty() 
    {
        return Players.Count == 0;
    }
    
    public bool IsGameStarted()
    {
        return _board != null;
    }
    
    public List<string> GetPlayerNames() 
    {
        return Players.Select(p => p.Name).ToList();
    }
    
    public void CancelGame()
    {
        if (_board == null)
        {
            throw new GameSessionException(
                $"Cannot cancel game in session {SessionId}, game has not started.");
        }
        
        _board.ForceFinishGame();
        _board = null;
    }
    
    public void EndGame()
    {
        _board = null;
    }
}