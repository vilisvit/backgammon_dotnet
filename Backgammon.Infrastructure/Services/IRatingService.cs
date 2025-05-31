using Backgammon.Infrastructure.Entities;

namespace Backgammon.Infrastructure.Services;

public interface IRatingService
{
    void SetRating(Rating rating);
    int GetAverageRating(string game);
    int GetRating(string game, Guid userId);
    void Reset();
}