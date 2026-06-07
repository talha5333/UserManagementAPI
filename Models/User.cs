using System.ComponentModel.DataAnnotations;

namespace UserManagementAPI.Models;

public class User
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Name is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 50 characters")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; }
    
    [Range(18, 100, ErrorMessage = "Age must be between 18 and 100")]
    public int Age { get; set; }
}

public class LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}