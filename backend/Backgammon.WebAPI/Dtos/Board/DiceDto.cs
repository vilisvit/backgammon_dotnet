namespace Backgammon.WebAPI.Dtos.Board;

public class DiceDto
{
    public required int FirstDiceValue { get; init; }
    public required int SecondDiceValue { get; init; }
    public required bool AreRolled { get; init; }
}