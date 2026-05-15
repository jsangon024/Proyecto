using MealPlanner.Api.Dtos;
using MealPlanner.Api.Services;

namespace MealPlanner.Api.Endpoints;

public static class RecipeEndpoints
{
    public static IEndpointRouteBuilder MapRecipeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/recipes").WithTags("Recipes");

        group.MapGet("/", GetAll);
        group.MapGet("/{id:guid}", GetById);
        group.MapPost("/", Create);
        group.MapPut("/{id:guid}", Update);
        group.MapDelete("/{id:guid}", Delete);
        group.MapGet("/saved", GetSaved);
        group.MapPost("/{id:guid}/save", SaveForUser);
        group.MapDelete("/{id:guid}/save", UnsaveForUser);

        return app;
    }

    private static IResult GetAll(RecipeService recipes)
    {
        return Results.Ok(recipes.GetAll().Select(RecipeDto.From));
    }

    private static IResult GetById(Guid id, RecipeService recipes)
    {
        var recipe = recipes.GetById(id);
        return recipe is null ? Results.NotFound() : Results.Ok(RecipeDto.From(recipe));
    }

    private static IResult Create(RecipeCreateRequest request, HttpContext context, RecipeService recipes)
    {
        var adminCheck = EndpointHelpers.RequireAdmin(EndpointHelpers.CurrentUser(context));
        if (adminCheck != Results.Empty)
        {
            return adminCheck;
        }

        try
        {
            var recipe = recipes.Create(request);
            return Results.Created($"/api/recipes/{recipe.Id}", RecipeDto.From(recipe));
        }
        catch (ValidationException exception)
        {
            return EndpointHelpers.ValidationError(exception);
        }
    }

    private static IResult Update(Guid id, RecipeUpdateRequest request, HttpContext context, RecipeService recipes)
    {
        var adminCheck = EndpointHelpers.RequireAdmin(EndpointHelpers.CurrentUser(context));
        if (adminCheck != Results.Empty)
        {
            return adminCheck;
        }

        try
        {
            var recipe = recipes.Update(id, request);
            return recipe is null ? Results.NotFound() : Results.Ok(RecipeDto.From(recipe));
        }
        catch (ValidationException exception)
        {
            return EndpointHelpers.ValidationError(exception);
        }
    }

    private static IResult Delete(Guid id, HttpContext context, RecipeService recipes)
    {
        var adminCheck = EndpointHelpers.RequireAdmin(EndpointHelpers.CurrentUser(context));
        if (adminCheck != Results.Empty)
        {
            return adminCheck;
        }

        return recipes.Delete(id) ? Results.NoContent() : Results.NotFound();
    }

    private static IResult GetSaved(HttpContext context, RecipeService recipes)
    {
        var user = EndpointHelpers.CurrentUser(context);
        return user is null
            ? Results.Unauthorized()
            : Results.Ok(recipes.GetSavedByUser(user.Id).Select(RecipeDto.From));
    }

    private static IResult SaveForUser(Guid id, HttpContext context, RecipeService recipes)
    {
        var user = EndpointHelpers.CurrentUser(context);
        if (user is null)
        {
            return Results.Unauthorized();
        }

        return recipes.SaveForUser(user.Id, id) ? Results.NoContent() : Results.NotFound();
    }

    private static IResult UnsaveForUser(Guid id, HttpContext context, RecipeService recipes)
    {
        var user = EndpointHelpers.CurrentUser(context);
        if (user is null)
        {
            return Results.Unauthorized();
        }

        return recipes.UnsaveForUser(user.Id, id) ? Results.NoContent() : Results.NotFound();
    }
}
