using System.ComponentModel.DataAnnotations;

namespace GGHN.DigitalLearning.Application.DTOs;

public class CourseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public string RequiredTier { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public List<LessonDto> Lessons { get; set; } = [];
    public DateTime CreatedAt { get; set; }
}

public class LessonDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public int Order { get; set; }
    public string? ContentUrl { get; set; }
    public string? Description { get; set; }
    public bool IsPublished { get; set; }
}

public class CreateCourseRequest
{
    [Required, MaxLength(500)]
    public string Title { get; set; } = string.Empty;
    [Required, MaxLength(2000)]
    public string Description { get; set; } = string.Empty;
    [Required, MaxLength(200)]
    public string Topic { get; set; } = string.Empty;
    [Required]
    public string Difficulty { get; set; } = string.Empty;
    [Range(1, 10000)]
    public int DurationMinutes { get; set; }
    [Required]
    public string RequiredTier { get; set; } = "Free";
    public string? ImageUrl { get; set; }
    public List<CreateLessonRequest> Lessons { get; set; } = [];
}

public class CreateLessonRequest
{
    [Required, MaxLength(500)]
    public string Title { get; set; } = string.Empty;
    [Range(1, 10000)]
    public int DurationMinutes { get; set; }
    [Range(1, 10000)]
    public int Order { get; set; }
    public string? ContentUrl { get; set; }
    public string? Description { get; set; }
}