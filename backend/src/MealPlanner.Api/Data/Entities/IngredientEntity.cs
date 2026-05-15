namespace MealPlanner.Api.Data.Entities;

public sealed class IngredientEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal CaloriesPer100G { get; set; }
    public decimal ProteinPer100G { get; set; }
    public decimal CarbsPer100G { get; set; }
    public decimal SugarsPer100G { get; set; }
    public decimal FatPer100G { get; set; }
    public decimal FiberPer100G { get; set; }
    public bool IsGlutenFree { get; set; }
    public bool IsDiabeticFriendly { get; set; }
    public int? GlycemicIndex { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
