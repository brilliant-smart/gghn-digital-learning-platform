using GGHN.DigitalLearning.Domain.Common;

namespace GGHN.DigitalLearning.Domain.Entities;

public class Lesson : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public int Order { get; set; }
    public string? ContentUrl { get; set; }
    public string? Description { get; set; }
    public bool IsPublished { get; set; }

    public Guid CourseId { get; set; }
    public Course Course { get; set; } = null!;

    public ICollection<UserProgress> Progress { get; set; } = [];
}