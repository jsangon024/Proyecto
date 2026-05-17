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
        group.MapPost("/verify-email", VerifyEmail);
        group.MapPost("/forgot-password", ForgotPassword);
        group.MapPost("/reset-password", ResetPassword);

        return app;
    }

    private static async Task<IResult> Register(RegisterRequest request, AuthService auth)
    {
        try
        {
            var response = await auth.RegisterAsync(request);
            return Results.Created("/api/auth/register", response);
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
        try
        {
            var response = auth.Login(request);
            return response is null
                ? Results.Json(new ErrorResponse("Email o password incorrectos."), statusCode: StatusCodes.Status401Unauthorized)
                : Results.Ok(response);
        }
        catch (ValidationException exception)
        {
            return EndpointHelpers.ValidationError(exception);
        }
    }

    private static IResult VerifyEmail(VerifyEmailRequest request, AuthService auth)
    {
        try
        {
            return Results.Ok(auth.VerifyEmail(request));
        }
        catch (ValidationException exception)
        {
            return EndpointHelpers.ValidationError(exception);
        }
    }

    private static async Task<IResult> ForgotPassword(ForgotPasswordRequest request, AuthService auth)
    {
        try
        {
            return Results.Ok(await auth.ForgotPasswordAsync(request));
        }
        catch (ValidationException exception)
        {
            return EndpointHelpers.ValidationError(exception);
        }
    }

    private static IResult ResetPassword(ResetPasswordRequest request, AuthService auth)
    {
        try
        {
            return Results.Ok(auth.ResetPassword(request));
        }
        catch (ValidationException exception)
        {
            return EndpointHelpers.ValidationError(exception);
        }
    }
}
