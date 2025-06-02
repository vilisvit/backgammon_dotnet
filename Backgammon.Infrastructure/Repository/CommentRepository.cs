using System.ComponentModel.DataAnnotations;
using Backgammon.Core.Entities;
using Backgammon.Core.Interfaces;
using Backgammon.Infrastructure.Data;
using Backgammon.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Backgammon.Infrastructure.Repository;

public class CommentRepository(GameDbContext db) : ICommentRepository
{
    public void AddComment(Comment comment)
    {
        try
        {
            Validator.ValidateObject(comment, new ValidationContext(comment), validateAllProperties: true);
            
            db.Comments.Add(comment);
            db.SaveChanges();
        }
        catch (Exception e)
        {
            throw new CommentException("Problem inserting comment", e);
        }
    }

    public List<Comment> GetComments(string game)
    {
        try
        {
            return db.Comments
                .Include(c => c.User)
                .Where(c => c.Game == game)
                .OrderByDescending(c => c.CommentedOn)
                .ToList();
        }
        catch (Exception e)
        {
            throw new CommentException("Problem retrieving comments", e);
        }
    }

    public void Reset()
    {
        db.Comments.RemoveRange(db.Comments);
        db.SaveChanges();
    }
}