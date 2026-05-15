using MealPlanner.Api.Data;
using MealPlanner.Api.Data.Entities;
using MealPlanner.Api.Dtos;
using Microsoft.EntityFrameworkCore;

namespace MealPlanner.Api.Services;

public sealed class RecipeService
{
    private readonly MealPlannerDbContext _db;

    public RecipeService(MealPlannerDbContext db)
    {
        _db = db;
    }

    public IReadOnlyCollection<RecipeEntity> GetAll()
    {
        return RecipeQuery().OrderBy(recipe => recipe.Name).ToArray();
    }

    public RecipeEntity? GetById(Guid id)
    {
        return RecipeQuery().FirstOrDefault(recipe => recipe.Id == id);
    }

    public IReadOnlyCollection<RecipeEntity> GetSavedByUser(Guid userId)
    {
        var recipeIds = _db.UserSavedRecipes
            .Where(savedRecipe => savedRecipe.UserId == userId)
            .Select(savedRecipe => savedRecipe.RecipeId)
            .ToArray();

        return RecipeQuery()
            .Where(recipe => recipeIds.Contains(recipe.Id))
            .OrderBy(recipe => recipe.Name)
            .ToArray();
    }

    public bool SaveForUser(Guid userId, Guid recipeId)
    {
        if (!_db.Recipes.Any(recipe => recipe.Id == recipeId))
        {
            return false;
        }

        if (_db.UserSavedRecipes.Any(savedRecipe => savedRecipe.UserId == userId && savedRecipe.RecipeId == recipeId))
        {
            return true;
        }

        _db.UserSavedRecipes.Add(new UserSavedRecipeEntity
        {
            UserId = userId,
            RecipeId = recipeId,
            SavedAt = DateTimeOffset.UtcNow
        });
        _db.SaveChanges();
        return true;
    }

    public bool UnsaveForUser(Guid userId, Guid recipeId)
    {
        var savedRecipe = _db.UserSavedRecipes.Find(userId, recipeId);
        if (savedRecipe is null)
        {
            return false;
        }

        _db.UserSavedRecipes.Remove(savedRecipe);
        _db.SaveChanges();
        return true;
    }

    public RecipeEntity Create(RecipeCreateRequest request)
    {
        ValidateRecipe(request.Name, request.MealType, request.Servings, request.Ingredients);

        var recipe = new RecipeEntity
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            MealType = request.MealType.Trim().ToLowerInvariant(),
            Description = request.Description?.Trim(),
            Servings = request.Servings,
            Tags = NormalizeTags(request.Tags),
            CreatedAt = DateTimeOffset.UtcNow,
            Ingredients = request.Ingredients.Select(ToRecipeIngredient).ToList()
        };

        _db.Recipes.Add(recipe);
        _db.SaveChanges();
        return GetById(recipe.Id) ?? recipe;
    }

    public RecipeEntity? Update(Guid id, RecipeUpdateRequest request)
    {
        var recipe = _db.Recipes
            .Include(item => item.Ingredients)
            .FirstOrDefault(item => item.Id == id);

        if (recipe is null)
        {
            return null;
        }

        ValidateRecipe(request.Name, request.MealType, request.Servings, request.Ingredients);

        recipe.Name = request.Name.Trim();
        recipe.MealType = request.MealType.Trim().ToLowerInvariant();
        recipe.Description = request.Description?.Trim();
        recipe.Servings = request.Servings;
        recipe.Tags = NormalizeTags(request.Tags);

        _db.RecipeIngredients.RemoveRange(recipe.Ingredients);
        recipe.Ingredients = request.Ingredients.Select(ToRecipeIngredient).ToList();

        _db.SaveChanges();
        return GetById(id);
    }

    public bool Delete(Guid id)
    {
        var recipe = _db.Recipes.Find(id);
        if (recipe is null)
        {
            return false;
        }

        _db.Recipes.Remove(recipe);
        _db.SaveChanges();
        return true;
    }

    private IQueryable<RecipeEntity> RecipeQuery()
    {
        return _db.Recipes
            .AsNoTracking()
            .Include(recipe => recipe.Ingredients)
            .ThenInclude(recipeIngredient => recipeIngredient.Ingredient);
    }

    private RecipeIngredientEntity ToRecipeIngredient(RecipeIngredientRequest request)
    {
        if (!_db.Ingredients.Any(ingredient => ingredient.Id == request.IngredientId))
        {
            throw new ValidationException("Uno de los ingredientes indicados no existe.");
        }

        return new RecipeIngredientEntity
        {
            Id = Guid.NewGuid(),
            IngredientId = request.IngredientId,
            Quantity = request.Quantity,
            Unit = request.Unit.Trim().ToLowerInvariant()
        };
    }

    private static void ValidateRecipe(string name, string mealType, int servings, IReadOnlyCollection<RecipeIngredientRequest> ingredients)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(mealType) || servings <= 0)
        {
            throw new ValidationException("La receta necesita nombre, tipo de comida y raciones validas.");
        }

        if (ingredients.Count == 0 || ingredients.Any(ingredient => ingredient.IngredientId == Guid.Empty || ingredient.Quantity <= 0 || string.IsNullOrWhiteSpace(ingredient.Unit)))
        {
            throw new ValidationException("La receta necesita al menos un ingrediente valido.");
        }
    }

    private static string[] NormalizeTags(IEnumerable<string> tags)
    {
        return tags.Select(tag => tag.Trim().ToLowerInvariant()).Where(tag => tag.Length > 0).Distinct().ToArray();
    }
}
