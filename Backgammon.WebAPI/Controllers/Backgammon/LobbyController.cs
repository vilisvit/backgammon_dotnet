using System.Security.Claims;
using AutoMapper;
using Backgammon.Core.Entities;
using Backgammon.GameCore.Lobby;
using Backgammon.Infrastructure.Repository;
using Backgammon.WebAPI.Dtos.Board;
using Backgammon.WebAPI.Dtos.Lobby;
using Backgammon.WebAPI.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Backgammon.WebAPI.Controllers.Backgammon;

[ApiController]
[Route("api/lobby")]
public class LobbyController(
    LobbyManager lobbyManager,
    UserRepository userRepository,
    IHubContext<BackgammonHub> hubContext,
    IMapper mapper
) : ControllerBase
{
    [HttpPost]
    public IActionResult CreateLobby()
    {
        var userResult = GetAuthenticatedUser(out var user);
        if (userResult is not OkResult)
            return userResult;

        var session = lobbyManager.CreateLobby(user!.Id);
        SendLobbyUpdate(session);
        return Ok(BuildLobbyDto(session));
    }

    [HttpPost("join/{sessionId}")]
    public IActionResult JoinLobby(string sessionId)
    {
        var userResult = GetAuthenticatedUser(out var user);
        if (userResult is not OkResult)
            return userResult;

        try
        {
            var session = lobbyManager.JoinLobby(sessionId, user!.Id);
            SendLobbyUpdate(session);
            return Ok(BuildLobbyDto(session));
        }
        catch (LobbyException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("start/{sessionId}")]
    public async Task<IActionResult> StartGame(string sessionId)
    {
        var userResult = GetAuthenticatedUser(out var user);
        if (userResult is not OkResult)
            return userResult;

        try
        {
            var lobby = lobbyManager.GetLobby(sessionId);
            if (!lobby.HasPlayer(user!.Id))
                return Forbid("You are not a player in this lobby.");

            if (!lobby.IsReadyToStart())
                return BadRequest("Lobby is not ready to start.");

            lobby.StartGame();
            SendLobbyUpdate(lobby);

            await hubContext.Clients.Group(sessionId).SendAsync("GameUpdated", new
            {
                status = "success",
                message = "Game started.",
                board = mapper.Map<BoardDto>(lobby.Board),
            });

            return Ok();
        }
        catch (LobbyException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("leave/{sessionId}")]
    public async Task<IActionResult> LeaveLobby(string sessionId)
    {
        var userResult = GetAuthenticatedUser(out var user);
        if (userResult is not OkResult)
            return userResult;

        try
        {
            var session = lobbyManager.GetLobby(sessionId);
            session.RemovePlayer(user!.Id);

            if (session.Players.Count == 0)
            {
                lobbyManager.RemoveLobby(sessionId);
                return Ok("Lobby removed.");
            }

            if (session.IsGameStarted)
            {
                session.CancelGame();

                await hubContext.Clients.Group(sessionId).SendAsync("GameCanceled", new
                {
                    status = "err_disconnected",
                    message = "A player disconnected. Game has been canceled."
                });

                return Ok("Game canceled.");
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
        var userResult = GetAuthenticatedUser(out var user);
        if (userResult is not OkResult)
            return userResult;

        try
        {
            var session = lobbyManager.GetLobby(sessionId);
            if (!session.HasPlayer(user!.Id))
                return Forbid("You are not a player in this lobby.");
            return Ok(BuildLobbyDto(session));
        }
        catch (LobbyException e)
        {
            return BadRequest(e.Message);
        }
    }

    // =================== HELPERS =====================

    private IActionResult GetAuthenticatedUser(out User? user)
    {
        user = null;

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized("Invalid or missing user ID in token.");

        try
        {
            user = userRepository.FindById(userId);
            return Ok();
        }
        catch (KeyNotFoundException)
        {
            return Unauthorized("User not found.");
        }
    }

    private void SendLobbyUpdate(GameSession session)
    {
        hubContext.Clients.Group(session.SessionId).SendAsync("LobbyUpdated", BuildLobbyDto(session));
    }

    private static LobbyDto BuildLobbyDto(GameSession session)
    {
        return new LobbyDto
        {
            SessionId = session.SessionId,
            Players = session.Players.Select(p => p.Name).ToList(),
            ReadyToStart = session.IsReadyToStart(),
            Started = session.IsGameStarted
        };
    }
}