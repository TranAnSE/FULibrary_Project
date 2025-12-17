using System.Security.Claims;

namespace LibraryManagementAPI.Middleware;

public class LibraryScopeMiddleware
{
    private readonly RequestDelegate _next;

    public LibraryScopeMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var roles = context.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            var libraryIdClaim = context.User.FindFirst("LibraryId");

            // If user is a Librarian, add LibraryId to HttpContext items for filtering
            if (roles.Contains("Librarian") && !roles.Contains("Admin") && libraryIdClaim != null)
            {
                context.Items["LibraryId"] = Guid.Parse(libraryIdClaim.Value);
            }
        }

        await _next(context);
    }
}

public static class LibraryScopeMiddlewareExtensions
{
    public static IApplicationBuilder UseLibraryScope(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<LibraryScopeMiddleware>();
    }
}
