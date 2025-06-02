using AutoMapper;
using Backgammon.Core.Entities;
using Backgammon.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Backgammon.Infrastructure.Repository;
using Backgammon.WebAPI.DTOs;

namespace Backgammon.WebAPI.Controllers.Services;

[ApiController]
[Route("api/comment")]
public class CommentController : ControllerBase
{
    private readonly CommentRepository _commentRepository;
    private readonly IUserRepository _userRepository; // Assuming you have a user repository to manage users
    private readonly IMapper _mapper;

    // 👇 @Autowired equivalent
    public CommentController(CommentRepository commentRepository, UserRepository userRepository, IMapper mapper)
    {
        _commentRepository = commentRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    [HttpPost("{game}")]
    public IActionResult AddComment(string game, [FromBody] CommentRequest request)
    {
        var userId = Guid.NewGuid(); // somehow get the user ID from the request context
        
        // Add user with ID to the user db
        // This is a placeholder; in a real application, you would retrieve the user ID from the authenticated user's context.
        
        _userRepository.AddUser(new User
        {
            Id = userId,
            Username = "Anonymous",
            PasswordHash = "placeholder"
        });

        var entity = new Comment
        {
            Game = game,
            UserId = userId,
            Content = request.Comment,
            CommentedOn = DateTime.UtcNow
        };

        _commentRepository.AddComment(entity);
        return StatusCode(201);
    }

    [HttpGet("{game}")]
    public ActionResult<List<CommentResponseDto>> GetComments(string game)
    {
        var comments = _commentRepository.GetComments(game);

        return Ok(comments.Select(comment => _mapper.Map<CommentResponseDto>(comment)));
    }
}