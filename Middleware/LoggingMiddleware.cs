using System.Diagnostics;

namespace UserManagementAPI.Middleware;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        
        _logger.LogInformation("→ {Method} {Path} - Request received", 
            context.Request.Method, 
            context.Request.Path);
        
        await _next(context);
        
        stopwatch.Stop();
        
        _logger.LogInformation("← {Method} {Path} - Response: {StatusCode} ({Duration}ms)", 
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            stopwatch.ElapsedMilliseconds);
    }
}