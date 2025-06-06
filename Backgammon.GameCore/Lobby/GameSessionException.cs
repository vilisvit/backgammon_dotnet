namespace Backgammon.GameCore.Lobby;

public class GameSessionException : Exception
{
    public GameSessionException() { }

    public GameSessionException(string message) : base(message) { }

    public GameSessionException(string message, Exception innerException)
        : base(message, innerException) { }
}