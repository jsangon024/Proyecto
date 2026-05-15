namespace MealPlanner.Api.Data.Entities;

public sealed class RecipeEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string MealType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Servings { get; set; }
    public string[] Tags { get; set; } = Array.Empty<string>();
    public DateTimeOffset CreatedAt { get; set; }
    public List<RecipeIngredientEntity> Ingredients { get; set; } = new();
}
