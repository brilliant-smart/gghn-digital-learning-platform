using GGHN.DigitalLearning.Application.DTOs;
using GGHN.DigitalLearning.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace GGHN.DigitalLearning.Api.Endpoints;

public static class AdminEndpoints
{
    public static IEndpointRouteBuilder MapAdminEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/auth/admin").WithTags("User Management (Admin)")
            .RequireAuthorization("Admin");

        group.MapGet("/users", async (int? page, int? pageSize, IAuthService authService) =>
        {
            var result = await authService.GetAllUsersAsync(page ?? 1, pageSize ?? 20);
            return Results.Ok(result);
        })
        .WithSummary("List all users with pagination");

        group.MapGet("/users/{userId}", async (string userId, IAuthService authService) =>
        {
            try
            {
                var user = await authService.GetCurrentUserAsync(userId);
                return Results.Ok(user);
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound();
            }
        })
        .WithSummary("Get user details by ID");

        group.MapPut("/users/{userId}/role", async (string userId, string role, IAuthService authService) =>
        {
            var updated = await authService.UpdateUserRoleAsync(userId, role);
            return updated ? Results.Ok(new { message = "Role updated" }) : Results.NotFound();
        })
        .WithSummary("Change user role");

        group.MapPut("/users/{userId}/tier", async (string userId, string tier, IAuthService authService) =>
        {
            var updated = await authService.UpdateUserTierAsync(userId, tier);
            return updated ? Results.Ok(new { message = "Membership tier updated" }) : Results.NotFound();
        })
        .WithSummary("Change user membership tier");

        group.MapDelete("/users/{userId}", async (string userId, IAuthService authService) =>
        {
            var deleted = await authService.DeleteUserAsync(userId);
            return deleted ? Results.NoContent() : Results.NotFound();
        })
        .WithSummary("Delete a user");

        return endpoints;
    }
}