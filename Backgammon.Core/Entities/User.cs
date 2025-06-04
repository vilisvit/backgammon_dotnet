namespace Backgammon.Core.Entities;

using Microsoft.AspNetCore.Identity;

public class User : IdentityUser<Guid>
{
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
}