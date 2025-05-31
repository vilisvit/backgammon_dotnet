using Backgammon.Infrastructure.Data;
using Backgammon.Infrastructure.Entities;
using Backgammon.Infrastructure.Exceptions;

namespace Backgammon.Infrastructure.Services;

public class ScoreServiceEf(GameDbContext db) : IScoreService
{
    public void AddScore(Score score)
    {
        try
        {
            db.Scores.Add(score);
            db.SaveChanges();
        }
        catch (Exception e)
        {
            throw new ScoreException("Problem inserting score", e);
        }
    }

    public List<Score> GetTopScores(string game)
    {
        try
        {
            return db.Scores
                .Where(s => s.Game == game)
                .OrderByDescending(s => s.Points)
                .Take(10)
                .ToList();
        }
        catch (Exception e)
        {
            throw new ScoreException("Problem retrieving top scores", e);
        }
    }

    public void Reset()
    {
        db.Scores.RemoveRange(db.Scores);
        db.SaveChanges();
    }
}