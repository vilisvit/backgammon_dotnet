using Backgammon.Core.Entities;
namespace Backgammon.Core.Interfaces;

public interface IRatingRepository
{
    void SetRating(Rating rating);
    int GetAverageRating(string game);
    int GetRating(string game, Guid userId);
    void Reset();
}