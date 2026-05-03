using GGHN.DigitalLearning.Application.DTOs;
using GGHN.DigitalLearning.Application.Interfaces;
using GGHN.DigitalLearning.Domain.Entities;
using GGHN.DigitalLearning.Domain.Enums;
using GGHN.DigitalLearning.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GGHN.DigitalLearning.Infrastructure.Services;

public class TemplateService : ITemplateService
{
    private readonly AppDbContext _context;

    public TemplateService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TemplateDto>> GetAllAsync()
    {
        return await _context.Templates
            .Where(t => t.Status == ContentStatus.Published)
            .OrderBy(t => t.Tier).ThenBy(t => t.Title)
            .Select(t => MapToDto(t))
            .ToListAsync();
    }

    public async Task<TemplateDto?> GetByIdAsync(Guid id)
    {
        var template = await _context.Templates.FindAsync(id);
        return template == null ? null : MapToDto(template);
    }

    public async Task<TemplateDto> CreateAsync(CreateTemplateRequest request)
    {
        var template = new Template
        {
            Title = request.Title,
            Description = request.Description,
            Format = request.Format,
            Tier = Enum.Parse<TemplateTier>(request.Tier),
            Price = request.Price,
            FileUrl = request.FileUrl,
            GuidanceNotesUrl = request.GuidanceNotesUrl,
            WorkedExampleUrl = request.WorkedExampleUrl
        };

        _context.Templates.Add(template);
        await _context.SaveChangesAsync();

        return MapToDto(template);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var template = await _context.Templates.FindAsync(id);
        if (template == null) return false;

        _context.Templates.Remove(template);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<TemplateDto?> UpdateAsync(Guid id, CreateTemplateRequest request)
    {
        var template = await _context.Templates.FindAsync(id);
        if (template == null) return null;

        template.Title = request.Title;
        template.Description = request.Description;
        template.Format = request.Format;
        template.Tier = Enum.Parse<TemplateTier>(request.Tier);
        template.Price = request.Price;
        template.FileUrl = request.FileUrl;
        template.GuidanceNotesUrl = request.GuidanceNotesUrl;
        template.WorkedExampleUrl = request.WorkedExampleUrl;
        template.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return MapToDto(template);
    }

    private static TemplateDto MapToDto(Template t) => new()
    {
        Id = t.Id,
        Title = t.Title,
        Description = t.Description,
        Format = t.Format,
        Tier = t.Tier.ToString(),
        Price = t.Price,
        FileUrl = t.FileUrl,
        CreatedAt = t.CreatedAt
    };
}