using MealPlanner.Api.Data;
using MealPlanner.Api.Data.Entities;
using MealPlanner.Api.Dtos;
using Microsoft.EntityFrameworkCore;

namespace MealPlanner.Api.Services;

public sealed class AdminService
{
    public const string AdminRole = "admin";
    public const string UserRole = "user";

    private static readonly HashSet<string> AllowedRoles = new(StringComparer.OrdinalIgnoreCase)
    {
        AdminRole,
        UserRole
    };

    private readonly MealPlannerDbContext _db;
    private readonly PasswordHasher _hasher;

    public AdminService(MealPlannerDbContext db, PasswordHasher hasher)
    {
        _db = db;
        _hasher = hasher;
    }

    public IReadOnlyCollection<UserEntity> GetUsers()
    {
        return _db.Users.AsNoTracking().OrderBy(user => user.Email).ToArray();
    }

    public UserEntity? GetUser(Guid id)
    {
        return _db.Users.AsNoTracking().FirstOrDefault(user => user.Id == id);
    }

    public UserEntity? UpdateRole(Guid targetUserId, string role)
    {
        var normalizedRole = NormalizeRole(role);
        if (!AllowedRoles.Contains(normalizedRole))
        {
            throw new ValidationException("El rol debe ser 'admin' o 'user'.");
        }

        var user = _db.Users.Find(targetUserId);
        if (user is null)
        {
            return null;
        }

        user.Role = normalizedRole;
        _db.SaveChanges();
        return user;
    }

    public bool DeleteUser(Guid targetUserId)
    {
        var user = _db.Users.Find(targetUserId);
        if (user is null)
        {
            return false;
        }

        _db.Users.Remove(user);
        _db.SaveChanges();
        return true;
    }

    public bool ChangeUserPassword(Guid targetUserId, AdminChangePasswordRequest request)
    {
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

        var user = _db.Users.Find(targetUserId);
        if (user is null)
        {
            return false;
        }

        if (user.Role == AdminRole)
        {
            throw new ValidationException("No se puede cambiar la password de otro administrador.");
        }

        user.PasswordHash = _hasher.Hash(request.NewPassword);
        _db.SaveChanges();
        return true;
    }

    private static string NormalizeRole(string role)
    {
        return (role ?? string.Empty).Trim().ToLowerInvariant();
    }
}
