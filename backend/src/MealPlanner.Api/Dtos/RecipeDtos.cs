using MealPlanner.Api.Data.Entities;

namespace MealPlanner.Api.Dtos;

public sealed record RecipeDto(
    Guid Id,
    string Name,
    string MealType,
    string? Description,
    decimal Calories,
    decimal ProteinGrams,
    bool IsGlutenFree,
    bool IsDiabeticFriendly,
    int Servings,
    string[] Tags,
    RecipeIngredientDto[] Ingredients)
{
    public static RecipeDto From(RecipeEntity recipe)
    {
        var ingredients = recipe.Ingredients
            .Where(item => item.Ingredient is not null)
            .Select(RecipeIngredientDto.From)
            .ToArray();

        return new RecipeDto(
            recipe.Id,
            recipe.Name,
            recipe.MealType,
            recipe.Description,
            ingredients.Sum(item => item.Calories),
            ingredients.Sum(item => item.ProteinGrams),
            ingredients.Length > 0 && ingredients.All(item => item.IsGlutenFree),
            ingredients.Length > 0 && ingredients.All(item => item.IsDiabeticFriendly),
            recipe.Servings,
            recipe.Tags,
            ingredients);
    }
}

public sealed record RecipeIngredientDto(
    Guid IngredientId,
    string Name,
    decimal Quantity,
    string Unit,
    decimal Calories,
    decimal ProteinGrams,
    bool IsGlutenFree,
    bool IsDiabeticFriendly)
{
    public static RecipeIngredientDto From(RecipeIngredientEntity recipeIngredient)
    {
        var ingredient = recipeIngredient.Ingredient ?? throw new InvalidOperationException("Recipe ingredient not loaded.");
        return new RecipeIngredientDto(
            ingredient.Id,
            ingredient.Name,
            recipeIngredient.Quantity,
            recipeIngredient.Unit,
            NutritionCalculator.CalculateByQuantity(ingredient.CaloriesPer100G, recipeIngredient.Quantity, recipeIngredient.Unit),
            NutritionCalculator.CalculateByQuantity(ingredient.ProteinPer100G, recipeIngredient.Quantity, recipeIngredient.Unit),
            ingredient.IsGlutenFree,
            ingredient.IsDiabeticFriendly);
    }
}

public sealed record RecipeCreateRequest(
    string Name,
    string MealType,
    string? Description,
    int Servings,
    string[] Tags,
    RecipeIngredientRequest[] Ingredients);

public sealed record RecipeUpdateRequest(
    string Name,
    string MealType,
    string? Description,
    int Servings,
    string[] Tags,
    RecipeIngredientRequest[] Ingredients);

public sealed record RecipeIngredientRequest(Guid IngredientId, decimal Quantity, string Unit);

public static class NutritionCalculator
{
    public static decimal CalculateByQuantity(decimal valuePer100G, decimal quantity, string unit)
    {
        return unit.Equals("g", StringComparison.OrdinalIgnoreCase)
            ? Math.Round(valuePer100G * quantity / 100, 2)
            : 0;
    }
}
