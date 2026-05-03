using GGHN.DigitalLearning.Domain.Common;
using GGHN.DigitalLearning.Domain.Enums;

namespace GGHN.DigitalLearning.Domain.Entities;

public class Course : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public Difficulty Difficulty { get; set; }
    public int DurationMinutes { get; set; }
    public MembershipTier RequiredTier { get; set; } = MembershipTier.Free;
    public ContentStatus Status { get; set; } = ContentStatus.Draft;
    public string? ImageUrl { get; set; }

    public ICollection<Lesson> Lessons { get; set; } = [];
    public ICollection<UserProgress> Progress { get; set; } = [];
}