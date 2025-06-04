using System.Security.Claims;
using AutoMapper;
using Backgammon.Core.Entities;
using Backgammon.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Backgammon.Infrastructure.Repository;
using Backgammon.WebAPI.DTOs;

namespace Backgammon.WebAPI.Controllers;

[ApiController]
[Route("api/comment")]
public class CommentController(CommentRepository commentRepository, UserRepository userRepository, IMapper mapper)
    : ControllerBase
{
    [HttpPost("{game}")]
    public IActionResult AddComment(string game, [FromBody] CommentRequestDto requestDto)
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

        var entity = new Comment
        {
            Game = game,
            UserId = userId,
            Content = requestDto.Comment,
            CommentedOn = DateTime.UtcNow
        };

        commentRepository.AddComment(entity);
        return StatusCode(201);
    }

    [HttpGet("{game}")]
    public ActionResult<List<CommentResponseDto>> GetComments(string game)
    {
        var comments = commentRepository.GetComments(game);

        return Ok(comments.Select(comment => mapper.Map<CommentResponseDto>(comment)));
    }
}