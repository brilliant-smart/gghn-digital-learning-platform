using GGHN.DigitalLearning.Application.DTOs;
using GGHN.DigitalLearning.Application.Interfaces;
using GGHN.DigitalLearning.Domain.Entities;
using GGHN.DigitalLearning.Domain.Enums;
using GGHN.DigitalLearning.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GGHN.DigitalLearning.Infrastructure.Services;

public class EditorialService : IEditorialService
{
    private readonly AppDbContext _context;

    public EditorialService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ResourceInReviewDto>> GetQueueAsync()
    {
        return await _context.Resources
            .Where(r => r.Status == ContentStatus.Draft || r.Status == ContentStatus.UnderReview)
            .OrderBy(r => r.CreatedAt)
            .Select(r => new ResourceInReviewDto
            {
                Id = r.Id,
                Title = r.Title,
                Summary = r.Summary,
                Status = r.Status.ToString(),
                ContributorId = r.ContributorId,
                CreatedAt = r.CreatedAt
            }).ToListAsync();
    }

    public async Task<EditorialReviewDto> SubmitForReviewAsync(Guid resourceId, string userId)
    {
        var resource = await _context.Resources.FindAsync(resourceId);
        if (resource == null) throw new InvalidOperationException("Resource not found");

        resource.Status = ContentStatus.UnderReview;
        resource.UpdatedAt = DateTime.UtcNow;

        var review = new EditorialReview
        {
            ResourceId = resourceId,
            Status = ContentStatus.UnderReview
        };

        _context.EditorialReviews.Add(review);
        await _context.SaveChangesAsync();

        return MapToDto(review);
    }

    public async Task<EditorialReviewDto> CreateReviewAsync(CreateReviewRequest request, string reviewerId)
    {
        var review = new EditorialReview
        {
            ResourceId = request.ResourceId,
            ReviewerId = reviewerId,
            ReviewNotes = request.ReviewNotes,
            Status = ContentStatus.UnderReview,
            ReviewedAt = DateTime.UtcNow
        };

        _context.EditorialReviews.Add(review);
        await _context.SaveChangesAsync();

        return MapToDto(review);
    }

    public async Task<EditorialReviewDto?> UpdateReviewAsync(Guid id, UpdateReviewRequest request)
    {
        var review = await _context.EditorialReviews.FindAsync(id);
        if (review == null) return null;

        review.ReviewNotes = request.ReviewNotes;
        review.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return MapToDto(review);
    }

    public async Task<bool> ApproveAsync(Guid resourceId, string reviewerId)
    {
        var resource = await _context.Resources.FindAsync(resourceId);
        if (resource == null) return false;

        resource.Status = ContentStatus.Published;
        resource.UpdatedAt = DateTime.UtcNow;
        resource.PublicationDate ??= DateTime.UtcNow;

        var review = await _context.EditorialReviews
            .FirstOrDefaultAsync(r => r.ResourceId == resourceId && r.Status == ContentStatus.UnderReview);

        if (review != null)
        {
            review.Status = ContentStatus.Published;
            review.ReviewerId = reviewerId;
            review.ReviewedAt = DateTime.UtcNow;
            review.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RejectAsync(Guid resourceId, string reviewerId, string reason)
    {
        var resource = await _context.Resources.FindAsync(resourceId);
        if (resource == null) return false;

        resource.Status = ContentStatus.Draft;
        resource.UpdatedAt = DateTime.UtcNow;

        var review = await _context.EditorialReviews
            .FirstOrDefaultAsync(r => r.ResourceId == resourceId && r.Status == ContentStatus.UnderReview);

        if (review != null)
        {
            review.Status = ContentStatus.Archived;
            review.ReviewerId = reviewerId;
            review.ReviewNotes = reason;
            review.ReviewedAt = DateTime.UtcNow;
            review.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<PublicationDto>> GetPublicationQueueAsync()
    {
        return await _context.Publications
            .Where(p => p.Status == ContentStatus.Draft || p.Status == ContentStatus.UnderReview)
            .OrderBy(p => p.CreatedAt)
            .Select(p => new PublicationDto
            {
                Id = p.Id,
                Title = p.Title,
                Summary = p.Summary,
                Author = p.Author,
                Status = p.Status.ToString(),
                PublicationType = p.PublicationType,
                Year = p.Year,
                CreatedAt = p.CreatedAt
            }).ToListAsync();
    }

    public async Task<bool> SubmitPublicationForReviewAsync(Guid publicationId, string userId)
    {
        var publication = await _context.Publications.FindAsync(publicationId);
        if (publication == null) return false;
        publication.Status = ContentStatus.UnderReview;
        publication.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ApprovePublicationAsync(Guid publicationId, string reviewerId)
    {
        var publication = await _context.Publications.FindAsync(publicationId);
        if (publication == null) return false;
        publication.Status = ContentStatus.Published;
        publication.PublishedAt = DateTime.UtcNow;
        publication.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RejectPublicationAsync(Guid publicationId, string reviewerId, string reason)
    {
        var publication = await _context.Publications.FindAsync(publicationId);
        if (publication == null) return false;
        publication.Status = ContentStatus.Draft;
        publication.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    private static EditorialReviewDto MapToDto(EditorialReview r) => new()
    {
        Id = r.Id,
        Status = r.Status.ToString(),
        ReviewNotes = r.ReviewNotes,
        ResourceId = r.ResourceId,
        ReviewerId = r.ReviewerId,
        ReviewedAt = r.ReviewedAt,
        CreatedAt = r.CreatedAt
    };
}