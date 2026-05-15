namespace MealPlanner.Api.Dtos;

public sealed record ChangePasswordRequest(string CurrentPassword, string NewPassword, string ConfirmPassword);
