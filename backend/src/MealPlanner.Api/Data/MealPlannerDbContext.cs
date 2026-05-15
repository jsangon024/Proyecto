using MealPlanner.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace MealPlanner.Api.Data;

public sealed class MealPlannerDbContext : DbContext
{
    public MealPlannerDbContext(DbContextOptions<MealPlannerDbContext> options)
        : base(options)
    {
    }

    public DbSet<UserEntity> Users => Set<UserEntity>();

    public DbSet<IngredientEntity> Ingredients => Set<IngredientEntity>();

    public DbSet<RecipeEntity> Recipes => Set<RecipeEntity>();

    public DbSet<RecipeIngredientEntity> RecipeIngredients => Set<RecipeIngredientEntity>();

    public DbSet<UserSavedRecipeEntity> UserSavedRecipes => Set<UserSavedRecipeEntity>();

    public DbSet<InventoryItemEntity> InventoryItems => Set<InventoryItemEntity>();

    public DbSet<MealPlanEntity> MealPlans => Set<MealPlanEntity>();

    public DbSet<MealPlanDayEntity> MealPlanDays => Set<MealPlanDayEntity>();

    public DbSet<PlannedMealEntity> PlannedMeals => Set<PlannedMealEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserEntity>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(user => user.Id);
            entity.Property(user => user.Id).HasColumnName("id");
            entity.Property(user => user.Email).HasColumnName("email").HasMaxLength(255);
            entity.Property(user => user.PasswordHash).HasColumnName("password_hash");
            entity.Property(user => user.Role).HasColumnName("role").HasMaxLength(50);
            entity.Property(user => user.DailyCalories).HasColumnName("daily_calories");
            entity.Property(user => user.MinimumProteinGrams).HasColumnName("minimum_protein_grams");
            entity.Property(user => user.ExcludedIngredients).HasColumnName("excluded_ingredients");
            entity.Property(user => user.CreatedAt).HasColumnName("created_at");
        });

        modelBuilder.Entity<IngredientEntity>(entity =>
        {
            entity.ToTable("ingredients");
            entity.HasKey(ingredient => ingredient.Id);
            entity.Property(ingredient => ingredient.Id).HasColumnName("id");
            entity.Property(ingredient => ingredient.Name).HasColumnName("name").HasMaxLength(150);
            entity.Property(ingredient => ingredient.Category).HasColumnName("category").HasMaxLength(100);
            entity.Property(ingredient => ingredient.CaloriesPer100G).HasColumnName("calories_per_100g").HasPrecision(8, 2);
            entity.Property(ingredient => ingredient.ProteinPer100G).HasColumnName("protein_per_100g").HasPrecision(8, 2);
            entity.Property(ingredient => ingredient.CarbsPer100G).HasColumnName("carbs_per_100g").HasPrecision(8, 2);
            entity.Property(ingredient => ingredient.SugarsPer100G).HasColumnName("sugars_per_100g").HasPrecision(8, 2);
            entity.Property(ingredient => ingredient.FatPer100G).HasColumnName("fat_per_100g").HasPrecision(8, 2);
            entity.Property(ingredient => ingredient.FiberPer100G).HasColumnName("fiber_per_100g").HasPrecision(8, 2);
            entity.Property(ingredient => ingredient.IsGlutenFree).HasColumnName("is_gluten_free");
            entity.Property(ingredient => ingredient.IsDiabeticFriendly).HasColumnName("is_diabetic_friendly");
            entity.Property(ingredient => ingredient.GlycemicIndex).HasColumnName("glycemic_index");
            entity.Property(ingredient => ingredient.CreatedAt).HasColumnName("created_at");
        });

        modelBuilder.Entity<RecipeEntity>(entity =>
        {
            entity.ToTable("recipes");
            entity.HasKey(recipe => recipe.Id);
            entity.Property(recipe => recipe.Id).HasColumnName("id");
            entity.Property(recipe => recipe.Name).HasColumnName("name").HasMaxLength(200);
            entity.Property(recipe => recipe.MealType).HasColumnName("meal_type").HasMaxLength(50);
            entity.Property(recipe => recipe.Description).HasColumnName("description");
            entity.Property(recipe => recipe.Servings).HasColumnName("servings");
            entity.Property(recipe => recipe.Tags).HasColumnName("tags");
            entity.Property(recipe => recipe.CreatedAt).HasColumnName("created_at");
            entity.HasMany(recipe => recipe.Ingredients)
                .WithOne(recipeIngredient => recipeIngredient.Recipe)
                .HasForeignKey(recipeIngredient => recipeIngredient.RecipeId);
        });

        modelBuilder.Entity<RecipeIngredientEntity>(entity =>
        {
            entity.ToTable("recipe_ingredients");
            entity.HasKey(recipeIngredient => recipeIngredient.Id);
            entity.Property(recipeIngredient => recipeIngredient.Id).HasColumnName("id");
            entity.Property(recipeIngredient => recipeIngredient.RecipeId).HasColumnName("recipe_id");
            entity.Property(recipeIngredient => recipeIngredient.IngredientId).HasColumnName("ingredient_id");
            entity.Property(recipeIngredient => recipeIngredient.Quantity).HasColumnName("quantity").HasPrecision(10, 2);
            entity.Property(recipeIngredient => recipeIngredient.Unit).HasColumnName("unit").HasMaxLength(30);
            entity.HasOne(recipeIngredient => recipeIngredient.Ingredient)
                .WithMany()
                .HasForeignKey(recipeIngredient => recipeIngredient.IngredientId);
        });

        modelBuilder.Entity<UserSavedRecipeEntity>(entity =>
        {
            entity.ToTable("user_saved_recipes");
            entity.HasKey(savedRecipe => new { savedRecipe.UserId, savedRecipe.RecipeId });
            entity.Property(savedRecipe => savedRecipe.UserId).HasColumnName("user_id");
            entity.Property(savedRecipe => savedRecipe.RecipeId).HasColumnName("recipe_id");
            entity.Property(savedRecipe => savedRecipe.SavedAt).HasColumnName("saved_at");
        });

        modelBuilder.Entity<InventoryItemEntity>(entity =>
        {
            entity.ToTable("inventory_items");
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Id).HasColumnName("id");
            entity.Property(item => item.UserId).HasColumnName("user_id");
            entity.Property(item => item.IngredientId).HasColumnName("ingredient_id");
            entity.Property(item => item.Quantity).HasColumnName("quantity").HasPrecision(10, 2);
            entity.Property(item => item.Unit).HasColumnName("unit").HasMaxLength(30);
            entity.Property(item => item.UpdatedAt).HasColumnName("updated_at");
            entity.HasOne(item => item.Ingredient)
                .WithMany()
                .HasForeignKey(item => item.IngredientId);
        });

        modelBuilder.Entity<MealPlanEntity>(entity =>
        {
            entity.ToTable("meal_plans");
            entity.HasKey(plan => plan.Id);
            entity.Property(plan => plan.Id).HasColumnName("id");
            entity.Property(plan => plan.UserId).HasColumnName("user_id");
            entity.Property(plan => plan.Year).HasColumnName("year");
            entity.Property(plan => plan.Month).HasColumnName("month");
            entity.Property(plan => plan.CreatedAt).HasColumnName("created_at");
            entity.HasMany(plan => plan.Days)
                .WithOne(day => day.MealPlan)
                .HasForeignKey(day => day.MealPlanId);
        });

        modelBuilder.Entity<MealPlanDayEntity>(entity =>
        {
            entity.ToTable("meal_plan_days");
            entity.HasKey(day => day.Id);
            entity.Property(day => day.Id).HasColumnName("id");
            entity.Property(day => day.MealPlanId).HasColumnName("meal_plan_id");
            entity.Property(day => day.PlanDate).HasColumnName("plan_date");
            entity.HasMany(day => day.Meals)
                .WithOne(meal => meal.MealPlanDay)
                .HasForeignKey(meal => meal.MealPlanDayId);
        });

        modelBuilder.Entity<PlannedMealEntity>(entity =>
        {
            entity.ToTable("planned_meals");
            entity.HasKey(meal => meal.Id);
            entity.Property(meal => meal.Id).HasColumnName("id");
            entity.Property(meal => meal.MealPlanDayId).HasColumnName("meal_plan_day_id");
            entity.Property(meal => meal.RecipeId).HasColumnName("recipe_id");
            entity.Property(meal => meal.MealType).HasColumnName("meal_type").HasMaxLength(50);
            entity.HasOne(meal => meal.Recipe)
                .WithMany()
                .HasForeignKey(meal => meal.RecipeId);
        });
    }
}
