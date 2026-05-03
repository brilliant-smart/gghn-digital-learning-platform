using GGHN.DigitalLearning.Domain.Common;

namespace GGHN.DigitalLearning.Domain.Entities;

public class ResourceView : BaseEntity
{
    public Guid ResourceId { get; set; }
    public Resource Resource { get; set; } = null!;
    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }
    public string? Country { get; set; }
    public DateTime ViewedAt { get; set; } = DateTime.UtcNow;
}