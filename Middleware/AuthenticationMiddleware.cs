using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace UserManagementAPI.Middleware;

public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuthenticationMiddleware> _logger;

    public AuthenticationMiddleware(RequestDelegate next, ILogger<AuthenticationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.ToString();
        
        // Skip authentication for login endpoint and Swagger
        if (path.Contains("/login") || path.Contains("/swagger"))
        {
            await _next(context);
            return;
        }
        
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogWarning("Unauthorized access attempt to {Path}", path);
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new { error = "Unauthorized. Token is required." });
            return;
        }
        
        if (!ValidateToken(token))
        {
            _logger.LogWarning("Invalid token used for {Path}", path);
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new { error = "Unauthorized. Invalid token." });
            return;
        }
        
        await _next(context);
    }

    private bool ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("YourSuperSecretKeyThatIsAtLeast32CharactersLong123!");
            
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out _);
            
            return true;
        }
        catch
        {
            return false;
        }
    }
}