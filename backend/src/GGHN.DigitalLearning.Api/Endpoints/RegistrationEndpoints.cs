using GGHN.DigitalLearning.Application.DTOs;
using GGHN.DigitalLearning.Application.Interfaces;
using System.Security.Claims;

namespace GGHN.DigitalLearning.Api.Endpoints;

public static class RegistrationEndpoints
{
    public static IEndpointRouteBuilder MapRegistrationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/registrations").WithTags("Registrations");

        group.MapPost("/", async (CreateRegistrationRequest request, IRegistrationService service, ClaimsPrincipal? user) =>
        {
            var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await service.RegisterAsync(request, userId);
            return Results.Created($"/api/registrations/{result.Id}", result);
        })
        .WithSummary("Submit a conference registration")
        .AllowAnonymous();

        group.MapGet("/mine", async (IRegistrationService service, ClaimsPrincipal user) =>
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var result = await service.GetMyRegistrationsAsync(userId);
            return Results.Ok(result);
        })
        .WithSummary("Get my registrations");

        group.MapGet("/{id:guid}", async (Guid id, IRegistrationService service) =>
        {
            var result = await service.GetByIdAsync(id);
            return result is null ? Results.NotFound() : Results.Ok(result);
        })
        .WithSummary("Get registration by ID");

        group.MapGet("/conference/{conferenceId:guid}", async (Guid conferenceId, string? status, IRegistrationService service) =>
        {
            var result = await service.GetByConferenceAsync(conferenceId, status);
            return Results.Ok(result);
        })
        .WithSummary("List registrations for a conference")
        .RequireAuthorization("AdminOrEditor");

        group.MapPut("/{id:guid}/status", async (Guid id, UpdateRegistrationStatusRequest request, IRegistrationService service, ClaimsPrincipal user) =>
        {
            var reviewerId = user.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var result = await service.UpdateStatusAsync(id, request, reviewerId);
            return result is null ? Results.NotFound() : Results.Ok(result);
        })
        .WithSummary("Approve, reject, or waitlist a registration")
        .RequireAuthorization("AdminOrEditor");

        group.MapGet("/conference/{conferenceId:guid}/stats", async (Guid conferenceId, IRegistrationService service) =>
        {
            var result = await service.GetStatsAsync(conferenceId);
            return Results.Ok(result);
        })
        .WithSummary("Registration statistics for a conference")
        .RequireAuthorization("AdminOrEditor");

        return endpoints;
    }
}
