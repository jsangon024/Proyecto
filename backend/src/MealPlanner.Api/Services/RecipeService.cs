using MealPlanner.Api.Data;
using MealPlanner.Api.Data.Entities;
using MealPlanner.Api.Dtos;
using Microsoft.EntityFrameworkCore;

namespace MealPlanner.Api.Services;

public sealed class RecipeService
{
    public static readonly Guid GlobalOwnerId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private readonly MealPlannerDbContext _db;

    public RecipeService(MealPlannerDbContext db)
    {
        _db = db;
    }

    public IReadOnlyCollection<RecipeEntity> GetAll(UserEntity user)
    {
        return VisibleRecipeQuery(user).OrderBy(recipe => recipe.Name).ToArray();
    }

    public RecipeEntity? GetById(UserEntity user, Guid id)
    {
        return VisibleRecipeQuery(user).FirstOrDefault(recipe => recipe.Id == id);
    }

    public IReadOnlyCollection<RecipeEntity> GetSavedByUser(UserEntity user)
    {
        var recipeIds = _db.UserSavedRecipes
            .Where(savedRecipe => savedRecipe.UserId == user.Id)
            .Select(savedRecipe => savedRecipe.RecipeId)
            .ToArray();

        return VisibleRecipeQuery(user)
            .Where(recipe => recipeIds.Contains(recipe.Id))
            .OrderBy(recipe => recipe.Name)
            .ToArray();
    }

    public bool SaveForUser(UserEntity user, Guid recipeId)
    {
        if (!VisibleRecipeQuery(user).Any(recipe => recipe.Id == recipeId))
        {
            return false;
        }

        if (_db.UserSavedRecipes.Any(savedRecipe => savedRecipe.UserId == user.Id && savedRecipe.RecipeId == recipeId))
        {
            return true;
        }

        _db.UserSavedRecipes.Add(new UserSavedRecipeEntity
        {
            UserId = user.Id,
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

    public RecipeEntity Create(UserEntity user, RecipeCreateRequest request)
    {
        ValidateRecipe(request.Name, request.MealType, request.Servings, request.Ingredients, request.Steps);

        var recipe = new RecipeEntity
        {
            Id = Guid.NewGuid(),
            OwnerId = user.Role == "admin" ? GlobalOwnerId : user.Id,
            Name = request.Name.Trim(),
            MealType = request.MealType.Trim().ToLowerInvariant(),
            Description = request.Description?.Trim(),
            Servings = request.Servings,
            Tags = NormalizeTags(request.Tags),
            CreatedAt = DateTimeOffset.UtcNow,
            Ingredients = request.Ingredients.Select(ToRecipeIngredient).ToList(),
            Steps = ToRecipeSteps(request.Steps)
        };

        _db.Recipes.Add(recipe);
        _db.SaveChanges();
        return GetById(user, recipe.Id) ?? recipe;
    }

    public RecipeEntity? Update(UserEntity user, Guid id, RecipeUpdateRequest request)
    {
        var recipe = _db.Recipes
            .Include(item => item.Ingredients)
            .Include(item => item.Steps)
            .FirstOrDefault(item => item.Id == id);

        if (recipe is null || !CanManage(user, recipe))
        {
            return null;
        }

        ValidateRecipe(request.Name, request.MealType, request.Servings, request.Ingredients, request.Steps);

        recipe.Name = request.Name.Trim();
        recipe.MealType = request.MealType.Trim().ToLowerInvariant();
        recipe.Description = request.Description?.Trim();
        recipe.Servings = request.Servings;
        recipe.Tags = NormalizeTags(request.Tags);

        _db.RecipeIngredients.RemoveRange(recipe.Ingredients);
        _db.RecipeSteps.RemoveRange(recipe.Steps);
        recipe.Ingredients = request.Ingredients.Select(ToRecipeIngredient).ToList();
        recipe.Steps = ToRecipeSteps(request.Steps);

        _db.SaveChanges();
        return GetById(user, id);
    }

    public bool Delete(UserEntity user, Guid id)
    {
        var recipe = _db.Recipes.Find(id);
        if (recipe is null || !CanManage(user, recipe))
        {
            return false;
        }

        if (_db.PlannedMeals.Any(meal => meal.RecipeId == id))
        {
            throw new ValidationException("No se puede eliminar una receta que ya aparece en un plan mensual.");
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
            .ThenInclude(recipeIngredient => recipeIngredient.Ingredient)
            .Include(recipe => recipe.Steps);
    }

    private IQueryable<RecipeEntity> VisibleRecipeQuery(UserEntity user)
    {
        return RecipeQuery().Where(recipe => recipe.OwnerId == GlobalOwnerId || recipe.OwnerId == user.Id);
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

    private static List<RecipeStepEntity> ToRecipeSteps(IReadOnlyCollection<RecipeStepRequest> steps)
    {
        return steps
            .Select((step, index) => new RecipeStepEntity
            {
                Id = Guid.NewGuid(),
                StepNumber = index + 1,
                Description = step.Description.Trim()
            })
            .ToList();
    }

    private static void ValidateRecipe(
        string name,
        string mealType,
        int servings,
        IReadOnlyCollection<RecipeIngredientRequest> ingredients,
        IReadOnlyCollection<RecipeStepRequest> steps)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(mealType) || servings <= 0)
        {
            throw new ValidationException("La receta necesita nombre, tipo de comida y raciones validas.");
        }

        if (ingredients.Count == 0 || ingredients.Any(ingredient => ingredient.IngredientId == Guid.Empty || ingredient.Quantity <= 0 || string.IsNullOrWhiteSpace(ingredient.Unit)))
        {
            throw new ValidationException("La receta necesita al menos un ingrediente valido.");
        }

        var repeatedIngredients = ingredients
            .GroupBy(ingredient => ingredient.IngredientId)
            .Any(group => group.Count() > 1);
        if (repeatedIngredients)
        {
            throw new ValidationException("No se pueden repetir ingredientes en una receta.");
        }

        if (steps.Count == 0 || steps.Any(step => string.IsNullOrWhiteSpace(step.Description)))
        {
            throw new ValidationException("La receta necesita al menos un paso valido.");
        }
    }

    private static string[] NormalizeTags(IEnumerable<string> tags)
    {
        return tags.Select(tag => tag.Trim().ToLowerInvariant()).Where(tag => tag.Length > 0).Distinct().ToArray();
    }

    private static bool CanManage(UserEntity user, RecipeEntity recipe)
    {
        return user.Role == "admin" || recipe.OwnerId == user.Id;
    }
}
