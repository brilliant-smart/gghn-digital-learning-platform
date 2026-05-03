using GGHN.DigitalLearning.Application.DTOs;
using GGHN.DigitalLearning.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace GGHN.DigitalLearning.Api.Endpoints;

public static class PublicationEndpoints
{
    public static IEndpointRouteBuilder MapPublicationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/publications").WithTags("Publications");

        group.MapGet("/", async ([AsParameters] PublicationFilterParams filter, IPublicationService service) =>
        {
            var result = await service.GetAllAsync(filter);
            return Results.Ok(result);
        })
        .WithSummary("Get all published publications with filtering and pagination")
        .AllowAnonymous();

        group.MapGet("/{id:guid}", async (Guid id, IPublicationService service) =>
        {
            var publication = await service.GetByIdAsync(id);
            return publication is null ? Results.NotFound() : Results.Ok(publication);
        })
        .WithSummary("Get a publication by ID");

        group.MapPost("/", async (CreatePublicationRequest request, IPublicationService service) =>
        {
            var publication = await service.CreateAsync(request);
            return Results.Created($"/api/publications/{publication.Id}", publication);
        })
        .WithSummary("Create a new publication")
        .RequireAuthorization("AdminOrEditor");

        group.MapPut("/{id:guid}", async (Guid id, UpdatePublicationRequest request, IPublicationService service) =>
        {
            var publication = await service.UpdateAsync(id, request);
            return publication is null ? Results.NotFound() : Results.Ok(publication);
        })
        .WithSummary("Update a publication")
        .RequireAuthorization("AdminOrEditor");

        group.MapDelete("/{id:guid}", async (Guid id, IPublicationService service) =>
        {
            var deleted = await service.DeleteAsync(id);
            return deleted ? Results.NoContent() : Results.NotFound();
        })
        .WithSummary("Delete a publication")
        .RequireAuthorization("Admin");

        return endpoints;
    }
}