namespace Backgammon.GameCore.Lobby;

public class LobbyException : Exception
{
    public LobbyException() { }
    public LobbyException(string message) : base(message) { }

    public LobbyException(string message, Exception innerException)
        : base(message, innerException) { }
}