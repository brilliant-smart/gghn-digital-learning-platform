using GGHN.DigitalLearning.Application.DTOs;
using GGHN.DigitalLearning.Application.Interfaces;
using GGHN.DigitalLearning.Domain.Entities;
using GGHN.DigitalLearning.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GGHN.DigitalLearning.Infrastructure.Services;

public class ProgressService : IProgressService
{
    private readonly AppDbContext _context;

    public ProgressService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProgressDto>> GetUserProgressAsync(string userId)
    {
        return await _context.UserProgress
            .Include(p => p.Course)
            .Include(p => p.Lesson)
            .Include(p => p.Pathway)
            .Where(p => p.UserId == userId)
            .Select(p => new ProgressDto
            {
                Id = p.Id,
                CourseId = p.CourseId,
                CourseTitle = p.Course != null ? p.Course.Title : null,
                LessonId = p.LessonId,
                LessonTitle = p.Lesson != null ? p.Lesson.Title : null,
                PathwayId = p.PathwayId,
                PathwayTitle = p.Pathway != null ? p.Pathway.Title : null,
                IsCompleted = p.IsCompleted,
                CompletedAt = p.CompletedAt,
                CertificateUrl = p.CertificateUrl
            }).ToListAsync();
    }

    public async Task<ProgressDto> MarkLessonCompleteAsync(string userId, MarkLessonCompleteRequest request)
    {
        var existing = await _context.UserProgress
            .FirstOrDefaultAsync(p => p.UserId == userId && p.LessonId == request.LessonId);

        if (existing != null)
        {
            existing.IsCompleted = true;
            existing.CompletedAt = DateTime.UtcNow;
            existing.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var lesson = await _context.Lessons.FindAsync(request.LessonId);
            return new ProgressDto
            {
                Id = existing.Id,
                CourseId = request.CourseId,
                LessonId = request.LessonId,
                LessonTitle = lesson?.Title,
                IsCompleted = true,
                CompletedAt = existing.CompletedAt
            };
        }

        var progress = new UserProgress
        {
            UserId = userId,
            CourseId = request.CourseId,
            LessonId = request.LessonId,
            IsCompleted = true,
            CompletedAt = DateTime.UtcNow
        };

        _context.UserProgress.Add(progress);
        await _context.SaveChangesAsync();

        var savedLesson = await _context.Lessons.FindAsync(request.LessonId);
        return new ProgressDto
        {
            Id = progress.Id,
            CourseId = request.CourseId,
            LessonId = request.LessonId,
            LessonTitle = savedLesson?.Title,
            IsCompleted = true,
            CompletedAt = progress.CompletedAt
        };
    }

    public async Task<ProgressDto> MarkPathwayCompleteAsync(string userId, Guid pathwayId)
    {
        var existing = await _context.UserProgress
            .FirstOrDefaultAsync(p => p.UserId == userId && p.PathwayId == pathwayId);

        if (existing != null)
        {
            existing.IsCompleted = true;
            existing.CompletedAt = DateTime.UtcNow;
            existing.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var pathway = await _context.Pathways.FindAsync(pathwayId);
            return new ProgressDto
            {
                Id = existing.Id,
                PathwayId = pathwayId,
                PathwayTitle = pathway?.Title,
                IsCompleted = true,
                CompletedAt = existing.CompletedAt
            };
        }

        var progress = new UserProgress
        {
            UserId = userId,
            PathwayId = pathwayId,
            IsCompleted = true,
            CompletedAt = DateTime.UtcNow,
            CertificateUrl = $"/certificates/{userId}/{pathwayId}"
        };

        _context.UserProgress.Add(progress);
        await _context.SaveChangesAsync();

        var savedPathway = await _context.Pathways.FindAsync(pathwayId);
        return new ProgressDto
        {
            Id = progress.Id,
            PathwayId = pathwayId,
            PathwayTitle = savedPathway?.Title,
            IsCompleted = true,
            CompletedAt = progress.CompletedAt,
            CertificateUrl = progress.CertificateUrl
        };
    }
}