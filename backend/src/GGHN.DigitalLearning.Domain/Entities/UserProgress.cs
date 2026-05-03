using GGHN.DigitalLearning.Domain.Common;
using GGHN.DigitalLearning.Domain.Enums;

namespace GGHN.DigitalLearning.Domain.Entities;

public class UserProgress : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public Guid? CourseId { get; set; }
    public Course? Course { get; set; }

    public Guid? LessonId { get; set; }
    public Lesson? Lesson { get; set; }

    public Guid? PathwayId { get; set; }
    public Pathway? Pathway { get; set; }

    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? CertificateUrl { get; set; }
}