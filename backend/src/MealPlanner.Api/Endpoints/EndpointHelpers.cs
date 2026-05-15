using MealPlanner.Api.Data.Entities;
using MealPlanner.Api.Dtos;
using MealPlanner.Api.Middleware;
using MealPlanner.Api.Services;

namespace MealPlanner.Api.Endpoints;

public static class EndpointHelpers
{
    public static UserEntity? CurrentUser(HttpContext context)
    {
        return context.Items.TryGetValue(BearerTokenMiddleware.UserContextKey, out var value) ? value as UserEntity : null;
    }

    public static IResult RequireAdmin(UserEntity? user)
    {
        if (user is null)
        {
            return Results.Unauthorized();
        }

        return user.Role == "admin"
            ? Results.Empty
            : Results.StatusCode(StatusCodes.Status403Forbidden);
    }

    public static IResult ValidationError(ValidationException exception)
    {
        return Results.BadRequest(new ErrorResponse(exception.Message));
    }
}
