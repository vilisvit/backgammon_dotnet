namespace Backgammon.WebAPI.Dtos.Auth;

public class AuthResponseDto(string token)
{
    public string AccessToken { get; init; } = token;
    public string TokenType { get; init; } = "Bearer ";
}