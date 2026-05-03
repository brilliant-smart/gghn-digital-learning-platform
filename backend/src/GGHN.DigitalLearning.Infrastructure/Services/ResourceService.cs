using GGHN.DigitalLearning.Application.DTOs;
using GGHN.DigitalLearning.Application.Interfaces;
using GGHN.DigitalLearning.Domain.Entities;
using GGHN.DigitalLearning.Domain.Enums;
using GGHN.DigitalLearning.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GGHN.DigitalLearning.Infrastructure.Services;

public class ResourceService : IResourceService
{
    private readonly AppDbContext _context;

    public ResourceService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<ResourceDto>> GetAllAsync(ResourceFilterParams filter)
    {
        var query = _context.Resources
            .Include(r => r.Takeaways)
            .Where(r => r.Status == ContentStatus.Published)
            .AsQueryable();

        if (!string.IsNullOrEmpty(filter.Topic))
            query = query.Where(r => r.Topic == filter.Topic);

        if (!string.IsNullOrEmpty(filter.Audience) && Enum.TryParse<Audience>(filter.Audience, out var audience))
            query = query.Where(r => r.Audience == audience);

        if (!string.IsNullOrEmpty(filter.Difficulty) && Enum.TryParse<Difficulty>(filter.Difficulty, out var difficulty))
            query = query.Where(r => r.Difficulty == difficulty);

        if (!string.IsNullOrEmpty(filter.Search))
            query = query.Where(r => r.Title.Contains(filter.Search) || r.Summary.Contains(filter.Search));

        var page = filter.Page ?? 1;
        var pageSize = filter.PageSize ?? 20;

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(r => MapToDto(r))
            .ToListAsync();

        return new PagedResult<ResourceDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<ResourceDto?> GetByIdAsync(Guid id)
    {
        var resource = await _context.Resources
            .Include(r => r.Takeaways)
            .FirstOrDefaultAsync(r => r.Id == id);

        return resource == null ? null : MapToDto(resource);
    }

    public async Task<ResourceDto> CreateAsync(CreateResourceRequest request)
    {
        var resource = new Resource
        {
            Title = request.Title,
            Summary = request.Summary,
            PlainLanguageSummary = request.PlainLanguageSummary,
            SourceUrl = request.SourceUrl,
            Topic = request.Topic,
            Audience = Enum.Parse<Audience>(request.Audience),
            Difficulty = Enum.Parse<Difficulty>(request.Difficulty),
            Geography = request.Geography,
            Format = request.Format,
            PublicationDate = request.PublicationDate,
            Status = ContentStatus.Draft,
            Takeaways = request.Takeaways.Select((t, i) => new ResourceTakeaway { Content = t, Order = i + 1 }).ToList()
        };

        _context.Resources.Add(resource);
        await _context.SaveChangesAsync();

        return MapToDto(resource);
    }

    public async Task<ResourceDto?> UpdateAsync(Guid id, UpdateResourceRequest request)
    {
        var resource = await _context.Resources.Include(r => r.Takeaways).FirstOrDefaultAsync(r => r.Id == id);
        if (resource == null) return null;

        resource.Title = request.Title;
        resource.Summary = request.Summary;
        resource.PlainLanguageSummary = request.PlainLanguageSummary;
        resource.SourceUrl = request.SourceUrl;
        resource.Topic = request.Topic;
        resource.Audience = Enum.Parse<Audience>(request.Audience);
        resource.Difficulty = Enum.Parse<Difficulty>(request.Difficulty);
        resource.Geography = request.Geography;
        resource.Format = request.Format;
        resource.PublicationDate = request.PublicationDate;
        resource.UpdatedAt = DateTime.UtcNow;

        _context.ResourceTakeaways.RemoveRange(resource.Takeaways);
        resource.Takeaways = request.Takeaways.Select((t, i) => new ResourceTakeaway
        {
            Content = t,
            Order = i + 1,
            ResourceId = id
        }).ToList();

        await _context.SaveChangesAsync();
        return MapToDto(resource);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var resource = await _context.Resources.FindAsync(id);
        if (resource == null) return false;

        _context.Resources.Remove(resource);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task RecordViewAsync(Guid id, string? userId = null)
    {
        var resource = await _context.Resources.FindAsync(id);
        if (resource == null) return;

        var view = new ResourceView
        {
            ResourceId = id,
            UserId = userId,
            ViewedAt = DateTime.UtcNow
        };

        _context.ResourceViews.Add(view);
        await _context.SaveChangesAsync();
    }

    private static ResourceDto MapToDto(Resource r) => new()
    {
        Id = r.Id,
        Title = r.Title,
        Summary = r.Summary,
        PlainLanguageSummary = r.PlainLanguageSummary,
        SourceUrl = r.SourceUrl,
        Topic = r.Topic,
        Audience = r.Audience.ToString(),
        Difficulty = r.Difficulty.ToString(),
        Status = r.Status.ToString(),
        Geography = r.Geography,
        Format = r.Format,
        PublicationDate = r.PublicationDate,
        Takeaways = r.Takeaways?.OrderBy(t => t.Order).Select(t => t.Content).ToList() ?? [],
        CreatedAt = r.CreatedAt
    };
}