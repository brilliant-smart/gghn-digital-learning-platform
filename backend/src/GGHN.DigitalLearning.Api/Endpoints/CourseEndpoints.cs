using GGHN.DigitalLearning.Application.DTOs;
using GGHN.DigitalLearning.Application.Interfaces;
using GGHN.DigitalLearning.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace GGHN.DigitalLearning.Api.Endpoints;

public static class CourseEndpoints
{
    public static IEndpointRouteBuilder MapCourseEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/courses").WithTags("Courses");

        group.MapGet("/", async (ICourseService service) =>
        {
            var courses = await service.GetAllAsync();
            return Results.Ok(courses);
        })
        .WithSummary("Get all published courses")
        .AllowAnonymous();

        group.MapGet("/{id:guid}", async (Guid id, ICourseService service, ClaimsPrincipal user) =>
        {
            var course = await service.GetByIdAsync(id);
            if (course is null) return Results.NotFound();

            if (course.RequiredTier == MembershipTier.Free.ToString()) return Results.Ok(course);

            if (user.Identity?.IsAuthenticated != true) return Results.Challenge();

            if (user.IsInRole("Admin") || user.IsInRole("Editor")) return Results.Ok(course);

            var tierClaim = user.FindFirst("membershipTier")?.Value;
            if (!Enum.TryParse<MembershipTier>(tierClaim, out var userTier))
                userTier = MembershipTier.Free;

            var requiredTier = Enum.Parse<MembershipTier>(course.RequiredTier);
            var tierOrder = new[] { MembershipTier.Free, MembershipTier.Member, MembershipTier.Institutional };

            return Array.IndexOf(tierOrder, userTier) >= Array.IndexOf(tierOrder, requiredTier)
                ? Results.Ok(course)
                : Results.Forbid();
        })
        .WithSummary("Get a course by ID with lessons");

        group.MapPost("/", async (CreateCourseRequest request, ICourseService service) =>
        {
            var course = await service.CreateAsync(request);
            return Results.Created($"/api/courses/{course.Id}", course);
        })
        .WithSummary("Create a new course")
        .RequireAuthorization("AdminOrEditor");

        group.MapPut("/{id:guid}", async (Guid id, CreateCourseRequest request, ICourseService service) =>
        {
            var course = await service.UpdateAsync(id, request);
            return course is null ? Results.NotFound() : Results.Ok(course);
        })
        .WithSummary("Update a course")
        .RequireAuthorization("AdminOrEditor");

        group.MapDelete("/{id:guid}", async (Guid id, ICourseService service) =>
        {
            var deleted = await service.DeleteAsync(id);
            return deleted ? Results.NoContent() : Results.NotFound();
        })
        .WithSummary("Delete a course")
        .RequireAuthorization("Admin");

        return endpoints;
    }
}