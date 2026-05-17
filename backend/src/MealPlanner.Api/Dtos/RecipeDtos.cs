using MealPlanner.Api.Data.Entities;
using MealPlanner.Api.Services;

namespace MealPlanner.Api.Dtos;

public sealed record RecipeDto(
    Guid Id,
    Guid OwnerId,
    string Name,
    string MealType,
    string? Description,
    decimal Calories,
    decimal ProteinGrams,
    bool IsGlutenFree,
    bool IsDiabeticFriendly,
    int Servings,
    string[] Tags,
    RecipeIngredientDto[] Ingredients,
    RecipeStepDto[] Steps,
    bool IsGlobal)
{
    public static RecipeDto From(RecipeEntity recipe)
    {
        var ingredients = recipe.Ingredients
            .Where(item => item.Ingredient is not null)
            .Select(RecipeIngredientDto.From)
            .ToArray();

        return new RecipeDto(
            recipe.Id,
            recipe.OwnerId,
            recipe.Name,
            recipe.MealType,
            recipe.Description,
            ingredients.Sum(item => item.Calories),
            ingredients.Sum(item => item.ProteinGrams),
            ingredients.Length > 0 && ingredients.All(item => item.IsGlutenFree),
            ingredients.Length > 0 && ingredients.All(item => item.IsDiabeticFriendly),
            recipe.Servings,
            recipe.Tags,
            ingredients,
            recipe.Steps
                .OrderBy(step => step.StepNumber)
                .Select(RecipeStepDto.From)
                .ToArray(),
            recipe.OwnerId == RecipeService.GlobalOwnerId);
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
    RecipeIngredientRequest[] Ingredients,
    RecipeStepRequest[] Steps);

public sealed record RecipeUpdateRequest(
    string Name,
    string MealType,
    string? Description,
    int Servings,
    string[] Tags,
    RecipeIngredientRequest[] Ingredients,
    RecipeStepRequest[] Steps);

public sealed record RecipeIngredientRequest(Guid IngredientId, decimal Quantity, string Unit);

public sealed record RecipeStepDto(int StepNumber, string Description)
{
    public static RecipeStepDto From(RecipeStepEntity step)
    {
        return new RecipeStepDto(step.StepNumber, step.Description);
    }
}

public sealed record RecipeStepRequest(string Description);

public static class NutritionCalculator
{
    public static decimal CalculateByQuantity(decimal valuePer100G, decimal quantity, string unit)
    {
        return unit.Equals("g", StringComparison.OrdinalIgnoreCase)
            ? Math.Round(valuePer100G * quantity / 100, 2)
            : 0;
    }
}
