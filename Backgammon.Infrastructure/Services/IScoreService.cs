using Backgammon.Infrastructure.Entities;

namespace Backgammon.Infrastructure.Services;

public interface IScoreService
{
    void AddScore(Score score);
    List<Score> GetTopScores(string game);
    void Reset();
}