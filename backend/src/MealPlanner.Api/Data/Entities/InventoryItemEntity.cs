namespace MealPlanner.Api.Data.Entities;

public sealed class InventoryItemEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid IngredientId { get; set; }
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public DateTimeOffset UpdatedAt { get; set; }
    public IngredientEntity? Ingredient { get; set; }
}
