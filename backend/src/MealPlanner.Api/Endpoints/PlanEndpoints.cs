using MealPlanner.Api.Dtos;
using MealPlanner.Api.Services;

namespace MealPlanner.Api.Endpoints;

public static class PlanEndpoints
{
    public static IEndpointRouteBuilder MapPlanEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/plans").WithTags("Plans");

        group.MapGet("/", GetAll);
        group.MapPost("/generate", Generate);
        group.MapGet("/latest", GetLatest);
        group.MapGet("/{id:guid}", GetById);
        group.MapDelete("/{id:guid}", Delete);

        return app;
    }

    private static IResult Generate(GeneratePlanRequest request, HttpContext context, MealPlannerService planner)
    {
        var user = EndpointHelpers.CurrentUser(context);
        if (user is null)
        {
            return Results.Unauthorized();
        }

        try
        {
            var plan = planner.Generate(user, request.Year, request.Month);
            return Results.Created($"/api/plans/{plan.Id}", plan);
        }
        catch (ValidationException exception)
        {
            return EndpointHelpers.ValidationError(exception);
        }
    }

    private static IResult GetAll(HttpContext context, MealPlannerService planner)
    {
        var user = EndpointHelpers.CurrentUser(context);
        if (user is null)
        {
            return Results.Unauthorized();
        }

        return Results.Ok(planner.GetAll(user));
    }

    private static IResult GetLatest(HttpContext context, MealPlannerService planner)
    {
        var user = EndpointHelpers.CurrentUser(context);
        if (user is null)
        {
            return Results.Unauthorized();
        }

        var plan = planner.GetLatest(user);
        return plan is null ? Results.NotFound() : Results.Ok(plan);
    }

    private static IResult GetById(Guid id, HttpContext context, MealPlannerService planner)
    {
        var user = EndpointHelpers.CurrentUser(context);
        if (user is null)
        {
            return Results.Unauthorized();
        }

        var plan = planner.GetById(user, id);
        return plan is null ? Results.NotFound() : Results.Ok(plan);
    }

    private static IResult Delete(Guid id, HttpContext context, MealPlannerService planner)
    {
        var user = EndpointHelpers.CurrentUser(context);
        if (user is null)
        {
            return Results.Unauthorized();
        }

        return planner.Delete(user, id) ? Results.NoContent() : Results.NotFound();
    }
}
