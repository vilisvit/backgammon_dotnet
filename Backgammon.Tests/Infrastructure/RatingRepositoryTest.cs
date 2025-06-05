using Backgammon.Core.Entities;
using Backgammon.Core.Exceptions;
using Backgammon.Infrastructure.Data;
using Backgammon.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

namespace Backgammon.Tests.Infrastructure;

public class RatingRepositoryTest
{
    private GameDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<GameDbContext>()
            .UseInMemoryDatabase(
                Guid.NewGuid().ToString())
            .Options;

        return new GameDbContext(options);
    }
    
    [Test]
    public void SetRatingWhenItIsNew_ShouldAddRating()
    {
        var context = GetDbContext();
        var service = new RatingRepository(context);
        var userId = Guid.NewGuid();

        context.Users.Add(new User { Id = userId, UserName = "TestUser", PasswordHash = "hash" });
        context.SaveChanges();

        service.SetRating(new Rating { Game = "backgammon", Value = 5, UserId = userId });

        var rating = service.GetRating("backgammon", userId);
        Assert.That(rating, Is.EqualTo(5));
    }
    
    [Test]
    public void SetRatingWhenItExists_ShouldUpdateRating()
    {
        var context = GetDbContext();
        var service = new RatingRepository(context);
        var userId = Guid.NewGuid();

        context.Users.Add(new User { Id = userId, UserName = "TestUser", PasswordHash = "hash" });
        context.SaveChanges();

        service.SetRating(new Rating { Game = "backgammon", Value = 5, UserId = userId });
        service.SetRating(new Rating { Game = "backgammon", Value = 3, UserId = userId });

        var rating = service.GetRating("backgammon", userId);
        Assert.That(rating, Is.EqualTo(3));
    }
    
    [Test]
    public void AddingTooHighRating_ShouldThrowException()
    {
        var context = GetDbContext();
        var service = new RatingRepository(context);
        var userId = Guid.NewGuid();

        context.Users.Add(new User { Id = userId, UserName = "TestUser", PasswordHash = "hash" });
        context.SaveChanges();
        
        var rating = service.GetRating("backgammon", userId);
        
        // print the current rating for debugging
        Console.WriteLine($"Current rating for user {userId}: {rating}");

        Assert.Throws<RatingException>(() => 
            service.SetRating(new Rating { Game = "backgammon", Value = 6, UserId = userId }));
    }
    
    [Test]
    public void GetAverageRating_ShouldReturnCorrectAverage()
    {
        var context = GetDbContext();
        var service = new RatingRepository(context);
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();

        context.Users.AddRange(
            new User { Id = userId1, UserName = "User1", PasswordHash = "hash" },
            new User { Id = userId2, UserName = "User2", PasswordHash = "hash" }
        );
        context.SaveChanges();

        service.SetRating(new Rating { Game = "backgammon", Value = 4, UserId = userId1 });
        service.SetRating(new Rating { Game = "backgammon", Value = 2, UserId = userId2 });

        var averageRating = service.GetAverageRating("backgammon");
        Assert.That(averageRating, Is.EqualTo(3));
    }

    [Test]
    public void GetAverageRating_ShouldReturnCorrectAverage_WhenGameHasNoRatings()
    {
        var context = GetDbContext();
        var service = new RatingRepository(context);

        var averageRating = service.GetAverageRating("backgammon");
        Assert.That(averageRating, Is.EqualTo(0));
    }
    
    [Test]
    public void Reset_ShouldClearAllRatings()
    {
        var context = GetDbContext();
        var service = new RatingRepository(context);

        context.Ratings.Add(new Rating { Game = "backgammon", Value = 5, UserId = Guid.NewGuid(), RatedOn = DateTime.UtcNow });
        context.SaveChanges();

        service.Reset();
        Assert.That(context.Ratings.ToList(), Is.Empty);
    }
}