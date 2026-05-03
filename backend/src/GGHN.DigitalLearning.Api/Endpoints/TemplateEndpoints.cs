using GGHN.DigitalLearning.Application.DTOs;
using GGHN.DigitalLearning.Application.Interfaces;
using GGHN.DigitalLearning.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace GGHN.DigitalLearning.Api.Endpoints;

public static class TemplateEndpoints
{
    public static IEndpointRouteBuilder MapTemplateEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/templates").WithTags("Templates & Tools");

        group.MapGet("/", async (ITemplateService service) =>
        {
            var templates = await service.GetAllAsync();
            return Results.Ok(templates);
        })
        .WithSummary("Get all published templates")
        .AllowAnonymous();

        group.MapGet("/{id:guid}", async (Guid id, ITemplateService service, ClaimsPrincipal user) =>
        {
            var template = await service.GetByIdAsync(id);
            if (template is null) return Results.NotFound();

            if (template.Tier == TemplateTier.Free.ToString()) return Results.Ok(template);

            if (user.Identity?.IsAuthenticated != true) return Results.Challenge();

            if (user.IsInRole("Admin") || user.IsInRole("Editor")) return Results.Ok(template);

            var tierClaim = user.FindFirst("membershipTier")?.Value;
            if (!Enum.TryParse<MembershipTier>(tierClaim, out var userTier))
                userTier = MembershipTier.Free;

            var tierOrder = new[] { MembershipTier.Free, MembershipTier.Member, MembershipTier.Institutional };

            return Array.IndexOf(tierOrder, userTier) >= Array.IndexOf(tierOrder, MembershipTier.Member)
                ? Results.Ok(template)
                : Results.Forbid();
        })
        .WithSummary("Get a template by ID");

        group.MapPost("/", async (CreateTemplateRequest request, ITemplateService service) =>
        {
            var template = await service.CreateAsync(request);
            return Results.Created($"/api/templates/{template.Id}", template);
        })
        .WithSummary("Create a new template")
        .RequireAuthorization("AdminOrEditor");

        group.MapPut("/{id:guid}", async (Guid id, CreateTemplateRequest request, ITemplateService service) =>
        {
            var template = await service.UpdateAsync(id, request);
            return template is null ? Results.NotFound() : Results.Ok(template);
        })
        .WithSummary("Update a template")
        .RequireAuthorization("AdminOrEditor");

        group.MapDelete("/{id:guid}", async (Guid id, ITemplateService service) =>
        {
            var deleted = await service.DeleteAsync(id);
            return deleted ? Results.NoContent() : Results.NotFound();
        })
        .WithSummary("Delete a template")
        .RequireAuthorization("Admin");

        return endpoints;
    }
}