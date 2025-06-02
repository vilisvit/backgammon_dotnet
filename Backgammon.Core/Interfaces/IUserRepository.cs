using Backgammon.Core.Entities;

namespace Backgammon.Core.Interfaces;

public interface IUserRepository
{
    User FindByUsername(string username);
    bool ExistsByUsername(string username);
    void AddUser(User user);
}