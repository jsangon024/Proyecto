namespace MealPlanner.Api.Data.Entities;

public sealed class MealPlanEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public List<MealPlanDayEntity> Days { get; set; } = new();
}
