using GGHN.DigitalLearning.Application.DTOs;
using GGHN.DigitalLearning.Application.Interfaces;
using GGHN.DigitalLearning.Domain.Entities;
using GGHN.DigitalLearning.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GGHN.DigitalLearning.Infrastructure.Services;

public class PathwayService : IPathwayService
{
    private readonly AppDbContext _context;

    public PathwayService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PathwayDto>> GetAllAsync()
    {
        return await _context.Pathways
            .Include(p => p.PathwayResources).ThenInclude(pr => pr.Resource)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => MapToDto(p))
            .ToListAsync();
    }

    public async Task<PathwayDto?> GetByIdAsync(Guid id)
    {
        var pathway = await _context.Pathways
            .Include(p => p.PathwayResources).ThenInclude(pr => pr.Resource)
            .FirstOrDefaultAsync(p => p.Id == id);

        return pathway == null ? null : MapToDto(pathway);
    }

    public async Task<PathwayDto> CreateAsync(CreatePathwayRequest request)
    {
        var pathway = new Pathway
        {
            Title = request.Title,
            Description = request.Description,
            Topic = request.Topic,
            LearningObjective = request.LearningObjective,
            EstimatedDurationMinutes = request.EstimatedDurationMinutes,
            ImageUrl = request.ImageUrl,
            PathwayResources = request.ResourceIds.Select((rid, i) => new PathwayResource
            {
                ResourceId = rid,
                Order = i + 1
            }).ToList()
        };

        _context.Pathways.Add(pathway);
        await _context.SaveChangesAsync();

        var saved = await _context.Pathways
            .Include(p => p.PathwayResources).ThenInclude(pr => pr.Resource)
            .FirstAsync(p => p.Id == pathway.Id);

        return MapToDto(saved);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var pathway = await _context.Pathways.FindAsync(id);
        if (pathway == null) return false;

        _context.Pathways.Remove(pathway);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<PathwayDto?> UpdateAsync(Guid id, CreatePathwayRequest request)
    {
        var pathway = await _context.Pathways.FindAsync(id);
        if (pathway == null) return null;

        pathway.Title = request.Title;
        pathway.Description = request.Description;
        pathway.Topic = request.Topic;
        pathway.LearningObjective = request.LearningObjective;
        pathway.EstimatedDurationMinutes = request.EstimatedDurationMinutes;
        pathway.ImageUrl = request.ImageUrl;
        pathway.UpdatedAt = DateTime.UtcNow;

        var existingResources = await _context.PathwayResources
            .Where(pr => pr.PathwayId == id).ToListAsync();
        _context.PathwayResources.RemoveRange(existingResources);

        pathway.PathwayResources = request.ResourceIds.Select((rid, i) => new PathwayResource
        {
            PathwayId = id,
            ResourceId = rid,
            Order = i + 1
        }).ToList();

        await _context.SaveChangesAsync();

        var updated = await _context.Pathways
            .Include(p => p.PathwayResources).ThenInclude(pr => pr.Resource)
            .FirstAsync(p => p.Id == id);

        return MapToDto(updated);
    }

    private static PathwayDto MapToDto(Pathway p) => new()
    {
        Id = p.Id,
        Title = p.Title,
        Description = p.Description,
        Topic = p.Topic,
        LearningObjective = p.LearningObjective,
        EstimatedDurationMinutes = p.EstimatedDurationMinutes,
        ImageUrl = p.ImageUrl,
        ResourceCount = p.PathwayResources?.Count ?? 0,
        Resources = p.PathwayResources?
            .OrderBy(pr => pr.Order)
            .Select(pr => new ResourceSummaryDto
            {
                Id = pr.Resource.Id,
                Title = pr.Resource.Title,
                Topic = pr.Resource.Topic,
                Audience = pr.Resource.Audience.ToString(),
                Difficulty = pr.Resource.Difficulty.ToString()
            }).ToList() ?? [],
        CreatedAt = p.CreatedAt
    };
}