using System.ComponentModel.DataAnnotations;
using Backgammon.Infrastructure.Data;
using Backgammon.Infrastructure.Entities;
using Backgammon.Infrastructure.Exceptions;

namespace Backgammon.Infrastructure.Services;

public class RatingServiceEf(GameDbContext db) : IRatingService
{
    public void SetRating(Rating rating)
    {
        try
        {
            Validator.ValidateObject(rating, new ValidationContext(rating), validateAllProperties: true);
            
            var existing = db.Ratings
                .FirstOrDefault(r => r.Game == rating.Game && r.UserId == rating.UserId);

            if (existing != null)
            {
                existing.Value = rating.Value;
                db.Ratings.Update(existing);
            }
            else
            {
                db.Ratings.Add(rating);
            }

            db.SaveChanges();
        }
        catch (Exception e)
        {
            throw new RatingException("Problem setting rating", e);
        }
    }

    public int GetRating(string game, Guid userId)
    {
        try
        {
            return db.Ratings
                .FirstOrDefault(r => r.Game == game && r.UserId == userId)?.Value ?? 0;
        }
        catch (Exception e)
        {
            throw new RatingException("Problem retrieving rating", e);
        }
    }

    public int GetAverageRating(string game)
    {
        try
        {
            var query = db.Ratings
                .Where(r => r.Game == game);

            if (!query.Any())
                return 0;

            return (int)Math.Round(query.Average(r => (double)r.Value));
        }
        catch (Exception e)
        {
            throw new RatingException("Problem retrieving average rating", e);
        }
    }

    public void Reset()
    {
        db.Ratings.RemoveRange(db.Ratings);
        db.SaveChanges();
    }
}