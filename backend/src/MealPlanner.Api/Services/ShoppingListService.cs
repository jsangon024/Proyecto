using MealPlanner.Api.Data;
using MealPlanner.Api.Data.Entities;
using MealPlanner.Api.Dtos;
using Microsoft.EntityFrameworkCore;

namespace MealPlanner.Api.Services;

public sealed class ShoppingListService
{
    private readonly MealPlannerDbContext _db;

    public ShoppingListService(MealPlannerDbContext db)
    {
        _db = db;
    }

    public IReadOnlyCollection<ShoppingListItemDto>? CreateFromPlans(
        UserEntity user,
        IReadOnlyCollection<Guid> planIds,
        Guid? selectedPlanId,
        IReadOnlyCollection<string>? selectedDates,
        bool onlyMissing)
    {
        var normalizedPlanIds = planIds.Distinct().ToArray();
        if (normalizedPlanIds.Length == 0)
        {
            return null;
        }

        var plans = _db.MealPlans
            .AsNoTracking()
            .IncludeFullPlan()
            .Where(plan => plan.UserId == user.Id && normalizedPlanIds.Contains(plan.Id))
            .ToArray();

        if (plans.Length != normalizedPlanIds.Length)
        {
            return null;
        }

        var required = CalculateRequiredIngredients(plans, selectedPlanId, selectedDates);
        if (onlyMissing)
        {
            SubtractInventory(user, required);
        }

        return required.Values.OrderBy(item => item.Name).ToArray();
    }

    private static Dictionary<string, ShoppingListItemDto> CalculateRequiredIngredients(
        IReadOnlyCollection<MealPlanEntity> plans,
        Guid? selectedPlanId,
        IReadOnlyCollection<string>? selectedDates)
    {
        var dateFilter = selectedDates is null || selectedDates.Count == 0
            ? null
            : selectedDates.Select(date => DateOnly.TryParse(date, out var parsed) ? parsed : (DateOnly?)null)
                .Where(date => date is not null)
                .Select(date => date!.Value)
                .ToHashSet();

        var required = new Dictionary<string, ShoppingListItemDto>(StringComparer.OrdinalIgnoreCase);
        foreach (var plan in plans)
        {
            var shouldFilterPlanByDate = selectedPlanId is not null && plan.Id == selectedPlanId;
            foreach (var day in plan.Days.Where(day =>
                !day.IsCompleted &&
                (!shouldFilterPlanByDate || dateFilter is null || dateFilter.Contains(day.PlanDate))))
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
