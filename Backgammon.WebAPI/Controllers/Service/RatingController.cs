using System.Security.Claims;
using AutoMapper;
using Backgammon.Core.Entities;
using Backgammon.Infrastructure.Repository;
using Backgammon.WebAPI.DTOs;
using Backgammon.WebAPI.DTOs.Rating;
using Microsoft.AspNetCore.Mvc;

namespace Backgammon.WebAPI.Controllers.Service;

[ApiController]
[Route("api/rating")]
public class RatingController(RatingRepository ratingRepository, UserRepository userRepository, IMapper mapper) : ControllerBase
{
    [HttpGet("{game}")]
    public ActionResult<int> GetAverageRating(string game)
    {
        try
        {
            return ratingRepository.GetAverageRating(game);
        } catch (Exception e)
        {
            return BadRequest($"Problem retrieving average rating: {e.Message}");
        }
    }
    
    [HttpGet("{game}/{player}")]
    public ActionResult<int> GetPlayerRating(string game, string player)
    {
        if (!userRepository.ExistsByUserName(player))
        {
            return NotFound("User with such username not found.");
        }
        
        var user = userRepository.FindByUserName(player);

        try
        {
            var rating = ratingRepository.GetRating(game, user.Id);
            return Ok(rating);
        } catch (Exception e)
        {
            return BadRequest($"Problem retrieving rating: {e.Message}");
        }
    }

    [HttpPut("{game}")]
    public ActionResult SetRating(string game, [FromBody] RatingRequestDto request)
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
        
        var entity = new Rating
        {
            Game = game,
            UserId = userId,
            Value = request.Rating,
            RatedOn = DateTime.UtcNow
        };

        try
        {
            ratingRepository.SetRating(entity);
        } catch (Exception e)
        {
            return BadRequest($"Problem setting rating: {e.Message}");
        }

        return StatusCode(200, "Rating set successfully.");
    }
}