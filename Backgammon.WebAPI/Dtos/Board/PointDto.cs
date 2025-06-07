namespace Backgammon.WebAPI.Dtos.Board;

public class PointDto
{
    public required int Id { get; init; }
    public required int CheckersCount { get; init; }
    public required string? CheckersColor { get; init; }
    public required bool IsSelected { get; init; }
    public required bool IsPossibleMove { get; init; }
    public required bool IsPossibleStartPoint { get; init; }
}