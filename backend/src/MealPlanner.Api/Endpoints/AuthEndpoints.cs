using MealPlanner.Api.Dtos;
using MealPlanner.Api.Services;

namespace MealPlanner.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/register", Register);
        group.MapPost("/login", Login);

        return app;
    }

    private static IResult Register(RegisterRequest request, AuthService auth)
    {
        try
        {
            var response = auth.Register(request);
            return Results.Created($"/api/users/{response.User.Id}", response);
        }
        catch (ConflictException exception)
        {
            return Results.Conflict(new ErrorResponse(exception.Message));
        }
        catch (ValidationException exception)
        {
            return EndpointHelpers.ValidationError(exception);
        }
    }

    private static IResult Login(LoginRequest request, AuthService auth)
    {
        var response = auth.Login(request);
        return response is null ? Results.Unauthorized() : Results.Ok(response);
    }
}
