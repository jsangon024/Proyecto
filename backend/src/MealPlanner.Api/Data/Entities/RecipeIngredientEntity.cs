namespace MealPlanner.Api.Data.Entities;

public sealed class RecipeIngredientEntity
{
    public Guid Id { get; set; }
    public Guid RecipeId { get; set; }
    public Guid IngredientId { get; set; }
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public RecipeEntity? Recipe { get; set; }
    public IngredientEntity? Ingredient { get; set; }
}
