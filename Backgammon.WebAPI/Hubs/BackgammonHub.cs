using System.Security.Claims;
using AutoMapper;
using Backgammon.Core.Entities;
using Backgammon.GameCore.Game;
using Backgammon.GameCore.Lobby;
using Backgammon.Infrastructure.Repository;
using Backgammon.WebAPI.Dtos.Board;
using Backgammon.WebAPI.Dtos.Game;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Backgammon.WebAPI.Hubs;

[Authorize]
public class BackgammonHub(
    LobbyManager lobbyManager,
    ScoreRepository scoreRepository,
    UserRepository userRepository,
    IMapper mapper)
    : Hub
{
    // ======================= CONNECTION EVENTS =======================

    public override async Task OnConnectedAsync()
    {
        var sessionId = Context.GetHttpContext()?.Request.Query["sessionId"].ToString();

        if (string.IsNullOrEmpty(sessionId))
        {
            await SendPrivateError("Session ID is required.");
            return;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetUserIdOrNull();
        if (userId is null || !userRepository.ExistsById(userId.Value))
        {
            await base.OnDisconnectedAsync(exception);
            return;
        }

        var user = userRepository.FindById(userId.Value);

        try
        {
            var session = lobbyManager.FindSessionByPlayer(user.Id);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, session.SessionId);
            lobbyManager.RemoveLobby(session.SessionId);

            await Clients.Group(session.SessionId).SendAsync("GameCanceled", new BoardMessage
            {
                Status = "canceled",
                Message = $"Player {user.UserName} left the game. Session {session.SessionId} canceled.",
                Board = null
            });
        }
        catch
        {
            // Session not found – ignore
        }

        await base.OnDisconnectedAsync(exception);
    }

    // ======================= GAME EVENTS =======================

    public async Task RollDice(string sessionId)
    {
        if (!TryGetUser(out var user))
        {
            await SendPrivateError("Unauthorized user.");
            return;
        }

        var board = GetValidBoard(sessionId, user.Id, requireMoveState: false, out var error);
        if (board == null)
        {
            await SendPrivateError(error!);
            return;
        }

        board.RollDice();

        if (board.NoMovesAvailable)
        {
            board.ChangePlayer();
            board.NoMovesWereAvailable = true;
            await BroadcastBoard(sessionId, "No moves available. Player changed.", board);
        }
        else
        {
            await BroadcastBoard(sessionId, "Dice rolled.", board);
        }
    }

    public async Task OnClick(string sessionId, int pointId)
    {
        if (!TryGetUser(out var user))
        {
            await SendPrivateError("Unauthorized user.");
            return;
        }

        var board = GetValidBoard(sessionId, user.Id, requireMoveState: true, out var error);
        if (board == null)
        {
            await SendPrivateError(error!);
            return;
        }

        string response;

        if (board.SelectedPointNum == -1 && !board.Bar.HasCheckers(board.CurrentPlayer!.Color))
        {
            if (!board.SelectPoint(pointId))
            {
                await SendPrivateError("Invalid selection.");
                return;
            }

            response = $"Point {pointId} selected";
        }
        else
        {
            if (board.SelectedPointNum == pointId)
            {
                board.DeselectPoint();
                response = $"Point {pointId} deselected";
            }
            else
            {
                if (!board.Move(board.GetPoint(pointId)))
                {
                    await SendPrivateError("Invalid move.");
                    return;
                }

                response = $"Moved checker to point {pointId}";

                if (board.GameState != GameState.Roll && board.NoMovesAvailable)
                    board.ChangePlayer();
            }
        }

        await BroadcastBoard(sessionId, response, board);
    }

    public async Task OnOffBoardClick(string sessionId)
    {
        if (!TryGetUser(out var user))
        {
            await SendPrivateError("Unauthorized user.");
            return;
        }

        var board = GetValidBoard(sessionId, user.Id, requireMoveState: true, out var error);
        if (board == null)
        {
            await SendPrivateError(error!);
            return;
        }

        if (!board.Move(board.OffBoard))
        {
            await SendPrivateError("Invalid move.");
            return;
        }

        if (board.GameState != GameState.Roll && board.NoMovesAvailable)
            board.ChangePlayer();

        await BroadcastBoard(sessionId, "Moved checker off board", board);
    }

    // ======================= HELPERS =======================

    private async Task BroadcastBoard(string sessionId, string message, Board board)
    {
        await Clients.Group(sessionId).SendAsync("GameUpdated", new BoardMessage
        {
            Status = "success",
            Message = message,
            Board = mapper.Map<BoardDto>(board)
        });

        if (board.Winner != null)
        {
            HandleWin(sessionId, board);
        }
    }

    private void HandleWin(string sessionId, Board board)
    {
        if (board.Winner == null)
        {
            throw new InvalidOperationException("Cannot handle win: no winner defined.");
        }
        
        var loser = board.Winner.Name == board.Player1.Name ? board.Player2 : board.Player1;

        scoreRepository.AddScore(new Score
        {
            Game = "backgammon",
            UserId = board.Winner.UserId,
            Points = board.GetScore(loser),
            PlayedOn = DateTime.Now
        });

        lobbyManager.GetLobby(sessionId).FinishGame();
    }

    private async Task SendPrivateError(string message)
    {
        await SendPrivateError(new BoardMessage
        {
            Status = "error",
            Message = message
        });
    }

    private async Task SendPrivateError(BoardMessage error)
    {
        await Clients.Caller.SendAsync("Error", error);
    }

    private Board? GetValidBoard(string sessionId, Guid userId, bool requireMoveState, out BoardMessage? error)
    {
        error = ValidateBoardAccess(sessionId, userId, requireMoveState);
        if (error != null)
            return null;

        return lobbyManager.GetLobby(sessionId).Board;
    }

    private BoardMessage? ValidateBoardAccess(string sessionId, Guid userId, bool requireMoveState)
    {
        try
        {
            var session = lobbyManager.GetLobby(sessionId);

            if (!session.HasPlayer(userId))
                return new BoardMessage { Status = "error", Message = "You are not part of this game session." };

            var board = session.Board;
            if (board == null)
                return new BoardMessage { Status = "error", Message = "No game in progress." };

            var currentPlayer = board.CurrentPlayer;
            if (board.GameState != GameState.ChoosingPlayer)
            {
                if (currentPlayer == null)
                    return new BoardMessage { Status = "error", Message = "Internal error: current player is null." };

                if (currentPlayer.UserId != userId)
                    return new BoardMessage { Status = "invalid_action", Message = "It's not your turn." };
            }

            if (requireMoveState && board.GameState != GameState.Move)
                return new BoardMessage { Status = "invalid_action", Message = "Game is not in the correct state for this action." };

            return null;
        }
        catch (LobbyException e)
        {
            return new BoardMessage { Status = "error", Message = e.Message };
        }
    }

    private bool TryGetUser(out User? user)
    {
        user = null;
        var userId = GetUserIdOrNull();
        if (userId == null)
            return false;

        if (!userRepository.ExistsById(userId.Value))
            return false;

        user = userRepository.FindById(userId.Value);
        return true;
    }

    private Guid? GetUserIdOrNull()
    {
        var value = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(value, out var userId) ? userId : null;
    }
}
