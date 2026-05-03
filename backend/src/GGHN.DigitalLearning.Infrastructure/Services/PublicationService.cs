using System.Text.Json;
using GGHN.DigitalLearning.Application.DTOs;
using GGHN.DigitalLearning.Application.Interfaces;
using GGHN.DigitalLearning.Domain.Entities;
using GGHN.DigitalLearning.Domain.Enums;
using GGHN.DigitalLearning.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GGHN.DigitalLearning.Infrastructure.Services;

public class PublicationService : IPublicationService
{
    private readonly AppDbContext _context;

    public PublicationService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<PublicationDto>> GetAllAsync(PublicationFilterParams filter)
    {
        var query = _context.Publications
            .Where(p => p.Status == ContentStatus.Published)
            .AsQueryable();

        if (!string.IsNullOrEmpty(filter.Search))
            query = query.Where(p => p.Title.Contains(filter.Search) || p.Summary.Contains(filter.Search));

        if (!string.IsNullOrEmpty(filter.Type))
            query = query.Where(p => p.PublicationType == filter.Type);

        if (!string.IsNullOrEmpty(filter.Tag))
            query = query.Where(p => p.Tags != null && p.Tags.Contains(filter.Tag));

        if (filter.Year.HasValue)
            query = query.Where(p => p.Year == filter.Year.Value);

        var page = filter.Page ?? 1;
        var pageSize = filter.PageSize ?? 20;

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(p => p.PublishedAt ?? p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => MapToDto(p))
            .ToListAsync();

        return new PagedResult<PublicationDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<PublicationDto?> GetByIdAsync(Guid id)
    {
        var publication = await _context.Publications.FindAsync(id);
        return publication == null ? null : MapToDto(publication);
    }

    public async Task<PublicationDto> CreateAsync(CreatePublicationRequest request)
    {
        var publication = new Publication
        {
            Title = request.Title,
            Summary = request.Summary,
            Content = request.Content,
            Author = request.Author,
            Status = ContentStatus.Draft,
            ImageUrl = request.ImageUrl,
            PublicationType = request.PublicationType,
            Tags = request.Tags != null ? JsonSerializer.Serialize(request.Tags) : null,
            KeyFindings = request.KeyFindings != null ? JsonSerializer.Serialize(request.KeyFindings) : null,
            ExternalUrl = request.ExternalUrl,
            Year = request.Year
        };

        _context.Publications.Add(publication);
        await _context.SaveChangesAsync();

        return MapToDto(publication);
    }

    public async Task<PublicationDto?> UpdateAsync(Guid id, UpdatePublicationRequest request)
    {
        var publication = await _context.Publications.FindAsync(id);
        if (publication == null) return null;

        publication.Title = request.Title;
        publication.Summary = request.Summary;
        publication.Content = request.Content;
        publication.Author = request.Author;
        publication.ImageUrl = request.ImageUrl;
        publication.PublicationType = request.PublicationType;
        publication.Tags = request.Tags != null ? JsonSerializer.Serialize(request.Tags) : null;
        publication.KeyFindings = request.KeyFindings != null ? JsonSerializer.Serialize(request.KeyFindings) : null;
        publication.ExternalUrl = request.ExternalUrl;
        publication.Year = request.Year;
        publication.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return MapToDto(publication);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var publication = await _context.Publications.FindAsync(id);
        if (publication == null) return false;

        _context.Publications.Remove(publication);
        await _context.SaveChangesAsync();
        return true;
    }

    private static List<string> ParseJsonList(string? json)
    {
        if (string.IsNullOrEmpty(json)) return [];
        try { return JsonSerializer.Deserialize<List<string>>(json) ?? []; }
        catch { return []; }
    }

    private static PublicationDto MapToDto(Publication p) => new()
    {
        Id = p.Id,
        Title = p.Title,
        Summary = p.Summary,
        Content = p.Content,
        Author = p.Author,
        Status = p.Status.ToString(),
        PublishedAt = p.PublishedAt,
        ImageUrl = p.ImageUrl,
        PublicationType = p.PublicationType,
        Tags = ParseJsonList(p.Tags),
        KeyFindings = ParseJsonList(p.KeyFindings),
        ExternalUrl = p.ExternalUrl,
        Year = p.Year,
        CreatedAt = p.CreatedAt
    };
}