using Backgammon.Core.Entities;

namespace Backgammon.Core.Interfaces;

public interface IScoreRepository
{
    void AddScore(Score score);
    List<Score> GetTopScores(string game);
    void Reset();
}