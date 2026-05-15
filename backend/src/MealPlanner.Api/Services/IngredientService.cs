using MealPlanner.Api.Data;
using MealPlanner.Api.Data.Entities;
using MealPlanner.Api.Dtos;
using Microsoft.EntityFrameworkCore;

namespace MealPlanner.Api.Services;

public sealed class IngredientService
{
    private readonly MealPlannerDbContext _db;

    public IngredientService(MealPlannerDbContext db)
    {
        _db = db;
    }

    public IReadOnlyCollection<IngredientEntity> GetAll()
    {
        return _db.Ingredients.AsNoTracking().OrderBy(ingredient => ingredient.Name).ToArray();
    }

    public IngredientEntity? GetById(Guid id)
    {
        return _db.Ingredients.AsNoTracking().FirstOrDefault(ingredient => ingredient.Id == id);
    }

    public IngredientEntity Create(IngredientCreateRequest request)
    {
        Validate(request.Name, request.Category, request.CaloriesPer100G, request.ProteinPer100G, request.GlycemicIndex);

        var ingredient = new IngredientEntity
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim().ToLowerInvariant(),
            Category = request.Category.Trim().ToLowerInvariant(),
            CaloriesPer100G = request.CaloriesPer100G,
            ProteinPer100G = request.ProteinPer100G,
            CarbsPer100G = request.CarbsPer100G,
            SugarsPer100G = request.SugarsPer100G,
            FatPer100G = request.FatPer100G,
            FiberPer100G = request.FiberPer100G,
            IsGlutenFree = request.IsGlutenFree,
            IsDiabeticFriendly = request.IsDiabeticFriendly,
            GlycemicIndex = request.GlycemicIndex,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _db.Ingredients.Add(ingredient);
        _db.SaveChanges();
        return ingredient;
    }

    public IngredientEntity? Update(Guid id, IngredientUpdateRequest request)
    {
        var ingredient = _db.Ingredients.Find(id);
        if (ingredient is null)
        {
            return null;
        }

        Validate(request.Name, request.Category, request.CaloriesPer100G, request.ProteinPer100G, request.GlycemicIndex);

        ingredient.Name = request.Name.Trim().ToLowerInvariant();
        ingredient.Category = request.Category.Trim().ToLowerInvariant();
        ingredient.CaloriesPer100G = request.CaloriesPer100G;
        ingredient.ProteinPer100G = request.ProteinPer100G;
        ingredient.CarbsPer100G = request.CarbsPer100G;
        ingredient.SugarsPer100G = request.SugarsPer100G;
        ingredient.FatPer100G = request.FatPer100G;
        ingredient.FiberPer100G = request.FiberPer100G;
        ingredient.IsGlutenFree = request.IsGlutenFree;
        ingredient.IsDiabeticFriendly = request.IsDiabeticFriendly;
        ingredient.GlycemicIndex = request.GlycemicIndex;

        _db.SaveChanges();
        return ingredient;
    }

    public bool Delete(Guid id)
    {
        var ingredient = _db.Ingredients.Find(id);
        if (ingredient is null)
        {
            return false;
        }

        _db.Ingredients.Remove(ingredient);
        _db.SaveChanges();
        return true;
    }

    private static void Validate(string name, string category, decimal calories, decimal protein, int? glycemicIndex)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(category) || calories < 0 || protein < 0)
        {
            throw new ValidationException("El ingrediente necesita nombre, categoria y valores nutricionales validos.");
        }

        if (glycemicIndex is < 0 or > 100)
        {
            throw new ValidationException("El indice glucemico debe estar entre 0 y 100.");
        }
    }
}
