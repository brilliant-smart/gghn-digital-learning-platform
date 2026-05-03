using GGHN.DigitalLearning.Application.DTOs;
using GGHN.DigitalLearning.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace GGHN.DigitalLearning.Api.Endpoints;

public static class DiscussionEndpoints
{
    public static IEndpointRouteBuilder MapDiscussionEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/discussions").WithTags("Discussions");

        group.MapGet("/resource/{resourceId:guid}", async (Guid resourceId, int? page, int? pageSize, IDiscussionService service) =>
        {
            var result = await service.GetByResourceAsync(resourceId, page ?? 1, pageSize ?? 20);
            return Results.Ok(result);
        })
        .WithSummary("Get all top-level discussions for a resource")
        .AllowAnonymous();

        group.MapGet("/{id:guid}", async (Guid id, IDiscussionService service) =>
        {
            var discussion = await service.GetByIdAsync(id);
            return discussion is null ? Results.NotFound() : Results.Ok(discussion);
        })
        .WithSummary("Get a discussion with replies")
        .AllowAnonymous();

        group.MapPost("/", async (CreateDiscussionRequest request, IDiscussionService service, ClaimsPrincipal user) =>
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var discussion = await service.CreateAsync(request, userId);
            return Results.Created($"/api/discussions/{discussion.Id}", discussion);
        })
        .WithSummary("Create a discussion")
        .RequireAuthorization();

        group.MapPost("/{id:guid}/reply", async (Guid id, CreateReplyRequest request, IDiscussionService service, ClaimsPrincipal user) =>
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var reply = await service.ReplyAsync(id, request, userId);
            return Results.Created($"/api/discussions/{reply.Id}", reply);
        })
        .WithSummary("Reply to a discussion")
        .RequireAuthorization();

        group.MapPut("/{id:guid}", async (Guid id, UpdateDiscussionRequest request, IDiscussionService service, ClaimsPrincipal user) =>
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var discussion = await service.UpdateAsync(id, request, userId);
            return discussion is null ? Results.NotFound() : Results.Ok(discussion);
        })
        .WithSummary("Edit own discussion")
        .RequireAuthorization();

        group.MapDelete("/{id:guid}", async (Guid id, IDiscussionService service, ClaimsPrincipal user) =>
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var isAdmin = user.IsInRole("Admin");
            var deleted = await service.DeleteAsync(id, userId, isAdmin);
            return deleted ? Results.NoContent() : Results.NotFound();
        })
        .WithSummary("Delete own discussion (or any if Admin)")
        .RequireAuthorization();

        return endpoints;
    }
}