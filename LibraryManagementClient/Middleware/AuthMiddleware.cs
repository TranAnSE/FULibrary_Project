using LibraryManagementClient.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementClient.Middleware;

public class AuthMiddleware
{
    private readonly RequestDelegate _next;

    public AuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IApiService apiService)
    {
        var path = context.Request.Path;
        
        // Skip authentication for certain paths
        if (path.StartsWithSegments("/Auth") || 
            path.StartsWithSegments("/Home") ||
            path.StartsWithSegments("/Search") ||
            path.StartsWithSegments("/Books/Details") || // Allow guests to view book details
            path.StartsWithSegments("/Books/New") || // Allow guests to view new books
            path.StartsWithSegments("/css") ||
            path.StartsWithSegments("/js") ||
            path.StartsWithSegments("/lib"))
        {
            await _next(context);
            return;
        }

        // Check if user is authenticated via cookie
        if (context.User?.Identity?.IsAuthenticated != true)
        {
            context.Response.Redirect("/Auth/Login");
            return;
        }

        // Check if we have a valid auth token
        var token = context.Session.GetString("AuthToken");
        if (string.IsNullOrEmpty(token))
        {
            // Try to get token from claims
            token = context.User.FindFirst("jwt_token")?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                // Restore token in session and api service
                apiService.SetAuthToken(token);
            }
        }

        if (string.IsNullOrEmpty(token))
        {
            context.Response.Redirect("/Auth/Login");
            return;
        }

        await _next(context);
    }
}
