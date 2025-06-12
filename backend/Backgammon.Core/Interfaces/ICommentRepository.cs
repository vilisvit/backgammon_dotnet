using Backgammon.Core.Entities;

namespace Backgammon.Core.Interfaces;

public interface ICommentRepository
{
    void AddComment(Comment comment);
    List<Comment> GetComments(string game);
    void Reset();
}