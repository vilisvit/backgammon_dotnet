using Backgammon.WebAPI.DTOs.Board;

namespace Backgammon.WebAPI.DTOs.Game;

public class BoardMessageDto
{
    public required string Status { get; init; }
    public required string Message { get; init; }
    public BoardDto? Board { get; init; } = null;
}