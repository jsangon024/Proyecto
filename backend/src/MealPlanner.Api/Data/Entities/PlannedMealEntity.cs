namespace MealPlanner.Api.Data.Entities;

public sealed class PlannedMealEntity
{
    public Guid Id { get; set; }
    public Guid MealPlanDayId { get; set; }
    public Guid RecipeId { get; set; }
    public string MealType { get; set; } = string.Empty;
    public MealPlanDayEntity? MealPlanDay { get; set; }
    public RecipeEntity? Recipe { get; set; }
}
