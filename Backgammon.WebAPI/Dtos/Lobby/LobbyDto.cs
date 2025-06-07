namespace Backgammon.WebAPI.Dtos.Lobby;

public class LobbyDto
{
    public required string SessionId { get; init; }
    public required List<string> Players { get; init; }
    public required bool ReadyToStart { get; init; }
    public required bool Started { get; init; }
}