using System.Security.Claims;
using Backgammon.GameCore.Lobby;
using Backgammon.Infrastructure.Repository;
using Backgammon.WebAPI.DTOs.Lobby;
using Microsoft.AspNetCore.Mvc;

namespace Backgammon.WebAPI.Controllers.Backgammon;

[ApiController]
[Route("api/lobby")]
public class LobbyController(LobbyManager lobbyManager, UserRepository userRepository) : ControllerBase
{
    [HttpPost]
    public IActionResult CreateLobby()
    {
        var usernameResult = GetAuthenticatedUsername();
        if (usernameResult is not OkObjectResult okResult)
            return usernameResult;

        var username = okResult.Value!.ToString()!;
        var session = lobbyManager.CreateLobby(username);
        SendLobbyUpdate(session);
        return Ok(BuildLobbyDto(session));
    }

    [HttpPost("join/{sessionId}")]
    public IActionResult JoinLobby(string sessionId)
    {
        var usernameResult = GetAuthenticatedUsername();
        if (usernameResult is not OkObjectResult okResult)
            return usernameResult;

        var username = okResult.Value!.ToString()!;
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

    [HttpPost("start/{sessionId}")]
    public IActionResult StartGame(string sessionId)
    {
        var usernameResult = GetAuthenticatedUsername();
        if (usernameResult is not OkObjectResult okResult)
            return usernameResult;

        var username = okResult.Value!.ToString()!;
        try
        {
            var lobby = lobbyManager.GetLobby(sessionId);
            if (!lobby.HasPlayer(username))
                return Forbid("You are not a player in this lobby.");

            if (!lobby.IsReadyToStart())
                return BadRequest("Lobby is not ready to start.");

            lobby.StartGame();
            SendLobbyUpdate(lobby);

            throw new NotImplementedException("Should send websocket message here.");
            // return Ok(BuildLobbyDto(lobby));
        }
        catch (LobbyException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("leave/{sessionId}")]
    public IActionResult LeaveLobby(string sessionId)
    {
        var usernameResult = GetAuthenticatedUsername();
        if (usernameResult is not OkObjectResult okResult)
            return usernameResult;

        var username = okResult.Value!.ToString()!;
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

    [HttpGet("{sessionId}")]
    public IActionResult GetLobby(string sessionId)
    {
        var usernameResult = GetAuthenticatedUsername();
        if (usernameResult is not OkObjectResult okResult)
            return usernameResult;

        var username = okResult.Value!.ToString()!;
        try
        {
            var session = lobbyManager.GetLobby(sessionId);
            if (!session.HasPlayer(username))
                return Forbid("You are not a player in this lobby.");
            return Ok(BuildLobbyDto(session));
        }
        catch (LobbyException e)
        {
            return BadRequest(e.Message);
        }
    }

    // =================== HELPERS =====================

    private IActionResult GetAuthenticatedUsername()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized("Invalid or missing user ID in token.");

        try
        {
            var user = userRepository.FindById(userId);
            return Ok(user.UserName!);
        }
        catch (KeyNotFoundException e)
        {
            return Unauthorized("User not found.");
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