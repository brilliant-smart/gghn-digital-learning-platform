using GGHN.DigitalLearning.Application.DTOs;
using GGHN.DigitalLearning.Application.Interfaces;
using GGHN.DigitalLearning.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GGHN.DigitalLearning.Infrastructure.Services;

public class CertificateService : ICertificateService
{
    private readonly AppDbContext _context;

    public CertificateService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CertificateDto?> GetCertificateAsync(Guid progressId, string userId)
    {
        var progress = await _context.UserProgress
            .Include(p => p.Course)
            .Include(p => p.Pathway)
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == progressId);

        if (progress == null || progress.UserId != userId || !progress.IsCompleted)
            return null;

        var itemTitle = progress.Pathway?.Title ?? progress.Course?.Title ?? "Unknown";
        var itemType = progress.PathwayId != null ? "Pathway" : "Course";

        return new CertificateDto
        {
            Id = progress.Id,
            UserName = $"{progress.User.FirstName} {progress.User.LastName}",
            ItemTitle = itemTitle,
            ItemType = itemType,
            CompletedAt = progress.CompletedAt ?? progress.CreatedAt,
            CertificateUrl = progress.CertificateUrl ?? $"/certificates/{userId}/{progress.PathwayId ?? progress.CourseId}"
        };
    }
}