using GGHN.DigitalLearning.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace GGHN.DigitalLearning.Api.Endpoints;

public static class AnalyticsEndpoints
{
    public static IEndpointRouteBuilder MapAnalyticsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/analytics").WithTags("Analytics")
            .RequireAuthorization("AdminOrEditor");

        group.MapGet("/dashboard", async (IAnalyticsService service) =>
        {
            var stats = await service.GetDashboardStatsAsync();
            return Results.Ok(stats);
        })
        .WithSummary("Get overview statistics");

        group.MapGet("/top-resources", async (IAnalyticsService service, int? count) =>
        {
            var resources = await service.GetTopResourcesAsync(count ?? 10);
            return Results.Ok(resources);
        })
        .WithSummary("Get most-engaged resources");

        group.MapGet("/top-pathways", async (IAnalyticsService service, int? count) =>
        {
            var pathways = await service.GetTopPathwaysAsync(count ?? 10);
            return Results.Ok(pathways);
        })
        .WithSummary("Get most-completed pathways");

        group.MapGet("/by-geography", async (IAnalyticsService service) =>
        {
            var stats = await service.GetByGeographyAsync();
            return Results.Ok(stats);
        })
        .WithSummary("Get user distribution by country");

        group.MapGet("/by-audience", async (IAnalyticsService service) =>
        {
            var stats = await service.GetByAudienceAsync();
            return Results.Ok(stats);
        })
        .WithSummary("Get user distribution by membership tier");

        return endpoints;
    }
}