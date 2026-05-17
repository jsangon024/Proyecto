using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using MealPlanner.Api.Data;
using MealPlanner.Api.Data.Entities;

namespace MealPlanner.Api.Services;

public sealed class TokenService
{
    private readonly MealPlannerDbContext _db;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TokenService> _logger;

    public TokenService(MealPlannerDbContext db, IConfiguration configuration, ILogger<TokenService> logger)
    {
        _db = db;
        _configuration = configuration;
        _logger = logger;
    }

    public string Create(UserEntity user)
    {
        var payload = new TokenPayload(user.Id, DateTimeOffset.UtcNow.AddHours(8).ToUnixTimeSeconds());
        var payloadJson = JsonSerializer.Serialize(payload);
        var payloadPart = Base64UrlEncode(Encoding.UTF8.GetBytes(payloadJson));
        var signaturePart = Base64UrlEncode(Sign(payloadPart));

        return $"{payloadPart}.{signaturePart}";
    }

    public UserEntity? Validate(string token)
    {
        var parts = token.Split('.', 2);
        if (parts.Length != 2)
        {
            return null;
        }

        var expectedSignature = Base64UrlEncode(Sign(parts[0]));
        if (!CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(parts[1]),
            Encoding.UTF8.GetBytes(expectedSignature)))
        {
            return null;
        }

        TokenPayload? payload;
        try
        {
            payload = JsonSerializer.Deserialize<TokenPayload>(Encoding.UTF8.GetString(Base64UrlDecode(parts[0])));
        }
        catch
        {
            return null;
        }

        if (payload is null || payload.ExpiresAtUnix <= DateTimeOffset.UtcNow.ToUnixTimeSeconds())
        {
            return null;
        }

        return _db.Users.Find(payload.UserId);
    }

    private byte[] Sign(string payloadPart)
    {
        var signingKey = _configuration["TOKEN_SIGNING_KEY"];
        if (string.IsNullOrWhiteSpace(signingKey))
        {
            signingKey = "development-only-token-signing-key-change-me";
            _logger.LogWarning("TOKEN_SIGNING_KEY no esta configurada. Usando clave de desarrollo.");
        }

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(signingKey));
        return hmac.ComputeHash(Encoding.UTF8.GetBytes(payloadPart));
    }

    private static string Base64UrlEncode(byte[] bytes)
    {
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private static byte[] Base64UrlDecode(string value)
    {
        var padded = value.Replace('-', '+').Replace('_', '/');
        padded = padded.PadRight(padded.Length + ((4 - padded.Length % 4) % 4), '=');
        return Convert.FromBase64String(padded);
    }

    private sealed record TokenPayload(Guid UserId, long ExpiresAtUnix);
}
