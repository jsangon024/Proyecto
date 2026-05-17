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

        var planIds = request.PlanIds is { Length: > 0 }
            ? request.PlanIds
            : request.PlanId is null
                ? Array.Empty<Guid>()
                : new[] { request.PlanId.Value };

        var shoppingList = shoppingLists.CreateFromPlans(
            user,
            planIds,
            request.PlanId,
            request.Dates,
            request.OnlyMissing ?? true);

        return shoppingList is null ? Results.NotFound() : Results.Ok(shoppingList);
    }
}
