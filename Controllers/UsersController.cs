using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Models;
using UserManagementAPI.Services;

namespace UserManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    // GET: api/users
    [HttpGet]
    public ActionResult<List<User>> GetAll()
    {
        return Ok(_userService.GetAllUsers());
    }

    // GET: api/users/5
    [HttpGet("{id}")]
    public ActionResult<User> GetById(int id)
    {
        var user = _userService.GetUserById(id);
        if (user == null)
            return NotFound(new { error = $"User with ID {id} not found" });
        
        return Ok(user);
    }

    // POST: api/users
    [HttpPost]
    public ActionResult<User> Create([FromBody] User user)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var createdUser = _userService.CreateUser(user);
        return CreatedAtAction(nameof(GetById), new { id = createdUser.Id }, createdUser);
    }

    // PUT: api/users/5
    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] User user)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var updatedUser = _userService.UpdateUser(id, user);
        if (updatedUser == null)
            return NotFound(new { error = $"User with ID {id} not found" });
        
        return Ok(updatedUser);
    }

    // DELETE: api/users/5
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var deleted = _userService.DeleteUser(id);
        if (!deleted)
            return NotFound(new { error = $"User with ID {id} not found" });
        
        return NoContent();
    }

    // POST: api/users/login (NO AUTH REQUIRED)
    [HttpPost("login")]
    [AllowAnonymous]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var token = _userService.Authenticate(request.Username, request.Password);
        if (token == null)
            return Unauthorized(new { error = "Invalid username or password" });
        
        return Ok(new { token });
    }
}