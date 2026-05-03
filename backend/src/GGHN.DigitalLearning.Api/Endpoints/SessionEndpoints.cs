using GGHN.DigitalLearning.Application.DTOs;
using GGHN.DigitalLearning.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace GGHN.DigitalLearning.Api.Endpoints;

public static class SessionEndpoints
{
    public static IEndpointRouteBuilder MapSessionEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/sessions").WithTags("Sessions");

        group.MapGet("/{id:guid}", async (Guid id, IConferenceService service) =>
        {
            var session = await service.GetSessionByIdAsync(id);
            return session is null ? Results.NotFound() : Results.Ok(session);
        })
        .WithSummary("Get a session by ID with speaker details")
        .AllowAnonymous();

        group.MapPost("/", async (CreateSessionRequest request, IConferenceService service) =>
        {
            var session = await service.CreateSessionAsync(request);
            return Results.Created($"/api/sessions/{session.Id}", session);
        })
        .WithSummary("Create a new session")
        .RequireAuthorization("AdminOrEditor");

        group.MapPut("/{id:guid}", async (Guid id, UpdateSessionRequest request, IConferenceService service) =>
        {
            var session = await service.UpdateSessionAsync(id, request);
            return session is null ? Results.NotFound() : Results.Ok(session);
        })
        .WithSummary("Update a session")
        .RequireAuthorization("AdminOrEditor");

        group.MapDelete("/{id:guid}", async (Guid id, IConferenceService service) =>
        {
            var deleted = await service.DeleteSessionAsync(id);
            return deleted ? Results.NoContent() : Results.NotFound();
        })
        .WithSummary("Delete a session")
        .RequireAuthorization("Admin");

        group.MapPost("/{id:guid}/publish", async (Guid id, IConferenceService service) =>
        {
            var existing = await service.GetSessionByIdAsync(id);
            if (existing is null) return Results.NotFound();

            var session = await service.UpdateSessionAsync(id, new UpdateSessionRequest
            {
                Title = existing.Title,
                Track = existing.Track,
                StartTime = existing.StartTime,
                EndTime = existing.EndTime,
                IsPublished = true
            });
            return Results.Ok(session);
        })
        .WithSummary("Publish a session recording")
        .RequireAuthorization("AdminOrEditor");

        return endpoints;
    }
}