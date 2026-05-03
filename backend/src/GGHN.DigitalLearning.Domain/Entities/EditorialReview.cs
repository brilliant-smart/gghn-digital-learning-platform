using GGHN.DigitalLearning.Domain.Common;
using GGHN.DigitalLearning.Domain.Enums;

namespace GGHN.DigitalLearning.Domain.Entities;

public class EditorialReview : BaseEntity
{
    public ContentStatus Status { get; set; } = ContentStatus.UnderReview;
    public string? ReviewNotes { get; set; }

    public Guid ResourceId { get; set; }
    public Resource Resource { get; set; } = null!;

    public string? ReviewerId { get; set; }
    public ApplicationUser? Reviewer { get; set; }

    public DateTime? ReviewedAt { get; set; }
}