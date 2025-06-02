using System.ComponentModel.DataAnnotations;
using Backgammon.Core.Entities;
using Backgammon.Core.Interfaces;
using Backgammon.Infrastructure.Data;

namespace Backgammon.Infrastructure.Repository;

public class UserRepository(GameDbContext db) : IUserRepository
{
    
    
    public User FindByUsername(string username)
    {
        throw new NotImplementedException();
    }

    public bool ExistsByUsername(string username)
    {
        throw new NotImplementedException();
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