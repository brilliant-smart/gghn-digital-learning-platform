using GGHN.DigitalLearning.Application.DTOs;
using GGHN.DigitalLearning.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace GGHN.DigitalLearning.Api.Endpoints;

public static class PathwayEndpoints
{
    public static IEndpointRouteBuilder MapPathwayEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/pathways").WithTags("Learning Pathways");

        group.MapGet("/", async (IPathwayService service) =>
        {
            var pathways = await service.GetAllAsync();
            return Results.Ok(pathways);
        })
        .WithSummary("Get all learning pathways")
        .AllowAnonymous();

        group.MapGet("/{id:guid}", async (Guid id, IPathwayService service) =>
        {
            var pathway = await service.GetByIdAsync(id);
            return pathway is null ? Results.NotFound() : Results.Ok(pathway);
        })
        .WithSummary("Get a pathway by ID with resources")
        .AllowAnonymous();

        group.MapPost("/", async (CreatePathwayRequest request, IPathwayService service) =>
        {
            var pathway = await service.CreateAsync(request);
            return Results.Created($"/api/pathways/{pathway.Id}", pathway);
        })
        .WithSummary("Create a new learning pathway")
        .RequireAuthorization("AdminOrEditor");

        group.MapPut("/{id:guid}", async (Guid id, CreatePathwayRequest request, IPathwayService service) =>
        {
            var pathway = await service.UpdateAsync(id, request);
            return pathway is null ? Results.NotFound() : Results.Ok(pathway);
        })
        .WithSummary("Update a learning pathway")
        .RequireAuthorization("AdminOrEditor");

        group.MapDelete("/{id:guid}", async (Guid id, IPathwayService service) =>
        {
            var deleted = await service.DeleteAsync(id);
            return deleted ? Results.NoContent() : Results.NotFound();
        })
        .WithSummary("Delete a learning pathway")
        .RequireAuthorization("Admin");

        return endpoints;
    }
}