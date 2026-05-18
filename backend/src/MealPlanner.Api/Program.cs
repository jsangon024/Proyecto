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

var configuredOrigins = (builder.Configuration["ALLOWED_ORIGINS"] ?? string.Empty)
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
var allowedOrigins = configuredOrigins
    .Concat(new[]
    {
        "http://localhost:5173",
        "http://localhost:3000",
        "https://mealplanner-six-xi.vercel.app"
    })
    .Select(NormalizeOrigin)
    .Where(origin => !string.IsNullOrWhiteSpace(origin))
    .Distinct(StringComparer.OrdinalIgnoreCase)
    .ToArray();

builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
    {
        policy.SetIsOriginAllowed(origin => IsAllowedOrigin(origin, allowedOrigins))
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("frontend");
app.UseHttpsRedirection();
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

static string NormalizeOrigin(string origin)
{
    if (string.IsNullOrWhiteSpace(origin))
    {
        return string.Empty;
    }

    if (!Uri.TryCreate(origin.Trim().TrimEnd('/'), UriKind.Absolute, out var uri))
    {
        return origin.Trim().TrimEnd('/');
    }

    return uri.IsDefaultPort
        ? $"{uri.Scheme}://{uri.Host}"
        : $"{uri.Scheme}://{uri.Host}:{uri.Port}";
}

static bool IsAllowedOrigin(string origin, string[] allowedOrigins)
{
    var normalizedOrigin = NormalizeOrigin(origin);
    if (allowedOrigins.Contains(normalizedOrigin, StringComparer.OrdinalIgnoreCase))
    {
        return true;
    }

    return Uri.TryCreate(normalizedOrigin, UriKind.Absolute, out var uri)
        && uri.Scheme == "https"
        && uri.Host.EndsWith(".vercel.app", StringComparison.OrdinalIgnoreCase)
        && uri.Host.StartsWith("mealplanner-", StringComparison.OrdinalIgnoreCase);
}
