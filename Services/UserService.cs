using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using UserManagementAPI.Models;

namespace UserManagementAPI.Services;

public class UserService : IUserService
{
    private static List<User> _users = new List<User>();
    private static int _nextId = 1;
    private readonly ILogger<UserService> _logger;

    public UserService(ILogger<UserService> logger)
    {
        _logger = logger;
        
        if (!_users.Any())
        {
            _users.Add(new User { Id = _nextId++, Name = "Alice Johnson", Email = "alice@example.com", Age = 30 });
            _users.Add(new User { Id = _nextId++, Name = "Bob Smith", Email = "bob@example.com", Age = 25 });
        }
    }

    public List<User> GetAllUsers()
    {
        return _users;
    }

    public User GetUserById(int id)
    {
        return _users.FirstOrDefault(u => u.Id == id);
    }

    public User CreateUser(User user)
    {
        user.Id = _nextId++;
        _users.Add(user);
        _logger.LogInformation($"User created: {user.Name} (ID: {user.Id})");
        return user;
    }

    public User UpdateUser(int id, User user)
    {
        var existingUser = GetUserById(id);
        if (existingUser == null) return null;
        
        existingUser.Name = user.Name;
        existingUser.Email = user.Email;
        existingUser.Age = user.Age;
        
        _logger.LogInformation($"User updated: {existingUser.Name} (ID: {id})");
        return existingUser;
    }

    public bool DeleteUser(int id)
    {
        var user = GetUserById(id);
        if (user == null) return false;
        
        _users.Remove(user);
        _logger.LogInformation($"User deleted: {user.Name} (ID: {id})");
        return true;
    }

    public string Authenticate(string username, string password)
    {
        if (username != "admin" || password != "password")
            return null;
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes("YourSuperSecretKeyThatIsAtLeast32CharactersLong123!");
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, username)
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), 
                SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}