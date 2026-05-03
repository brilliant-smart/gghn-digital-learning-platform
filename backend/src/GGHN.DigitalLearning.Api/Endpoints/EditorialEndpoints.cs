using GGHN.DigitalLearning.Application.DTOs;
using GGHN.DigitalLearning.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace GGHN.DigitalLearning.Api.Endpoints;

public static class EditorialEndpoints
{
    public static IEndpointRouteBuilder MapEditorialEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/editorial").WithTags("Editorial");

        group.MapGet("/queue", async (IEditorialService service) =>
        {
            var queue = await service.GetQueueAsync();
            return Results.Ok(queue);
        })
        .WithSummary("List resources in Draft/UnderReview status")
        .RequireAuthorization("AdminOrEditor");

        group.MapPost("/resources/{resourceId:guid}/submit", async (Guid resourceId, IEditorialService service, ClaimsPrincipal user) =>
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var review = await service.SubmitForReviewAsync(resourceId, userId);
            return Results.Ok(review);
        })
        .WithSummary("Submit a resource for review")
        .RequireAuthorization();

        group.MapPost("/reviews", async (CreateReviewRequest request, IEditorialService service, ClaimsPrincipal user) =>
        {
            var reviewerId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var review = await service.CreateReviewAsync(request, reviewerId);
            return Results.Created($"/api/editorial/reviews/{review.Id}", review);
        })
        .WithSummary("Create an editorial review")
        .RequireAuthorization("AdminOrEditor");

        group.MapPut("/reviews/{id:guid}", async (Guid id, UpdateReviewRequest request, IEditorialService service) =>
        {
            var review = await service.UpdateReviewAsync(id, request);
            return review is null ? Results.NotFound() : Results.Ok(review);
        })
        .WithSummary("Update an editorial review")
        .RequireAuthorization("AdminOrEditor");

        group.MapPost("/resources/{resourceId:guid}/approve", async (Guid resourceId, IEditorialService service, ClaimsPrincipal user) =>
        {
            var reviewerId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var approved = await service.ApproveAsync(resourceId, reviewerId);
            return approved ? Results.Ok(new { message = "Resource approved and published" }) : Results.NotFound();
        })
        .WithSummary("Approve and publish a resource")
        .RequireAuthorization("AdminOrEditor");

        group.MapPost("/resources/{resourceId:guid}/reject", async (Guid resourceId, IEditorialService service, ClaimsPrincipal user, string? reason) =>
        {
            var reviewerId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var rejected = await service.RejectAsync(resourceId, reviewerId, reason ?? "Rejected by reviewer");
            return rejected ? Results.Ok(new { message = "Resource rejected" }) : Results.NotFound();
        })
        .WithSummary("Reject a resource with feedback")
        .RequireAuthorization("AdminOrEditor");

        group.MapGet("/publications/queue", async (IEditorialService service) =>
        {
            var queue = await service.GetPublicationQueueAsync();
            return Results.Ok(queue);
        })
        .WithSummary("List publications in Draft/UnderReview status")
        .RequireAuthorization("AdminOrEditor");

        group.MapPost("/publications/{publicationId:guid}/submit", async (Guid publicationId, IEditorialService service, ClaimsPrincipal user) =>
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var ok = await service.SubmitPublicationForReviewAsync(publicationId, userId);
            return ok ? Results.Ok(new { message = "Publication submitted for review" }) : Results.NotFound();
        })
        .WithSummary("Submit a publication for review")
        .RequireAuthorization();

        group.MapPost("/publications/{publicationId:guid}/approve", async (Guid publicationId, IEditorialService service, ClaimsPrincipal user) =>
        {
            var reviewerId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var ok = await service.ApprovePublicationAsync(publicationId, reviewerId);
            return ok ? Results.Ok(new { message = "Publication approved and published" }) : Results.NotFound();
        })
        .WithSummary("Approve and publish a publication")
        .RequireAuthorization("AdminOrEditor");

        group.MapPost("/publications/{publicationId:guid}/reject", async (Guid publicationId, IEditorialService service, ClaimsPrincipal user, string? reason) =>
        {
            var reviewerId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var ok = await service.RejectPublicationAsync(publicationId, reviewerId, reason ?? "Rejected by reviewer");
            return ok ? Results.Ok(new { message = "Publication rejected" }) : Results.NotFound();
        })
        .WithSummary("Reject a publication with feedback")
        .RequireAuthorization("AdminOrEditor");

        return endpoints;
    }
}