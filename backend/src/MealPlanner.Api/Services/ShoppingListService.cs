using MealPlanner.Api.Data;
using MealPlanner.Api.Data.Entities;
using MealPlanner.Api.Dtos;
using Microsoft.EntityFrameworkCore;

namespace MealPlanner.Api.Services;

public sealed class ShoppingListService
{
    private readonly MealPlannerDbContext _db;
    private readonly MealPlannerService _planner;

    public ShoppingListService(MealPlannerDbContext db, MealPlannerService planner)
    {
        _db = db;
        _planner = planner;
    }

    public IReadOnlyCollection<ShoppingListItemDto>? CreateFromPlan(UserEntity user, Guid planId)
    {
        var plan = _planner.GetByIdEntity(user, planId);
        if (plan is null)
        {
            return null;
        }

        var required = CalculateRequiredIngredients(plan);
        SubtractInventory(user, required);

        return required.Values.OrderBy(item => item.Name).ToArray();
    }

    private Dictionary<string, ShoppingListItemDto> CalculateRequiredIngredients(MealPlanEntity plan)
    {
        var required = new Dictionary<string, ShoppingListItemDto>(StringComparer.OrdinalIgnoreCase);
        foreach (var day in plan.Days)
        {
            foreach (var meal in day.Meals)
            {
                if (meal.Recipe is null)
                {
                    continue;
                }

                foreach (var recipeIngredient in meal.Recipe.Ingredients)
                {
                    if (recipeIngredient.Ingredient is null)
                    {
                        continue;
                    }

                    var key = BuildKey(recipeIngredient.IngredientId, recipeIngredient.Unit);
                    required.TryGetValue(key, out var current);
                    required[key] = new ShoppingListItemDto(
                        recipeIngredient.IngredientId,
                        recipeIngredient.Ingredient.Name,
                        (current?.Quantity ?? 0) + recipeIngredient.Quantity,
                        recipeIngredient.Unit);
                }
            }
        }

        return required;
    }

    private void SubtractInventory(UserEntity user, Dictionary<string, ShoppingListItemDto> required)
    {
        var inventory = _db.InventoryItems
            .AsNoTracking()
            .Include(item => item.Ingredient)
            .Where(item => item.UserId == user.Id)
            .ToArray();

        foreach (var item in inventory)
        {
            var key = BuildKey(item.IngredientId, item.Unit);
            if (!required.TryGetValue(key, out var current))
            {
                continue;
            }

            var missing = Math.Max(0, current.Quantity - item.Quantity);
            if (missing == 0)
            {
                required.Remove(key);
            }
            else
            {
                required[key] = current with { Quantity = missing };
            }
        }
    }

    private static string BuildKey(Guid ingredientId, string unit)
    {
        return $"{ingredientId}|{unit}";
    }
}
