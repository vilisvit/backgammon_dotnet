namespace Backgammon.WebAPI.Dtos.Board;

public class BoardDto
{
    public required PointDto[] Points { get; init; }
    public required BarDto Bar { get; init; }
    public required OffBoardDto OffBoard { get; init; }
    public required DiceDto Dice { get; init; }
    
    public required string GameState { get; init; }
    
    public required PlayerDto Player1 { get; init; }
    public required PlayerDto Player2 { get; init; }
    public required string? CurrentPlayerUsername { get; init; }
    public required string? WinnerUsername { get; init; }
    public required bool NoMovesWereAvailable { get; init; }
}