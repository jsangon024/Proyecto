using MealPlanner.Api.Data;
using MealPlanner.Api.Data.Entities;
using MealPlanner.Api.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace MealPlanner.Api.Services;

public sealed class AuthService
{
    private readonly MealPlannerDbContext _db;
    private readonly PasswordHasher _hasher;
    private readonly TokenService _tokens;
    private readonly EmailService _email;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        MealPlannerDbContext db,
        PasswordHasher hasher,
        TokenService tokens,
        EmailService email,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _db = db;
        _hasher = hasher;
        _tokens = tokens;
        _email = email;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<AuthMessageResponse> RegisterAsync(RegisterRequest request)
    {
        var email = NormalizeEmail(request.Email);
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ValidationException("Email y password son obligatorios.");
        }

        if (request.Password.Length < 8)
        {
            throw new ValidationException("La password debe tener al menos 8 caracteres.");
        }

        if (request.Password != request.ConfirmPassword)
        {
            throw new ValidationException("La confirmacion de password no coincide.");
        }

        var existingUser = _db.Users.FirstOrDefault(user => user.Email == email);
        if (existingUser is not null)
        {
            if (existingUser.IsEmailVerified)
            {
                throw new ConflictException("Ya existe un usuario con ese email.");
            }

            var retryToken = CreateSecureToken();
            existingUser.PasswordHash = _hasher.Hash(request.Password);
            existingUser.EmailVerificationTokenHash = HashToken(retryToken);
            existingUser.EmailVerificationTokenExpiresAt = DateTimeOffset.UtcNow.AddHours(24);
            _db.SaveChanges();

            await SendVerificationEmailOrFailAsync(existingUser.Email, retryToken, removeUserOnFailure: null);
            return new AuthMessageResponse("La cuenta estaba pendiente de activar. Hemos enviado un nuevo correo de verificacion.");
        }

        var verificationToken = CreateSecureToken();
        var user = new UserEntity
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = _hasher.Hash(request.Password),
            Role = "user",
            DailyCalories = 2200,
            MinimumProteinGrams = 120,
            ExcludedIngredients = Array.Empty<Guid>(),
            IsEmailVerified = false,
            EmailVerificationTokenHash = HashToken(verificationToken),
            EmailVerificationTokenExpiresAt = DateTimeOffset.UtcNow.AddHours(24),
            CreatedAt = DateTimeOffset.UtcNow
        };

        _db.Users.Add(user);
        _db.SaveChanges();
        await SendVerificationEmailOrFailAsync(email, verificationToken, user);
        return new AuthMessageResponse("Cuenta creada. Revisa tu correo para activar la cuenta antes de iniciar sesion.");
    }

    public AuthResponse? Login(LoginRequest request)
    {
        var email = NormalizeEmail(request.Email);
        var user = _db.Users.AsNoTracking().FirstOrDefault(candidate => candidate.Email == email);
        if (user is null || !_hasher.Verify(request.Password, user.PasswordHash))
        {
            return null;
        }

        if (!user.IsEmailVerified)
        {
            throw new ValidationException("Debes verificar tu email antes de iniciar sesion.");
        }

        return new AuthResponse(_tokens.Create(user), UserDto.From(user));
    }

    public AuthMessageResponse VerifyEmail(VerifyEmailRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Token))
        {
            throw new ValidationException("El enlace de activacion no es valido o ha caducado.");
        }

        var tokenHash = HashToken(request.Token);
        var user = _db.Users.FirstOrDefault(candidate => candidate.EmailVerificationTokenHash == tokenHash);
        if (user is null || user.EmailVerificationTokenExpiresAt < DateTimeOffset.UtcNow)
        {
            throw new ValidationException("El enlace de activacion no es valido o ha caducado.");
        }

        user.IsEmailVerified = true;
        user.EmailVerificationTokenHash = null;
        user.EmailVerificationTokenExpiresAt = null;
        _db.SaveChanges();

        return new AuthMessageResponse("Cuenta activada. Ya puedes iniciar sesion.");
    }

    public async Task<AuthMessageResponse> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        var email = NormalizeEmail(request.Email);
        var user = _db.Users.FirstOrDefault(candidate => candidate.Email == email);
        if (user is not null && user.IsEmailVerified)
        {
            var resetToken = CreateSecureToken();
            user.PasswordResetTokenHash = HashToken(resetToken);
            user.PasswordResetTokenExpiresAt = DateTimeOffset.UtcNow.AddHours(1);
            _db.SaveChanges();

            try
            {
                await _email.SendPasswordResetAsync(email, BuildFrontendLink("reset-password", resetToken));
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "No se pudo enviar el correo de recuperacion a {Email}", email);
                user.PasswordResetTokenHash = null;
                user.PasswordResetTokenExpiresAt = null;
                _db.SaveChanges();
                throw new ValidationException("No se pudo enviar el correo de recuperacion. Revisa la configuracion SMTP.");
            }
        }

        return new AuthMessageResponse("Si existe una cuenta verificada con ese email, recibiras un enlace para cambiar la password.");
    }

    public AuthMessageResponse ResetPassword(ResetPasswordRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Token))
        {
            throw new ValidationException("El enlace de recuperacion no es valido o ha caducado.");
        }

        if (string.IsNullOrWhiteSpace(request.NewPassword) || string.IsNullOrWhiteSpace(request.ConfirmPassword))
        {
            throw new ValidationException("La nueva password y su confirmacion son obligatorias.");
        }

        if (request.NewPassword.Length < 8)
        {
            throw new ValidationException("La nueva password debe tener al menos 8 caracteres.");
        }

        if (request.NewPassword != request.ConfirmPassword)
        {
            throw new ValidationException("La confirmacion de password no coincide.");
        }

        var tokenHash = HashToken(request.Token);
        var user = _db.Users.FirstOrDefault(candidate => candidate.PasswordResetTokenHash == tokenHash);
        if (user is null || user.PasswordResetTokenExpiresAt < DateTimeOffset.UtcNow)
        {
            throw new ValidationException("El enlace de recuperacion no es valido o ha caducado.");
        }

        user.PasswordHash = _hasher.Hash(request.NewPassword);
        user.PasswordResetTokenHash = null;
        user.PasswordResetTokenExpiresAt = null;
        _db.SaveChanges();

        return new AuthMessageResponse("Password actualizada. Ya puedes iniciar sesion.");
    }

    private static string NormalizeEmail(string? email)
    {
        return (email ?? string.Empty).Trim().ToLowerInvariant();
    }

    private string BuildFrontendLink(string page, string token)
    {
        var baseUrl = (_configuration["FRONTEND_BASE_URL"] ?? "http://localhost:5173").TrimEnd('/');
        return $"{baseUrl}/#/{page}/{token}";
    }

    private async Task SendVerificationEmailOrFailAsync(string email, string token, UserEntity? removeUserOnFailure)
    {
        try
        {
            await _email.SendAccountVerificationAsync(email, BuildFrontendLink("verify-email", token));
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "No se pudo enviar el correo de activacion a {Email}", email);

            if (removeUserOnFailure is not null)
            {
                _db.Users.Remove(removeUserOnFailure);
                _db.SaveChanges();
            }

            throw new ValidationException("No se pudo enviar el correo de activacion. Revisa la configuracion SMTP.");
        }
    }

    private static string CreateSecureToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
    }

    private static string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes);
    }
}
