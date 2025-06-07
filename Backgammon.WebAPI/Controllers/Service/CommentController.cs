using System.Security.Claims;
using AutoMapper;
using Backgammon.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Backgammon.Infrastructure.Repository;
using Backgammon.WebAPI.Dtos.Comment;

namespace Backgammon.WebAPI.Controllers.Service;

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

        try
        {
            commentRepository.AddComment(entity);
        }
        catch (Exception e)
        {
            return BadRequest($"Problem adding comment: {e.Message}");
        }

        return StatusCode(201);
    }

    [HttpGet("{game}")]
    public ActionResult<List<CommentResponseDto>> GetComments(string game)
    {
        try
        {
            var comments = commentRepository.GetComments(game);
            return Ok(comments.Select(mapper.Map<CommentResponseDto>));
        } catch (Exception e)
        {
            return BadRequest($"Problem retrieving comments: {e.Message}");
        }
    }
}