using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backgammon.Core.Entities;

public class Rating
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Game { get; set; } = null!;

    [Required]
    [Range(1, 5)]
    public int Value { get; set; }

    [Required]
    public DateTime RatedOn { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;
}