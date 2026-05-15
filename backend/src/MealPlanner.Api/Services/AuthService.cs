using MealPlanner.Api.Data;
using MealPlanner.Api.Data.Entities;
using MealPlanner.Api.Dtos;
using Microsoft.EntityFrameworkCore;

namespace MealPlanner.Api.Services;

public sealed class AuthService
{
    private readonly MealPlannerDbContext _db;
    private readonly PasswordHasher _hasher;
    private readonly TokenService _tokens;

    public AuthService(MealPlannerDbContext db, PasswordHasher hasher, TokenService tokens)
    {
        _db = db;
        _hasher = hasher;
        _tokens = tokens;
    }

    public AuthResponse Register(RegisterRequest request)
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

        if (_db.Users.Any(user => user.Email == email))
        {
            throw new ConflictException("Ya existe un usuario con ese email.");
        }

        var user = new UserEntity
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = _hasher.Hash(request.Password),
            Role = "user",
            DailyCalories = 2200,
            MinimumProteinGrams = 120,
            ExcludedIngredients = Array.Empty<Guid>(),
            CreatedAt = DateTimeOffset.UtcNow
        };

        _db.Users.Add(user);
        _db.SaveChanges();
        return new AuthResponse(_tokens.Create(user), UserDto.From(user));
    }

    public AuthResponse? Login(LoginRequest request)
    {
        var email = NormalizeEmail(request.Email);
        var user = _db.Users.AsNoTracking().FirstOrDefault(candidate => candidate.Email == email);
        if (user is null || !_hasher.Verify(request.Password, user.PasswordHash))
        {
            return null;
        }

        return new AuthResponse(_tokens.Create(user), UserDto.From(user));
    }

    private static string NormalizeEmail(string? email)
    {
        return (email ?? string.Empty).Trim().ToLowerInvariant();
    }
}
