namespace Backgammon.WebAPI.DTOs.Comment;

public class CommentResponseDto
{
    public string Comment { get; init; } = null!; // TODO: Rename to Content after frontend update
    public DateTime CommentedOn { get; init; }
    public string Game { get; init; } = null!;

    // public int UserId { get; set; } // TODO: Uncomment after frontend update
    public string Player { get; init; } = null!;
}
