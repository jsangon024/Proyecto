using MealPlanner.Api.Dtos;
using MealPlanner.Api.Services;

namespace MealPlanner.Api.Endpoints;

public static class AdminEndpoints
{
    public static IEndpointRouteBuilder MapAdminEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin").WithTags("Admin");

        group.MapGet("/users", GetUsers);
        group.MapGet("/users/{id:guid}", GetUser);
        group.MapPut("/users/{id:guid}/role", UpdateRole);
        group.MapPut("/users/{id:guid}/password", ChangePassword);
        group.MapDelete("/users/{id:guid}", DeleteUser);

        return app;
    }

    private static IResult GetUsers(HttpContext context, AdminService admin)
    {
        var adminCheck = EndpointHelpers.RequireAdmin(EndpointHelpers.CurrentUser(context));
        if (adminCheck != Results.Empty)
        {
            return adminCheck;
        }

        return Results.Ok(admin.GetUsers().Select(AdminUserDto.From));
    }

    private static IResult GetUser(Guid id, HttpContext context, AdminService admin)
    {
        var adminCheck = EndpointHelpers.RequireAdmin(EndpointHelpers.CurrentUser(context));
        if (adminCheck != Results.Empty)
        {
            return adminCheck;
        }

        var user = admin.GetUser(id);
        return user is null ? Results.NotFound() : Results.Ok(AdminUserDto.From(user));
    }

    private static IResult UpdateRole(Guid id, UpdateUserRoleRequest request, HttpContext context, AdminService admin)
    {
        var currentUser = EndpointHelpers.CurrentUser(context);
        var adminCheck = EndpointHelpers.RequireAdmin(currentUser);
        if (adminCheck != Results.Empty)
        {
            return adminCheck;
        }

        if (currentUser!.Id == id && request.Role.Trim().Equals(AdminService.UserRole, StringComparison.OrdinalIgnoreCase))
        {
            return Results.BadRequest(new ErrorResponse("Un administrador no puede quitarse su propio rol de administrador."));
        }

        try
        {
            var user = admin.UpdateRole(id, request.Role);
            return user is null ? Results.NotFound() : Results.Ok(AdminUserDto.From(user));
        }
        catch (ValidationException exception)
        {
            return EndpointHelpers.ValidationError(exception);
        }
    }

    private static IResult DeleteUser(Guid id, HttpContext context, AdminService admin)
    {
        var currentUser = EndpointHelpers.CurrentUser(context);
        var adminCheck = EndpointHelpers.RequireAdmin(currentUser);
        if (adminCheck != Results.Empty)
        {
            return adminCheck;
        }

        if (currentUser!.Id == id)
        {
            return Results.BadRequest(new ErrorResponse("Un administrador no puede eliminar su propia cuenta desde el panel de administracion."));
        }

        return admin.DeleteUser(id) ? Results.NoContent() : Results.NotFound();
    }

    private static IResult ChangePassword(Guid id, AdminChangePasswordRequest request, HttpContext context, AdminService admin)
    {
        var adminCheck = EndpointHelpers.RequireAdmin(EndpointHelpers.CurrentUser(context));
        if (adminCheck != Results.Empty)
        {
            return adminCheck;
        }

        try
        {
            return admin.ChangeUserPassword(id, request) ? Results.NoContent() : Results.NotFound();
        }
        catch (ValidationException exception)
        {
            return EndpointHelpers.ValidationError(exception);
        }
    }
}
