namespace Backgammon.WebAPI.DTOs.Auth;

public class RegisterRequestDto
{
    public required string UserName { get; init; }
    public required string Password { get; init; }
}