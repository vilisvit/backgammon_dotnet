namespace Backgammon.WebAPI.DTOs.Board;

public class BarDto
{
    public required int WhiteCheckersCount { get; init; }
    public required int BlackCheckersCount { get; init; }
    public required bool SelectedForBlackPlayer { get; init; }
    public required bool SelectedForWhitePlayer { get; init; }
}