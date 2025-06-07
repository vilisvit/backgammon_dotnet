using Backgammon.WebAPI.Dtos.Board;

namespace Backgammon.WebAPI.Dtos.Game;

public class BoardMessage
{
    public required string Status { get; init; }
    public required string Message { get; init; }
    public BoardDto? Board { get; init; } = null;
}