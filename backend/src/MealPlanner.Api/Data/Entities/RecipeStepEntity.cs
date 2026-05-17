namespace MealPlanner.Api.Data.Entities;

public sealed class RecipeStepEntity
{
    public Guid Id { get; set; }
    public Guid RecipeId { get; set; }
    public int StepNumber { get; set; }
    public string Description { get; set; } = string.Empty;
    public RecipeEntity? Recipe { get; set; }
}
