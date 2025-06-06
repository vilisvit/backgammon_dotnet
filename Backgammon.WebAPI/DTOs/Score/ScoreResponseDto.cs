namespace Backgammon.WebAPI.DTOs.Score;

public class ScoreResponseDto
{
    public required int Points { get; init; }
    public required DateTime PlayedOn { get; init; }
    public required string Game { get; init; }

    // public int UserId { get; set; } // TODO: Uncomment after frontend update
    public required string Player { get; init; }
}