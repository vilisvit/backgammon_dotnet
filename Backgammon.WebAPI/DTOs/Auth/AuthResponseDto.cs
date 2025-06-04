namespace Backgammon.WebAPI.DTOs.Auth;

public class AuthResponseDto
{
    public string AccessToken { get; set; } = null!;
    public string TokenType { get; set; } = "Bearer ";
    
    public AuthResponseDto(string token) 
    {
        AccessToken = token;
    }
}