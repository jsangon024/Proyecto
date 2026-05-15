namespace MealPlanner.Api.Models;

public sealed record NutritionGoals(int DailyCalories, int MinimumProteinGrams, string[] ExcludedIngredients)
{
    public static NutritionGoals Default => new(2200, 120, Array.Empty<string>());
}
