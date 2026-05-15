using System.Security.Cryptography;
using MealPlanner.Api.Data;
using MealPlanner.Api.Data.Entities;
using MealPlanner.Api.Models;

namespace MealPlanner.Api.Services;

public sealed class TokenService
{
    private readonly MealPlannerDbContext _db;
    private readonly TokenSessionStore _sessions;

    public TokenService(MealPlannerDbContext db, TokenSessionStore sessions)
    {
        _db = db;
        _sessions = sessions;
    }

    public string Create(UserEntity user)
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        var token = Convert.ToBase64String(bytes);
        _sessions.Save(token, new AuthSession(user.Id, DateTimeOffset.UtcNow.AddHours(8)));
        return token;
    }

    public UserEntity? Validate(string token)
    {
        if (!_sessions.TryGet(token, out var session) || session is null)
        {
            return null;
        }

        if (session.ExpiresAtUtc <= DateTimeOffset.UtcNow)
        {
            _sessions.Remove(token);
            return null;
        }

        return _db.Users.Find(session.UserId);
    }
}
