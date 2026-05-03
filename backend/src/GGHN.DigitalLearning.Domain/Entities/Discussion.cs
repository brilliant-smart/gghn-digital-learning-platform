using GGHN.DigitalLearning.Domain.Common;

namespace GGHN.DigitalLearning.Domain.Entities;

public class Discussion : BaseEntity
{
    public string Content { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
    public Discussion? Parent { get; set; }
    public ICollection<Discussion> Replies { get; set; } = [];

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public Guid ResourceId { get; set; }
    public Resource Resource { get; set; } = null!;
}