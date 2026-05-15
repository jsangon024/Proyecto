namespace MealPlanner.Api.Dtos;

public sealed record GeneratePlanRequest(int Year, int Month);

public sealed record ShoppingListRequest(Guid PlanId);

public sealed record MealPlanDto(Guid Id, Guid UserId, int Year, int Month, DateTimeOffset CreatedAtUtc, MealPlanDayDto[] Days);

public sealed record MealPlanDayDto(string Date, PlannedMealDto[] Meals);

public sealed record PlannedMealDto(string MealType, Guid RecipeId, string RecipeName, decimal Calories, decimal ProteinGrams);

public sealed record ShoppingListItemDto(Guid IngredientId, string Name, decimal Quantity, string Unit);
