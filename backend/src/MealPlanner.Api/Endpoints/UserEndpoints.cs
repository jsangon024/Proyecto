using MealPlanner.Api.Dtos;
using MealPlanner.Api.Models;
using MealPlanner.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace MealPlanner.Api.Endpoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/me").WithTags("Users");

        group.MapGet("/", GetCurrentUser);
        group.MapPut("/goals", UpdateGoals);
        group.MapPut("/password", ChangePassword);
        group.MapDelete("/", DeleteOwnAccount);

        return app;
    }

    private static IResult GetCurrentUser(HttpContext context)
    {
        var user = EndpointHelpers.CurrentUser(context);
        return user is null ? Results.Unauthorized() : Results.Ok(UserDto.From(user));
    }

    private static IResult UpdateGoals(NutritionGoals goals, HttpContext context, UserService users)
    {
        var user = EndpointHelpers.CurrentUser(context);
        if (user is null)
        {
            return Results.Unauthorized();
        }

        try
        {
            return Results.Ok(UserDto.From(users.UpdateGoals(user, goals)));
        }
        catch (ValidationException exception)
        {
            return EndpointHelpers.ValidationError(exception);
        }
    }

    private static IResult ChangePassword(ChangePasswordRequest request, HttpContext context, UserService users)
    {
        var user = EndpointHelpers.CurrentUser(context);
        if (user is null)
        {
            return Results.Unauthorized();
        }

        try
        {
            users.ChangePassword(user, request);
            return Results.NoContent();
        }
        catch (ValidationException exception)
        {
            return EndpointHelpers.ValidationError(exception);
        }
    }

    private static IResult DeleteOwnAccount([FromBody] DeleteAccountRequest request, HttpContext context, UserService users)
    {
        var user = EndpointHelpers.CurrentUser(context);
        if (user is null)
        {
            return Results.Unauthorized();
        }

        try
        {
            users.DeleteOwnAccount(user, request);
            return Results.NoContent();
        }
        catch (ValidationException exception)
        {
            return EndpointHelpers.ValidationError(exception);
        }
    }
}
