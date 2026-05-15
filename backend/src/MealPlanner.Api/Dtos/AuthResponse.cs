namespace MealPlanner.Api.Dtos;

public sealed record AuthResponse(string AccessToken, UserDto User);
