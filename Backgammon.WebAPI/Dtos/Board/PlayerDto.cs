namespace Backgammon.WebAPI.Dtos.Board;

public class PlayerDto
{
    public required string UserName { get; init; }
    public required string Color { get; init; }
    public required int CurrentScore { get; init; }
}