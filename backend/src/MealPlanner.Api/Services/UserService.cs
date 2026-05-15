using MealPlanner.Api.Data;
using MealPlanner.Api.Data.Entities;
using MealPlanner.Api.Dtos;
using MealPlanner.Api.Models;

namespace MealPlanner.Api.Services;

public sealed class UserService
{
    private readonly MealPlannerDbContext _db;
    private readonly PasswordHasher _hasher;

    public UserService(MealPlannerDbContext db, PasswordHasher hasher)
    {
        _db = db;
        _hasher = hasher;
    }

    public UserEntity UpdateGoals(UserEntity user, NutritionGoals goals)
    {
        if (goals.DailyCalories <= 0 || goals.MinimumProteinGrams < 0)
        {
            throw new ValidationException("Los objetivos nutricionales no son validos.");
        }

        var storedUser = _db.Users.Find(user.Id) ?? throw new ValidationException("Usuario no encontrado.");
        storedUser.DailyCalories = goals.DailyCalories;
        storedUser.MinimumProteinGrams = goals.MinimumProteinGrams;
        storedUser.ExcludedIngredients = goals.ExcludedIngredients
            .Select(value => Guid.TryParse(value, out var id) ? id : Guid.Empty)
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToArray();

        _db.SaveChanges();
        return storedUser;
    }

    public void ChangePassword(UserEntity user, ChangePasswordRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CurrentPassword) ||
            string.IsNullOrWhiteSpace(request.NewPassword) ||
            string.IsNullOrWhiteSpace(request.ConfirmPassword))
        {
            throw new ValidationException("Todos los campos de password son obligatorios.");
        }

        if (request.NewPassword.Length < 8)
        {
            throw new ValidationException("La nueva password debe tener al menos 8 caracteres.");
        }

        if (request.NewPassword != request.ConfirmPassword)
        {
            throw new ValidationException("La confirmacion de password no coincide.");
        }

        var storedUser = _db.Users.Find(user.Id) ?? throw new ValidationException("Usuario no encontrado.");
        if (!_hasher.Verify(request.CurrentPassword, storedUser.PasswordHash))
        {
            throw new ValidationException("La password actual no es correcta.");
        }

        storedUser.PasswordHash = _hasher.Hash(request.NewPassword);
        _db.SaveChanges();
    }

    public void DeleteOwnAccount(UserEntity user)
    {
        var storedUser = _db.Users.Find(user.Id) ?? throw new ValidationException("Usuario no encontrado.");
        _db.Users.Remove(storedUser);
        _db.SaveChanges();
    }
}
