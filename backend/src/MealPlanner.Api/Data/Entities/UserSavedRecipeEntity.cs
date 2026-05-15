namespace MealPlanner.Api.Data.Entities;

public sealed class UserSavedRecipeEntity
{
    public Guid UserId { get; set; }
    public Guid RecipeId { get; set; }
    public DateTimeOffset SavedAt { get; set; }
    public UserEntity? User { get; set; }
    public RecipeEntity? Recipe { get; set; }
}
