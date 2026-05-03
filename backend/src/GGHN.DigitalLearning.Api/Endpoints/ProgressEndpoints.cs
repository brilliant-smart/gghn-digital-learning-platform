using GGHN.DigitalLearning.Application.DTOs;
using GGHN.DigitalLearning.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace GGHN.DigitalLearning.Api.Endpoints;

public static class ProgressEndpoints
{
    public static IEndpointRouteBuilder MapProgressEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/progress").WithTags("Progress Tracking")
            .RequireAuthorization();

        group.MapGet("/me", async (ClaimsPrincipal user, IProgressService service) =>
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? user.FindFirst("sub")?.Value
                ?? throw new UnauthorizedAccessException();

            var progress = await service.GetUserProgressAsync(userId);
            return Results.Ok(progress);
        })
        .WithSummary("Get current user's learning progress");

        group.MapPost("/lesson-complete", async (MarkLessonCompleteRequest request, ClaimsPrincipal user, IProgressService service) =>
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? user.FindFirst("sub")?.Value
                ?? throw new UnauthorizedAccessException();

            var progress = await service.MarkLessonCompleteAsync(userId, request);
            return Results.Ok(progress);
        })
        .WithSummary("Mark a lesson as completed");

        group.MapPost("/pathway-complete/{pathwayId:guid}", async (Guid pathwayId, ClaimsPrincipal user, IProgressService service) =>
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? user.FindFirst("sub")?.Value
                ?? throw new UnauthorizedAccessException();

            var progress = await service.MarkPathwayCompleteAsync(userId, pathwayId);
            return Results.Ok(progress);
        })
        .WithSummary("Mark a learning pathway as completed");

        group.MapGet("/certificate/{progressId:guid}", async (Guid progressId, ClaimsPrincipal user, ICertificateService certService) =>
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? user.FindFirst("sub")?.Value
                ?? throw new UnauthorizedAccessException();

            var certificate = await certService.GetCertificateAsync(progressId, userId);
            return certificate is null ? Results.NotFound() : Results.Ok(certificate);
        })
        .WithSummary("Get certificate for a completed pathway or course");

        return endpoints;
    }
}