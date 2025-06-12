namespace Backgammon.WebAPI.Dtos.Board;

public class PlayerDto
{
    public required string Username { get; init; }
    public required string Color { get; init; }
    public required int CurrentScore { get; init; }
}