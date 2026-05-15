using MealPlanner.Api.Data;
using MealPlanner.Api.Data.Entities;
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

    public AdminService(MealPlannerDbContext db)
    {
        _db = db;
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

    private static string NormalizeRole(string role)
    {
        return (role ?? string.Empty).Trim().ToLowerInvariant();
    }
}
