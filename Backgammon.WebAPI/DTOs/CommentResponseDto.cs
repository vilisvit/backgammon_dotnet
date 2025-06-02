namespace Backgammon.WebAPI.DTOs;

public class CommentResponseDto
{
    public string Comment { get; set; } // TODO: Rename to Content after frontend update
    public DateTime CommentedOn { get; set; }
    public string Game { get; set; }

    // public int UserId { get; set; } // TODO: Uncomment after frontend update
    public string Player { get; set; }
}
