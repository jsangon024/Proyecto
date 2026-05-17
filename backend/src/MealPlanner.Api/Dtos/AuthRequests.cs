namespace MealPlanner.Api.Dtos;

public sealed record RegisterRequest(string Email, string Password, string ConfirmPassword);

public sealed record LoginRequest(string Email, string Password);

public sealed record AuthMessageResponse(string Message);

public sealed record VerifyEmailRequest(string Token);

public sealed record ForgotPasswordRequest(string Email);

public sealed record ResetPasswordRequest(string Token, string NewPassword, string ConfirmPassword);

public sealed record DeleteAccountRequest(string Password);

public sealed record AdminChangePasswordRequest(string NewPassword, string ConfirmPassword);
