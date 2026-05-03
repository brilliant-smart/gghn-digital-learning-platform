using GGHN.DigitalLearning.Application.DTOs;
using GGHN.DigitalLearning.Application.Interfaces;
using GGHN.DigitalLearning.Domain.Enums;
using GGHN.DigitalLearning.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GGHN.DigitalLearning.Infrastructure.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly AppDbContext _context;

    public AnalyticsService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardStatsDto> GetDashboardStatsAsync()
    {
        return new DashboardStatsDto
        {
            TotalUsers = await _context.Users.CountAsync(),
            TotalResources = await _context.Resources.CountAsync(r => r.Status == ContentStatus.Published),
            TotalCourses = await _context.Courses.CountAsync(c => c.Status == ContentStatus.Published),
            TotalPathways = await _context.Pathways.CountAsync(p => p.Status == ContentStatus.Published),
            TotalCompletions = await _context.UserProgress.CountAsync(up => up.IsCompleted),
            TotalDiscussions = await _context.Discussions.CountAsync(),
            TotalPublications = await _context.Publications.CountAsync(p => p.Status == ContentStatus.Published),
            TotalConferences = await _context.Conferences.CountAsync()
        };
    }

    public async Task<IEnumerable<TopResourceDto>> GetTopResourcesAsync(int count = 10)
    {
        return await _context.Resources
            .Where(r => r.Status == ContentStatus.Published)
            .Select(r => new TopResourceDto
            {
                Id = r.Id,
                Title = r.Title,
                Topic = r.Topic,
                ViewCount = _context.ResourceViews.Count(rv => rv.ResourceId == r.Id)
            })
            .OrderByDescending(r => r.ViewCount)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<TopPathwayDto>> GetTopPathwaysAsync(int count = 10)
    {
        return await _context.Pathways
            .Where(p => p.Status == ContentStatus.Published)
            .Select(p => new TopPathwayDto
            {
                Id = p.Id,
                Title = p.Title,
                CompletionCount = p.PathwayResources.Count
            })
            .OrderByDescending(p => p.CompletionCount)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<GeographyStatDto>> GetByGeographyAsync()
    {
        return await _context.Users
            .Where(u => u.Country != null)
            .GroupBy(u => u.Country!)
            .Select(g => new GeographyStatDto
            {
                Country = g.Key,
                UserCount = g.Count()
            })
            .OrderByDescending(g => g.UserCount)
            .ToListAsync();
    }

    public async Task<IEnumerable<AudienceStatDto>> GetByAudienceAsync()
    {
        return await _context.Users
            .GroupBy(u => u.MembershipTier)
            .Select(g => new AudienceStatDto
            {
                MembershipTier = g.Key.ToString(),
                UserCount = g.Count()
            })
            .OrderByDescending(g => g.UserCount)
            .ToListAsync();
    }
}