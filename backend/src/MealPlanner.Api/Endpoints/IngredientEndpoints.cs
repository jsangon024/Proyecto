using MealPlanner.Api.Dtos;
using MealPlanner.Api.Services;

namespace MealPlanner.Api.Endpoints;

public static class IngredientEndpoints
{
    public static IEndpointRouteBuilder MapIngredientEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/ingredients").WithTags("Ingredients");

        group.MapGet("/", GetAll);
        group.MapGet("/{id:guid}", GetById);
        group.MapPost("/", Create);
        group.MapPut("/{id:guid}", Update);
        group.MapDelete("/{id:guid}", Delete);

        return app;
    }

    private static IResult GetAll(IngredientService ingredients)
    {
        return Results.Ok(ingredients.GetAll().Select(IngredientDto.From));
    }

    private static IResult GetById(Guid id, IngredientService ingredients)
    {
        var ingredient = ingredients.GetById(id);
        return ingredient is null ? Results.NotFound() : Results.Ok(IngredientDto.From(ingredient));
    }

    private static IResult Create(IngredientCreateRequest request, HttpContext context, IngredientService ingredients)
    {
        var adminCheck = EndpointHelpers.RequireAdmin(EndpointHelpers.CurrentUser(context));
        if (adminCheck != Results.Empty)
        {
            return adminCheck;
        }

        try
        {
            var ingredient = ingredients.Create(request);
            return Results.Created($"/api/ingredients/{ingredient.Id}", IngredientDto.From(ingredient));
        }
        catch (ValidationException exception)
        {
            return EndpointHelpers.ValidationError(exception);
        }
    }

    private static IResult Update(Guid id, IngredientUpdateRequest request, HttpContext context, IngredientService ingredients)
    {
        var adminCheck = EndpointHelpers.RequireAdmin(EndpointHelpers.CurrentUser(context));
        if (adminCheck != Results.Empty)
        {
            return adminCheck;
        }

        try
        {
            var ingredient = ingredients.Update(id, request);
            return ingredient is null ? Results.NotFound() : Results.Ok(IngredientDto.From(ingredient));
        }
        catch (ValidationException exception)
        {
            return EndpointHelpers.ValidationError(exception);
        }
    }

    private static IResult Delete(Guid id, HttpContext context, IngredientService ingredients)
    {
        var adminCheck = EndpointHelpers.RequireAdmin(EndpointHelpers.CurrentUser(context));
        if (adminCheck != Results.Empty)
        {
            return adminCheck;
        }

        return ingredients.Delete(id) ? Results.NoContent() : Results.NotFound();
    }
}
