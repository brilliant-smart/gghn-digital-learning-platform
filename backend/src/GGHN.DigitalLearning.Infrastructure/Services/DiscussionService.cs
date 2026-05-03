using GGHN.DigitalLearning.Application.DTOs;
using GGHN.DigitalLearning.Application.Interfaces;
using GGHN.DigitalLearning.Domain.Entities;
using GGHN.DigitalLearning.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GGHN.DigitalLearning.Infrastructure.Services;

public class DiscussionService : IDiscussionService
{
    private readonly AppDbContext _context;

    public DiscussionService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<DiscussionDto>> GetByResourceAsync(Guid resourceId, int page = 1, int pageSize = 20)
    {
        var query = _context.Discussions
            .Include(d => d.User)
            .Include(d => d.Replies).ThenInclude(r => r.User)
            .Where(d => d.ResourceId == resourceId && d.ParentId == null)
            .OrderByDescending(d => d.CreatedAt);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(d => MapToDto(d))
            .ToListAsync();

        return new PagedResult<DiscussionDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<DiscussionDto?> GetByIdAsync(Guid id)
    {
        var discussion = await _context.Discussions
            .Include(d => d.User)
            .Include(d => d.Replies).ThenInclude(r => r.User)
            .FirstOrDefaultAsync(d => d.Id == id);

        return discussion == null ? null : MapToDto(discussion);
    }

    public async Task<DiscussionDto> CreateAsync(CreateDiscussionRequest request, string userId)
    {
        var discussion = new Discussion
        {
            Content = request.Content,
            ResourceId = request.ResourceId,
            UserId = userId
        };

        _context.Discussions.Add(discussion);
        await _context.SaveChangesAsync();

        var saved = await _context.Discussions
            .Include(d => d.User)
            .FirstAsync(d => d.Id == discussion.Id);

        return MapToDto(saved);
    }

    public async Task<DiscussionDto> ReplyAsync(Guid parentId, CreateReplyRequest request, string userId)
    {
        var parent = await _context.Discussions.FindAsync(parentId);
        if (parent == null) throw new InvalidOperationException("Parent discussion not found");

        var reply = new Discussion
        {
            Content = request.Content,
            ParentId = parentId,
            ResourceId = parent.ResourceId,
            UserId = userId
        };

        _context.Discussions.Add(reply);
        await _context.SaveChangesAsync();

        var saved = await _context.Discussions
            .Include(d => d.User)
            .FirstAsync(d => d.Id == reply.Id);

        return MapToDto(saved);
    }

    public async Task<DiscussionDto?> UpdateAsync(Guid id, UpdateDiscussionRequest request, string userId)
    {
        var discussion = await _context.Discussions.FindAsync(id);
        if (discussion == null || discussion.UserId != userId) return null;

        discussion.Content = request.Content;
        discussion.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var updated = await _context.Discussions
            .Include(d => d.User)
            .FirstAsync(d => d.Id == id);

        return MapToDto(updated);
    }

    public async Task<bool> DeleteAsync(Guid id, string userId, bool isAdmin)
    {
        var discussion = await _context.Discussions.FindAsync(id);
        if (discussion == null) return false;
        if (discussion.UserId != userId && !isAdmin) return false;

        _context.Discussions.Remove(discussion);
        await _context.SaveChangesAsync();
        return true;
    }

    private static DiscussionDto MapToDto(Discussion d) => new()
    {
        Id = d.Id,
        Content = d.Content,
        ParentId = d.ParentId,
        ResourceId = d.ResourceId,
        UserId = d.UserId,
        UserName = d.User?.FirstName + " " + d.User?.LastName ?? "",
        CreatedAt = d.CreatedAt,
        UpdatedAt = d.UpdatedAt,
        Replies = d.Replies?.Select(r => MapToDto(r)).ToList() ?? []
    };
}