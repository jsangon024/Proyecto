namespace MealPlanner.Api.Data.Entities;

public sealed class UserEntity
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "user";
    public int DailyCalories { get; set; } = 2200;
    public int MinimumProteinGrams { get; set; } = 120;
    public Guid[] ExcludedIngredients { get; set; } = Array.Empty<Guid>();
    public bool IsEmailVerified { get; set; }
    public string? EmailVerificationTokenHash { get; set; }
    public DateTimeOffset? EmailVerificationTokenExpiresAt { get; set; }
    public string? PasswordResetTokenHash { get; set; }
    public DateTimeOffset? PasswordResetTokenExpiresAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
