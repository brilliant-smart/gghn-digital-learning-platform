using System.Security.Claims;
using GGHN.DigitalLearning.Application.DTOs;
using GGHN.DigitalLearning.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace GGHN.DigitalLearning.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/auth").WithTags("Authentication");

        group.MapPost("/register", async (RegisterRequest request, IAuthService authService) =>
        {
            var result = await authService.RegisterAsync(request);
            return Results.Ok(result);
        })
        .WithSummary("Register a new user")
        .AllowAnonymous()
        .RequireRateLimiting("auth-register");

        group.MapPost("/login", async (LoginRequest request, IAuthService authService) =>
        {
            try
            {
                var result = await authService.LoginAsync(request);
                return Results.Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Results.Unauthorized();
            }
        })
        .WithSummary("Login and get JWT token")
        .AllowAnonymous()
        .RequireRateLimiting("auth-login");

        group.MapPost("/refresh", async (RefreshTokenRequest request, IAuthService authService) =>
        {
            try
            {
                var result = await authService.RefreshTokenAsync(request.RefreshToken);
                return Results.Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Results.Unauthorized();
            }
        })
        .WithSummary("Refresh access token using a valid refresh token")
        .AllowAnonymous()
        .RequireRateLimiting("auth-login");

        group.MapGet("/me", async (ClaimsPrincipal user, IAuthService authService) =>
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? user.FindFirst("sub")?.Value
                ?? throw new UnauthorizedAccessException();

            try
            {
                var result = await authService.GetCurrentUserAsync(userId);
                return Results.Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound();
            }
        })
        .WithSummary("Get current user profile")
        .RequireAuthorization();

        group.MapPut("/profile", async (UpdateProfileRequest request, IAuthService authService, ClaimsPrincipal user) =>
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? user.FindFirst("sub")?.Value
                ?? throw new UnauthorizedAccessException();

            var result = await authService.UpdateProfileAsync(userId, request);
            return Results.Ok(result);
        })
        .WithSummary("Update current user profile")
        .RequireAuthorization();

        return endpoints;
    }
}