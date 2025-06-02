using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backgammon.Core.Entities;

public class Comment
{
    [Key]
    public int Id { get; set; }
    
    // TODO: make Game entity and use foreign key
    
    [Required]
    [MaxLength(50)]
    public string Game { get; set; } = null!;
    
    [Required]
    [MaxLength(1000)]
    [MinLength(1)]
    public string Content { get; set; } = null!;
    
    [Required]
    public DateTime CommentedOn { get; set; }

    [Required]
    public Guid UserId { get; set; }
    
    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;
}
