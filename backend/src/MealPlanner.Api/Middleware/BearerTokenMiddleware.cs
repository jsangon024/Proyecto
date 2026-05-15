using MealPlanner.Api.Services;

namespace MealPlanner.Api.Middleware;

public sealed class BearerTokenMiddleware
{
    public const string UserContextKey = "current-user";
    private readonly RequestDelegate _next;

    public BearerTokenMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, TokenService tokens)
    {
        var authorization = context.Request.Headers.Authorization.ToString();
        if (authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            var token = authorization["Bearer ".Length..].Trim();
            var user = tokens.Validate(token);
            if (user is not null)
            {
                context.Items[UserContextKey] = user;
            }
        }

        await _next(context);
    }
}
