using System.Collections.Concurrent;
using MealPlanner.Api.Models;

namespace MealPlanner.Api.Services;

public sealed class TokenSessionStore
{
    private readonly ConcurrentDictionary<string, AuthSession> _sessions = new();

    public void Save(string token, AuthSession session)
    {
        _sessions[token] = session;
    }

    public bool TryGet(string token, out AuthSession? session)
    {
        return _sessions.TryGetValue(token, out session);
    }

    public void Remove(string token)
    {
        _sessions.TryRemove(token, out _);
    }
}
