namespace MealPlanner.Api.Dtos;

public sealed record RegisterRequest(string Email, string Password, string ConfirmPassword);

public sealed record LoginRequest(string Email, string Password);
