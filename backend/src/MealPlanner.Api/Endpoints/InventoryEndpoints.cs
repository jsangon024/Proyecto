using MealPlanner.Api.Dtos;
using MealPlanner.Api.Services;

namespace MealPlanner.Api.Endpoints;

public static class InventoryEndpoints
{
    public static IEndpointRouteBuilder MapInventoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/inventory").WithTags("Inventory");

        group.MapGet("/", GetAll);
        group.MapGet("/{id:guid}", GetById);
        group.MapPost("/", Create);
        group.MapPut("/{id:guid}", Update);
        group.MapDelete("/{id:guid}", Delete);

        return app;
    }

    private static IResult GetAll(HttpContext context, InventoryService inventory)
    {
        var user = EndpointHelpers.CurrentUser(context);
        return user is null ? Results.Unauthorized() : Results.Ok(inventory.GetAll(user).Select(InventoryItemDto.From));
    }

    private static IResult GetById(Guid id, HttpContext context, InventoryService inventory)
    {
        var user = EndpointHelpers.CurrentUser(context);
        if (user is null)
        {
            return Results.Unauthorized();
        }

        var item = inventory.GetById(user, id);
        return item is null ? Results.NotFound() : Results.Ok(InventoryItemDto.From(item));
    }

    private static IResult Create(InventoryCreateRequest request, HttpContext context, InventoryService inventory)
    {
        var user = EndpointHelpers.CurrentUser(context);
        if (user is null)
        {
            return Results.Unauthorized();
        }

        try
        {
            var item = inventory.Create(user, request);
            return Results.Created($"/api/inventory/{item.Id}", InventoryItemDto.From(item));
        }
        catch (ValidationException exception)
        {
            return EndpointHelpers.ValidationError(exception);
        }
    }

    private static IResult Update(Guid id, InventoryUpdateRequest request, HttpContext context, InventoryService inventory)
    {
        var user = EndpointHelpers.CurrentUser(context);
        if (user is null)
        {
            return Results.Unauthorized();
        }

        try
        {
            var item = inventory.Update(user, id, request);
            return item is null ? Results.NotFound() : Results.Ok(InventoryItemDto.From(item));
        }
        catch (ValidationException exception)
        {
            return EndpointHelpers.ValidationError(exception);
        }
    }

    private static IResult Delete(Guid id, HttpContext context, InventoryService inventory)
    {
        var user = EndpointHelpers.CurrentUser(context);
        return user is null
            ? Results.Unauthorized()
            : inventory.Delete(user, id) ? Results.NoContent() : Results.NotFound();
    }
}
