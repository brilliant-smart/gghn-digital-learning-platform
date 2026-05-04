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
            var result = await authService.LoginAsync(request);
            return Results.Ok(result);
        })
        .WithSummary("Login and get JWT token")
        .AllowAnonymous()
        .RequireRateLimiting("auth-login");

        group.MapPost("/refresh", async (RefreshTokenRequest request, IAuthService authService) =>
        {
            var result = await authService.RefreshTokenAsync(request.RefreshToken);
            return Results.Ok(result);
        })
        .WithSummary("Refresh access token using a valid refresh token")
        .AllowAnonymous()
        .RequireRateLimiting("auth-login");

        group.MapGet("/me", async (ClaimsPrincipal user, IAuthService authService) =>
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? user.FindFirst("sub")?.Value
                ?? throw new UnauthorizedAccessException();

            var result = await authService.GetCurrentUserAsync(userId);
            return Results.Ok(result);
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

        group.MapPost("/resend-verification", async (ResendVerificationRequest request, IAuthService authService) =>
        {
            await authService.ResendEmailConfirmationAsync(request.Email);
            return Results.Ok(new { message = "If an account with that email exists, a verification link has been sent." });
        })
        .WithSummary("Resend email verification link")
        .AllowAnonymous()
        .RequireRateLimiting("auth-register");

        return endpoints;
    }
}