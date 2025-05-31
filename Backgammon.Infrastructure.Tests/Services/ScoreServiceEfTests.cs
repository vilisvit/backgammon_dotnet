using Microsoft.EntityFrameworkCore;
using Backgammon.Infrastructure.Data;
using Backgammon.Infrastructure.Entities;
using Backgammon.Infrastructure.Services;

namespace Backgammon.Infrastructure.Tests.Services
{
    public class ScoreServiceEfTests
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
        public void AddAndGetTopScores_ShouldReturnCorrectScore()
        {
            var context = GetDbContext();
            var service = new ScoreServiceEf(context);
            var userId = Guid.NewGuid();

            context.Users.Add(new User { Id = userId, Username = "Test", PasswordHash = "hash" });
            context.SaveChanges();

            service.AddScore(new Score { Game = "backgammon", Points = 120, PlayedOn = DateTime.UtcNow, UserId = userId });

            var scores = service.GetTopScores("backgammon");
            Assert.That(scores, Has.Exactly(1).Items);
            Assert.That(scores.First().Points, Is.EqualTo(120));
        }

        [Test]
        public void Reset_ShouldClearAllScores()
        {
            var context = GetDbContext();
            var service = new ScoreServiceEf(context);

            context.Scores.Add(new Score { Game = "game", Points = 50, PlayedOn = DateTime.UtcNow, UserId = Guid.NewGuid() });
            context.SaveChanges();

            service.Reset();
            Assert.That(context.Scores.ToList(), Is.Empty);
        }
    }
}