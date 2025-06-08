namespace Backgammon.WebAPI.Dtos.Board;

public class PointDto
{
    public required int Id { get; init; }
    public required int CheckersCount { get; init; }
    public required string? CheckersColor { get; init; }
    public required bool Selected { get; init; }
    public required bool PossibleMove { get; init; }
    public required bool PossibleStartPoint { get; init; }
}