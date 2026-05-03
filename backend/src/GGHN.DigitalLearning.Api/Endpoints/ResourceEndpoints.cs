using GGHN.DigitalLearning.Application.DTOs;
using GGHN.DigitalLearning.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace GGHN.DigitalLearning.Api.Endpoints;

public static class ResourceEndpoints
{
    public static IEndpointRouteBuilder MapResourceEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/resources").WithTags("Resources");

        group.MapGet("/", async ([AsParameters] ResourceFilterParams filter, IResourceService service) =>
        {
            var result = await service.GetAllAsync(filter);
            return Results.Ok(result);
        })
        .WithSummary("Get all published resources with filtering and pagination")
        .AllowAnonymous();

        group.MapGet("/{id:guid}", async (Guid id, IResourceService service, ClaimsPrincipal? user) =>
        {
            var resource = await service.GetByIdAsync(id);
            if (resource is null) return Results.NotFound();

            var userId = user?.Identity?.IsAuthenticated == true
                ? user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                : null;
            _ = service.RecordViewAsync(id, userId);

            return Results.Ok(resource);
        })
        .WithSummary("Get a resource by ID")
        .AllowAnonymous();

        group.MapPost("/", async (CreateResourceRequest request, IResourceService service) =>
        {
            var resource = await service.CreateAsync(request);
            return Results.Created($"/api/resources/{resource.Id}", resource);
        })
        .WithSummary("Create a new resource")
        .RequireAuthorization("AdminOrEditor");

        group.MapPut("/{id:guid}", async (Guid id, UpdateResourceRequest request, IResourceService service) =>
        {
            var resource = await service.UpdateAsync(id, request);
            return resource is null ? Results.NotFound() : Results.Ok(resource);
        })
        .WithSummary("Update a resource")
        .RequireAuthorization("AdminOrEditor");

        group.MapDelete("/{id:guid}", async (Guid id, IResourceService service) =>
        {
            var deleted = await service.DeleteAsync(id);
            return deleted ? Results.NoContent() : Results.NotFound();
        })
        .WithSummary("Delete a resource")
        .RequireAuthorization("Admin");

        return endpoints;
    }
}