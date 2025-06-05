using AutoMapper;
using Backgammon.Infrastructure.Repository;
using Backgammon.WebAPI.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Backgammon.WebAPI.Controllers;

[ApiController]
[Route("api/score")]
public class ScoreController(ScoreRepository scoreRepository, IMapper mapper) : ControllerBase
{
    [HttpGet("{game}")]
    public ActionResult<List<ScoreResponseDto>> GetTopScores(string game)
    {
        var scores = scoreRepository.GetTopScores(game);
        return Ok(scores.Select(mapper.Map<ScoreResponseDto>));
    }
}