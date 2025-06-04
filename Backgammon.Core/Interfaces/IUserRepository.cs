using Backgammon.Core.Entities;

namespace Backgammon.Core.Interfaces;

public interface IUserRepository
{
    User FindById(Guid userId);
    bool ExistsById(Guid userId);
    void AddUser(User user);
}