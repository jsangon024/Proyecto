using MealPlanner.Api.Data;
using MealPlanner.Api.Endpoints;
using MealPlanner.Api.Middleware;
using MealPlanner.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("MealPlannerDb")
    ?? "Host=localhost;Port=5432;Database=meal_planner_db;Username=postgres;Password=postgres";

builder.Services.AddDbContext<MealPlannerDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

builder.Services.AddSingleton<PasswordHasher>();
builder.Services.AddSingleton<TokenSessionStore>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<IngredientService>();
builder.Services.AddScoped<RecipeService>();
builder.Services.AddScoped<InventoryService>();
builder.Services.AddScoped<MealPlannerService>();
builder.Services.AddScoped<ShoppingListService>();
builder.Services.AddEndpointsApiExplorer();

var allowedOrigins = (builder.Configuration["ALLOWED_ORIGINS"] ?? "http://localhost:5173,http://localhost:3000")
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("frontend");
app.UseSecurityHeaders();
app.UseMiddleware<BearerTokenMiddleware>();

app.MapHealthEndpoints();
app.MapAuthEndpoints();
app.MapUserEndpoints();
app.MapAdminEndpoints();
app.MapIngredientEndpoints();
app.MapRecipeEndpoints();
app.MapInventoryEndpoints();
app.MapPlanEndpoints();
app.MapShoppingListEndpoints();

app.Run();
