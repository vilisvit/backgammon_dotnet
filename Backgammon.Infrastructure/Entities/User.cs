using System.ComponentModel.DataAnnotations;

namespace Backgammon.Infrastructure.Entities;

public class User
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = null!;

    [Required]
    public string PasswordHash { get; set; } = null!;

    // Optional metadata
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
}