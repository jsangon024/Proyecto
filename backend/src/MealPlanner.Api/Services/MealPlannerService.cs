using MealPlanner.Api.Data;
using MealPlanner.Api.Data.Entities;
using MealPlanner.Api.Dtos;
using Microsoft.EntityFrameworkCore;

namespace MealPlanner.Api.Services;

public sealed class MealPlannerService
{
    private readonly MealPlannerDbContext _db;

    public MealPlannerService(MealPlannerDbContext db)
    {
        _db = db;
    }

    public MealPlanDto Generate(UserEntity user, int year, int month)
    {
        if (month is < 1 or > 12 || year < 2024)
        {
            throw new ValidationException("Mes o anio no valido.");
        }

        var availableRecipes = FindAvailableRecipes(user);
        var plan = BuildPlan(user, year, month, availableRecipes);

        _db.MealPlans.Add(plan);
        _db.SaveChanges();

        return ToDto(GetByIdEntity(user, plan.Id) ?? plan);
    }

    public MealPlanDto? GetLatest(UserEntity user)
    {
        var plan = _db.MealPlans
            .AsNoTracking()
            .IncludeFullPlan()
            .Where(candidate => candidate.UserId == user.Id)
            .OrderByDescending(candidate => candidate.CreatedAt)
            .FirstOrDefault();

        return plan is null ? null : ToDto(plan);
    }

    public IReadOnlyCollection<MealPlanDto> GetAll(UserEntity user)
    {
        return _db.MealPlans
            .AsNoTracking()
            .IncludeFullPlan()
            .Where(candidate => candidate.UserId == user.Id)
            .OrderByDescending(candidate => candidate.CreatedAt)
            .ToArray()
            .Select(plan => ToDto(plan))
            .ToArray();
    }

    public MealPlanEntity? GetByIdEntity(UserEntity user, Guid id)
    {
        return _db.MealPlans
            .AsNoTracking()
            .IncludeFullPlan()
            .FirstOrDefault(plan => plan.Id == id && plan.UserId == user.Id);
    }

    public MealPlanDto? GetById(UserEntity user, Guid id)
    {
        var plan = GetByIdEntity(user, id);
        return plan is null ? null : ToDto(plan);
    }

    public bool Delete(UserEntity user, Guid id)
    {
        var plan = _db.MealPlans.FirstOrDefault(candidate => candidate.Id == id && candidate.UserId == user.Id);
        if (plan is null)
        {
            return false;
        }

        _db.MealPlans.Remove(plan);
        _db.SaveChanges();
        return true;
    }

    private RecipeEntity[] FindAvailableRecipes(UserEntity user)
    {
        var query = _db.Recipes
            .AsNoTracking()
            .Include(recipe => recipe.Ingredients)
            .ThenInclude(recipeIngredient => recipeIngredient.Ingredient)
            .AsQueryable();

        if (user.ExcludedIngredients.Length > 0)
        {
            query = query.Where(recipe => recipe.Ingredients.All(ingredient => !user.ExcludedIngredients.Contains(ingredient.IngredientId)));
        }

        var availableRecipes = query.ToArray();
        if (availableRecipes.Length == 0)
        {
            throw new ValidationException("No hay recetas disponibles para las restricciones indicadas.");
        }

        return availableRecipes;
    }

    private static MealPlanEntity BuildPlan(UserEntity user, int year, int month, IReadOnlyList<RecipeEntity> availableRecipes)
    {
        var plan = new MealPlanEntity
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Year = year,
            Month = month,
            CreatedAt = DateTimeOffset.UtcNow
        };

        var dayCount = DateTime.DaysInMonth(year, month);
        var mealTypes = new[] { "breakfast", "lunch", "dinner" };
        var targetMealCalories = Math.Max(1, user.DailyCalories / mealTypes.Length);

        for (var day = 1; day <= dayCount; day++)
        {
            var planDay = new MealPlanDayEntity
            {
                Id = Guid.NewGuid(),
                PlanDate = new DateOnly(year, month, day)
            };

            foreach (var mealType in mealTypes.Select((value, index) => new { value, index }))
            {
                var candidates = availableRecipes
                    .Where(recipe => recipe.MealType == mealType.value)
                    .DefaultIfEmpty(availableRecipes[(day + mealType.index) % availableRecipes.Count])
                    .OrderBy(recipe => Math.Abs(RecipeNutrition(recipe).Calories - targetMealCalories))
                    .ThenBy(recipe => recipe.Name)
                    .ToArray();

                var selected = candidates[(day + mealType.index) % candidates.Length];
                planDay.Meals.Add(new PlannedMealEntity
                {
                    Id = Guid.NewGuid(),
                    RecipeId = selected.Id,
                    MealType = mealType.value
                });
            }

            plan.Days.Add(planDay);
        }

        return plan;
    }

    private static MealPlanDto ToDto(MealPlanEntity plan)
    {
        return new MealPlanDto(
            plan.Id,
            plan.UserId,
            plan.Year,
            plan.Month,
            plan.CreatedAt,
            plan.Days
                .OrderBy(day => day.PlanDate)
                .Select(day => new MealPlanDayDto(
                    day.PlanDate.ToString("yyyy-MM-dd"),
                    day.Meals
                        .OrderBy(meal => meal.MealType)
                        .Select(meal =>
                        {
                            var nutrition = RecipeNutrition(meal.Recipe!);
                            return new PlannedMealDto(meal.MealType, meal.RecipeId, meal.Recipe!.Name, nutrition.Calories, nutrition.Protein);
                        })
                        .ToArray()))
                .ToArray());
    }

    private static (decimal Calories, decimal Protein) RecipeNutrition(RecipeEntity recipe)
    {
        var calories = recipe.Ingredients.Sum(item =>
            item.Ingredient is null ? 0 : NutritionCalculator.CalculateByQuantity(item.Ingredient.CaloriesPer100G, item.Quantity, item.Unit));
        var protein = recipe.Ingredients.Sum(item =>
            item.Ingredient is null ? 0 : NutritionCalculator.CalculateByQuantity(item.Ingredient.ProteinPer100G, item.Quantity, item.Unit));

        return (calories, protein);
    }
}

internal static class MealPlanQueryExtensions
{
    public static IQueryable<MealPlanEntity> IncludeFullPlan(this IQueryable<MealPlanEntity> query)
    {
        return query
            .Include(plan => plan.Days)
            .ThenInclude(day => day.Meals)
            .ThenInclude(meal => meal.Recipe)
            .ThenInclude(recipe => recipe!.Ingredients)
            .ThenInclude(recipeIngredient => recipeIngredient.Ingredient);
    }
}
