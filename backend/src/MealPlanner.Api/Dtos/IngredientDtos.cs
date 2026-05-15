using MealPlanner.Api.Data.Entities;

namespace MealPlanner.Api.Dtos;

public sealed record IngredientDto(
    Guid Id,
    string Name,
    string Category,
    decimal CaloriesPer100G,
    decimal ProteinPer100G,
    decimal CarbsPer100G,
    decimal SugarsPer100G,
    decimal FatPer100G,
    decimal FiberPer100G,
    bool IsGlutenFree,
    bool IsDiabeticFriendly,
    int? GlycemicIndex)
{
    public static IngredientDto From(IngredientEntity ingredient)
    {
        return new IngredientDto(
            ingredient.Id,
            ingredient.Name,
            ingredient.Category,
            ingredient.CaloriesPer100G,
            ingredient.ProteinPer100G,
            ingredient.CarbsPer100G,
            ingredient.SugarsPer100G,
            ingredient.FatPer100G,
            ingredient.FiberPer100G,
            ingredient.IsGlutenFree,
            ingredient.IsDiabeticFriendly,
            ingredient.GlycemicIndex);
    }
}

public sealed record IngredientCreateRequest(
    string Name,
    string Category,
    decimal CaloriesPer100G,
    decimal ProteinPer100G,
    decimal CarbsPer100G,
    decimal SugarsPer100G,
    decimal FatPer100G,
    decimal FiberPer100G,
    bool IsGlutenFree,
    bool IsDiabeticFriendly,
    int? GlycemicIndex);

public sealed record IngredientUpdateRequest(
    string Name,
    string Category,
    decimal CaloriesPer100G,
    decimal ProteinPer100G,
    decimal CarbsPer100G,
    decimal SugarsPer100G,
    decimal FatPer100G,
    decimal FiberPer100G,
    bool IsGlutenFree,
    bool IsDiabeticFriendly,
    int? GlycemicIndex);
