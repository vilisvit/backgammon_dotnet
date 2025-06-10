using System.ComponentModel.DataAnnotations;
using Backgammon.Core.Entities;
using Backgammon.Core.Interfaces;
using Backgammon.Infrastructure.Data;

namespace Backgammon.Infrastructure.Repository;

public class UserRepository(GameDbContext db) : IUserRepository
{
    public User FindByUserName(string userName)
    {
        var user = db.Users.FirstOrDefault(u => u.UserName == userName);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with username {userName} not found.");
        }
        return user;
    }

    public bool ExistsByUserName(string userName)
    {
        return db.Users.Any(u => u.UserName == userName);
    }

    public User FindById(Guid userId)
    {
        var user = db.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {userId} not found.");
        }
        return user;
    }

    public bool ExistsById(Guid userId)
    {
        return db.Users.Any(u => u.Id == userId);
    }

    public void AddUser(User user)
    {
        try
        {
            Validator.ValidateObject(user, new ValidationContext(user), validateAllProperties: true);
            db.Users.Add(user);
            db.SaveChanges();
        }
        catch (Exception e)
        {
            throw new Exception("Problem adding user", e);
        }
    }
}