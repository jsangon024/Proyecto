namespace MealPlanner.Api.Services;

public class ValidationException : Exception
{
    public ValidationException(string message)
        : base(message)
    {
    }
}

public sealed class ConflictException : Exception
{
    public ConflictException(string message)
        : base(message)
    {
    }
}
