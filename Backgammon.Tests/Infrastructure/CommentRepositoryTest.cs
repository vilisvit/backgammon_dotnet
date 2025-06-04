using Backgammon.Core.Entities;
using Backgammon.Infrastructure.Data;
using Backgammon.Infrastructure.Exceptions;
using Backgammon.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

namespace Backgammon.Tests.Infrastructure;

[TestFixture]
public class CommentRepositoryTest
{
    private GameDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<GameDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new GameDbContext(options);
    }

    [Test]
    public void AddComment_ShouldBeRetrievable()
    {
        var context = GetDbContext();
        var service = new CommentRepository(context);

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = "tester",
            PasswordHash = "secret"
        };
        context.Users.Add(user);
        context.SaveChanges();

        var comment = new Comment
        {
            Game = "Backgammon",
            Content = "Great game!",
            CommentedOn = DateTime.UtcNow,
            UserId = user.Id
        };

        service.AddComment(comment);

        var comments = service.GetComments("Backgammon");
        Assert.That(comments, Has.Count.EqualTo(1));
        Assert.That(comments[0].Content, Is.EqualTo("Great game!"));
        Assert.That(comments[0].UserId, Is.EqualTo(user.Id));
    }

    [Test]
    public void GetComments_EmptyByDefault()
    {
        var context = GetDbContext();
        var service = new CommentRepository(context);

        var comments = service.GetComments("Backgammon");

        Assert.That(comments, Is.Empty);
    }

    [Test]
    public void AddComment_ThrowsOnNullMessage()
    {
        var context = GetDbContext();
        var service = new CommentRepository(context);

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = "tester",
            PasswordHash = "secret"
        };
        context.Users.Add(user);
        context.SaveChanges();

        var comment = new Comment
        {
            Game = "Backgammon",
            Content = null!, // This should violate [Required]
            CommentedOn = DateTime.UtcNow,
            UserId = user.Id
        };

        Assert.Throws<CommentException>(() => {
            service.AddComment(comment);
        });
    }
    
    [Test]
    public void RetrievedComments_ShouldBeOrderedByCommentedOn()
    {
        var context = GetDbContext();
        var service = new CommentRepository(context);

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = "tester",
            PasswordHash = "secret"
        };
        context.Users.Add(user);
        context.SaveChanges();

        var comment1 = new Comment
        {
            Game = "Backgammon",
            Content = "First comment",
            CommentedOn = DateTime.UtcNow.AddMinutes(-10),
            UserId = user.Id
        };
        var comment2 = new Comment
        {
            Game = "Backgammon",
            Content = "Second comment",
            CommentedOn = DateTime.UtcNow,
            UserId = user.Id
        };

        service.AddComment(comment1);
        service.AddComment(comment2);

        var comments = service.GetComments("Backgammon");
        
        Assert.That(comments, Has.Count.EqualTo(2));
        Assert.That(comments[0].Content, Is.EqualTo("Second comment"));
        Assert.That(comments[1].Content, Is.EqualTo("First comment"));
    }
    
    [Test]
    public void Reset_ShouldClearAllComments()
    {
        var context = GetDbContext();
        var service = new CommentRepository(context);

        context.Comments.Add(new Comment
        {
            Game = "Backgammon",
            Content = "Test comment",
            CommentedOn = DateTime.UtcNow,
            UserId = Guid.NewGuid()
        });
        context.SaveChanges();

        service.Reset();

        Assert.That(context.Comments.ToList(), Is.Empty);
    }
}
