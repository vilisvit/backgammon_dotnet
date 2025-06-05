namespace Backgammon.WebAPI.DTOs.Auth;

public class RegisterRequestDto
{
    public string UserName { get; init; } = null!;
    public string Password { get; init; } = null!;
}