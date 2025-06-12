namespace Backgammon.WebAPI.Dtos.Board;

public class OffBoardDto
{
    public required int WhiteCheckersCount { get; init; }
    public required int BlackCheckersCount { get; init; }
    public required bool PossibleMoveForBlackPlayer { get; init; }
    public required bool PossibleMoveForWhitePlayer { get; init; }
}