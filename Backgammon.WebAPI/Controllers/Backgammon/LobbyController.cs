using System.Security.Claims;
using Backgammon.GameCore.Lobby;
using Backgammon.Infrastructure.Repository;
using Backgammon.WebAPI.DTOs.Lobby;
using Microsoft.AspNetCore.Mvc;

namespace Backgammon.WebAPI.Controllers.Backgammon;

[ApiController]
[Route("api")]
public class LobbyController(LobbyManager lobbyManager, UserRepository userRepository) : ControllerBase
{
    [HttpPost("lobby")]
    public IActionResult CreateLobby()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized("Invalid or missing user ID in token.");
        }
        
        if (userRepository.ExistsById(userId) == false)
        {
            return Unauthorized("User not found.");
        }

        var username = userRepository.FindById(userId).UserName!;
        
        var session = lobbyManager.CreateLobby(username);
        
        SendLobbyUpdate(session);
        
        return Ok(BuildLobbyDto(session));
    }

    [HttpPost("lobby/join/{sessionId}")]
    public IActionResult JoinLobby(string sessionId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized("Invalid or missing user ID in token.");
        }
        
        if (userRepository.ExistsById(userId) == false)
        {
            return Unauthorized("User not found.");
        }

        var username = userRepository.FindById(userId).UserName!;

        try
        {
            var session = lobbyManager.JoinLobby(sessionId, username);
            SendLobbyUpdate(session);
            return Ok(BuildLobbyDto(session));
        }
        catch (LobbyException e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpPost("lobby/start/{sessionId}")]
    public IActionResult StartGame(string sessionId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized("Invalid or missing user ID in token.");
        }
        
        if (userRepository.ExistsById(userId) == false)
        {
            return Unauthorized("User not found.");
        }
        
        var username = userRepository.FindById(userId).UserName!;
        try
        {
            var lobby = lobbyManager.GetLobby(sessionId);
            if (!lobby.HasPlayer(username))
            {
                return Forbid("You are not a player in this lobby.");
            }
            if (!lobby.IsReadyToStart())
            {
                return BadRequest("Lobby is not ready to start.");
            }
            lobby.StartGame();
            SendLobbyUpdate(lobby);
            
            throw new NotImplementedException("Should send websocket message here.");
            
            return Ok(BuildLobbyDto(lobby));
        } catch (LobbyException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("lobby/leave/{sessionId}")]
    public IActionResult LeaveLobby(string sessionId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized("Invalid or missing user ID in token.");
        }
        
        if (userRepository.ExistsById(userId) == false)
        {
            return Unauthorized("User not found.");
        }
        
        var username = userRepository.FindById(userId).UserName!;
        
        try
        {
            var session = lobbyManager.GetLobby(sessionId);
            session.RemovePlayer(username);
            
            if (session.Players.Count == 0)
            {
                lobbyManager.RemoveLobby(sessionId);
                return Ok("Lobby removed.");
            }
            if (session.IsGameStarted())
            {
                session.CancelGame();
                throw new NotImplementedException("Should send websocket message here.");
            }
            SendLobbyUpdate(session);
            return Ok("Left lobby successfully.");
        }
        catch (LobbyException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("lobby/{sessionId}")]
    public IActionResult GetLobby(string sessionId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized("Invalid or missing user ID in token.");
        }
        
        if (userRepository.ExistsById(userId) == false)
        {
            return Unauthorized("User not found.");
        }
        
        var username = userRepository.FindById(userId).UserName!;
        try
        {
            var session = lobbyManager.GetLobby(sessionId);
            if (!session.HasPlayer(username))
            {
                return Forbid("You are not a player in this lobby.");
            }
            return Ok(BuildLobbyDto(session));
        }
        catch (LobbyException e)
        {
            return BadRequest(e.Message);
        }
    }
    
    private static void SendLobbyUpdate(GameSession session)
    {
        throw new NotImplementedException();
    }

    private static LobbyDto BuildLobbyDto(GameSession session)
    {
        return new LobbyDto
        {
            SessionId = session.SessionId,
            Players = session.Players.Select(p => p.Name).ToList(),
            ReadyToStart = session.IsReadyToStart(),
            Started = session.IsGameStarted()
        };
    }
    
}