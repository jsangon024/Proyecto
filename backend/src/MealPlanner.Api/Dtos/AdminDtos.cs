using MealPlanner.Api.Data.Entities;

namespace MealPlanner.Api.Dtos;

public sealed record AdminUserDto(Guid Id, string Email, string Role, DateTimeOffset CreatedAt)
{
    public static AdminUserDto From(UserEntity user)
    {
        return new AdminUserDto(user.Id, user.Email, user.Role, user.CreatedAt);
    }
}

public sealed record UpdateUserRoleRequest(string Role);
