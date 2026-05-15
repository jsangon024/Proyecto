using MealPlanner.Api.Dtos;
using MealPlanner.Api.Services;

namespace MealPlanner.Api.Endpoints;

public static class ShoppingListEndpoints
{
    public static IEndpointRouteBuilder MapShoppingListEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/shopping-list/from-plan", CreateFromPlan)
            .WithTags("Shopping List");

        return app;
    }

    private static IResult CreateFromPlan(ShoppingListRequest request, HttpContext context, ShoppingListService shoppingLists)
    {
        var user = EndpointHelpers.CurrentUser(context);
        if (user is null)
        {
            return Results.Unauthorized();
        }

        var shoppingList = shoppingLists.CreateFromPlan(user, request.PlanId);
        return shoppingList is null ? Results.NotFound() : Results.Ok(shoppingList);
    }
}
