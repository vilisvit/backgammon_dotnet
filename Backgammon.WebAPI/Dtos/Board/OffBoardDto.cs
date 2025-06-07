namespace Backgammon.WebAPI.Dtos.Board;

public class OffBoardDto
{
    public required int WhiteCheckersCount { get; init; }
    public required int BlackCheckersCount { get; init; }
    public required bool IsPossibleMoveForBlackPlayer { get; init; }
    public required bool IsPossibleMoveForWhitePlayer { get; init; }
}