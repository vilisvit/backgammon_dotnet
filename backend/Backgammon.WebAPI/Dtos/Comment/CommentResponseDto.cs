namespace Backgammon.WebAPI.Dtos.Comment;

public class CommentResponseDto
{
    public required string Comment { get; init; } // TODO: Rename to Content after frontend update
    public required DateTime CommentedOn { get; init; }
    public required string Game { get; init; }

    // public int UserId { get; set; } // TODO: Uncomment after frontend update
    public required string Player { get; init; }
}
