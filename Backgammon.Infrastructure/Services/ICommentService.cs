using Backgammon.Infrastructure.Entities;

namespace Backgammon.Infrastructure.Services;

public interface ICommentService
{
    void AddComment(Comment comment);
    List<Comment> GetComments(string game);
    void Reset();
}