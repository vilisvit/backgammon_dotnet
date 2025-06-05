namespace Backgammon.WebAPI.DTOs.Score;

public class ScoreResponseDto
{
    public int Points { get; init; }
    public DateTime PlayedOn { get; init; }
    public string Game { get; init; } = null!;

    // public int UserId { get; set; } // TODO: Uncomment after frontend update
    public string Player { get; init; } = null!;
}