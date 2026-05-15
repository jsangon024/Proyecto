namespace MealPlanner.Api.Models;

public sealed record AuthSession(Guid UserId, DateTimeOffset ExpiresAtUtc);
