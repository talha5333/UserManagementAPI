using UserManagementAPI.Models;

namespace UserManagementAPI.Services;

public interface IUserService
{
    List<User> GetAllUsers();
    User GetUserById(int id);
    User CreateUser(User user);
    User UpdateUser(int id, User user);
    bool DeleteUser(int id);
    string Authenticate(string username, string password);
}