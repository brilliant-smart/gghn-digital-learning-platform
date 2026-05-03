using GGHN.DigitalLearning.Application.DTOs;
using GGHN.DigitalLearning.Application.Interfaces;

namespace GGHN.DigitalLearning.Api.Endpoints;

public static class ConferenceEndpoints
{
    public static IEndpointRouteBuilder MapConferenceEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var conferenceGroup = endpoints.MapGroup("/api/conferences").WithTags("Conferences");

        conferenceGroup.MapGet("/", async (IConferenceService service) =>
        {
            var conferences = await service.GetAllAsync();
            return Results.Ok(conferences);
        })
        .WithSummary("Get all conferences")
        .AllowAnonymous();

        conferenceGroup.MapGet("/{id:guid}", async (Guid id, IConferenceService service) =>
        {
            var conference = await service.GetByIdAsync(id);
            return conference is null ? Results.NotFound() : Results.Ok(conference);
        })
        .WithSummary("Get a conference by ID with sessions and sponsors")
        .AllowAnonymous();

        conferenceGroup.MapPost("/", async (CreateConferenceRequest request, IConferenceService service) =>
        {
            var conference = await service.CreateAsync(request);
            return Results.Created($"/api/conferences/{conference.Id}", conference);
        })
        .WithSummary("Create a new conference")
        .RequireAuthorization("Admin");

        var speakersGroup = endpoints.MapGroup("/api/speakers").WithTags("Speakers");

        speakersGroup.MapGet("/", async (IConferenceService service) =>
        {
            var speakers = await service.GetAllSpeakersAsync();
            return Results.Ok(speakers);
        })
        .WithSummary("Get all speakers")
        .AllowAnonymous();

        speakersGroup.MapPost("/", async (CreateSpeakerRequest request, IConferenceService service) =>
        {
            var speaker = await service.CreateSpeakerAsync(request);
            return Results.Created($"/api/speakers/{speaker.Id}", speaker);
        })
        .WithSummary("Create a new speaker")
        .RequireAuthorization("AdminOrEditor");

        speakersGroup.MapPut("/{id:guid}", async (Guid id, UpdateSpeakerRequest request, IConferenceService service) =>
        {
            var speaker = await service.UpdateSpeakerAsync(id, request);
            return speaker is null ? Results.NotFound() : Results.Ok(speaker);
        })
        .WithSummary("Update a speaker")
        .RequireAuthorization("AdminOrEditor");

        speakersGroup.MapDelete("/{id:guid}", async (Guid id, IConferenceService service) =>
        {
            var deleted = await service.DeleteSpeakerAsync(id);
            return deleted ? Results.NoContent() : Results.NotFound();
        })
        .WithSummary("Delete a speaker")
        .RequireAuthorization("Admin");

        conferenceGroup.MapPut("/{id:guid}", async (Guid id, UpdateConferenceRequest request, IConferenceService service) =>
        {
            var result = await service.UpdateConferenceAsync(id, request);
            return result is null ? Results.NotFound() : Results.Ok(result);
        })
        .WithSummary("Update a conference")
        .RequireAuthorization("AdminOrEditor");

        conferenceGroup.MapDelete("/{id:guid}", async (Guid id, IConferenceService service) =>
        {
            var deleted = await service.DeleteConferenceAsync(id);
            return deleted ? Results.NoContent() : Results.NotFound();
        })
        .WithSummary("Delete a conference")
        .RequireAuthorization("Admin");

        var sponsorGroup = endpoints.MapGroup("/api/sponsors").WithTags("Sponsors");

        sponsorGroup.MapPost("/", async (CreateSponsorRequest request, IConferenceService service) =>
        {
            var result = await service.CreateSponsorAsync(request);
            return Results.Created($"/api/sponsors/{result.Id}", result);
        })
        .WithSummary("Add a sponsor")
        .RequireAuthorization("AdminOrEditor");

        sponsorGroup.MapPut("/{id:guid}", async (Guid id, UpdateSponsorRequest request, IConferenceService service) =>
        {
            var result = await service.UpdateSponsorAsync(id, request);
            return result is null ? Results.NotFound() : Results.Ok(result);
        })
        .WithSummary("Update a sponsor")
        .RequireAuthorization("AdminOrEditor");

        sponsorGroup.MapDelete("/{id:guid}", async (Guid id, IConferenceService service) =>
        {
            var deleted = await service.DeleteSponsorAsync(id);
            return deleted ? Results.NoContent() : Results.NotFound();
        })
        .WithSummary("Delete a sponsor")
        .RequireAuthorization("Admin");

        return endpoints;
    }
}