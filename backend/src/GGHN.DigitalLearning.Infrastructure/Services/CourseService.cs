using GGHN.DigitalLearning.Application.DTOs;
using GGHN.DigitalLearning.Application.Interfaces;
using GGHN.DigitalLearning.Domain.Entities;
using GGHN.DigitalLearning.Domain.Enums;
using GGHN.DigitalLearning.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GGHN.DigitalLearning.Infrastructure.Services;

public class CourseService : ICourseService
{
    private readonly AppDbContext _context;

    public CourseService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CourseDto>> GetAllAsync()
    {
        return await _context.Courses
            .Include(c => c.Lessons)
            .Where(c => c.Status == ContentStatus.Published)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => MapToDto(c))
            .ToListAsync();
    }

    public async Task<CourseDto?> GetByIdAsync(Guid id)
    {
        var course = await _context.Courses
            .Include(c => c.Lessons.OrderBy(l => l.Order))
            .FirstOrDefaultAsync(c => c.Id == id);

        return course == null ? null : MapToDto(course);
    }

    public async Task<CourseDto> CreateAsync(CreateCourseRequest request)
    {
        var course = new Course
        {
            Title = request.Title,
            Description = request.Description,
            Topic = request.Topic,
            Difficulty = Enum.Parse<Difficulty>(request.Difficulty),
            DurationMinutes = request.DurationMinutes,
            RequiredTier = Enum.Parse<MembershipTier>(request.RequiredTier),
            ImageUrl = request.ImageUrl,
            Status = ContentStatus.Draft,
            Lessons = request.Lessons.Select(l => new Lesson
            {
                Title = l.Title,
                DurationMinutes = l.DurationMinutes,
                Order = l.Order,
                ContentUrl = l.ContentUrl,
                Description = l.Description
            }).ToList()
        };

        _context.Courses.Add(course);
        await _context.SaveChangesAsync();

        return MapToDto(course);
    }

    public async Task<CourseDto?> UpdateAsync(Guid id, CreateCourseRequest request)
    {
        var course = await _context.Courses.Include(c => c.Lessons).FirstOrDefaultAsync(c => c.Id == id);
        if (course == null) return null;

        course.Title = request.Title;
        course.Description = request.Description;
        course.Topic = request.Topic;
        course.Difficulty = Enum.Parse<Difficulty>(request.Difficulty);
        course.DurationMinutes = request.DurationMinutes;
        course.RequiredTier = Enum.Parse<MembershipTier>(request.RequiredTier);
        course.ImageUrl = request.ImageUrl;
        course.UpdatedAt = DateTime.UtcNow;

        _context.Lessons.RemoveRange(course.Lessons);
        course.Lessons = request.Lessons.Select(l => new Lesson
        {
            Title = l.Title,
            DurationMinutes = l.DurationMinutes,
            Order = l.Order,
            ContentUrl = l.ContentUrl,
            Description = l.Description,
            CourseId = id
        }).ToList();

        await _context.SaveChangesAsync();
        return MapToDto(course);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null) return false;

        _context.Courses.Remove(course);
        await _context.SaveChangesAsync();
        return true;
    }

    private static CourseDto MapToDto(Course c) => new()
    {
        Id = c.Id,
        Title = c.Title,
        Description = c.Description,
        Topic = c.Topic,
        Difficulty = c.Difficulty.ToString(),
        DurationMinutes = c.DurationMinutes,
        RequiredTier = c.RequiredTier.ToString(),
        ImageUrl = c.ImageUrl,
        Lessons = c.Lessons?.Select(l => new LessonDto
        {
            Id = l.Id,
            Title = l.Title,
            DurationMinutes = l.DurationMinutes,
            Order = l.Order,
            ContentUrl = l.ContentUrl,
            Description = l.Description,
            IsPublished = l.IsPublished
        }).OrderBy(l => l.Order).ToList() ?? [],
        CreatedAt = c.CreatedAt
    };
}