namespace MealPlanner.Api.Data.Entities;

public sealed class MealPlanDayEntity
{
    public Guid Id { get; set; }
    public Guid MealPlanId { get; set; }
    public DateOnly PlanDate { get; set; }
    public bool IsCompleted { get; set; }
    public MealPlanEntity? MealPlan { get; set; }
    public List<PlannedMealEntity> Meals { get; set; } = new();
}
