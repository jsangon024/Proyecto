using MealPlanner.Api.Data.Entities;
using MealPlanner.Api.Models;

namespace MealPlanner.Api.Dtos;

public sealed record UserDto(Guid Id, string Email, string Role, NutritionGoals Goals)
{
    public static UserDto From(UserEntity user)
    {
        return new UserDto(
            user.Id,
            user.Email,
            user.Role,
            new NutritionGoals(user.DailyCalories, user.MinimumProteinGrams, user.ExcludedIngredients.Select(id => id.ToString()).ToArray()));
    }
}
