using GGHN.DigitalLearning.Domain.Common;

namespace GGHN.DigitalLearning.Domain.Entities;

public class ResourceTakeaway : BaseEntity
{
    public string Content { get; set; } = string.Empty;
    public int Order { get; set; }

    public Guid ResourceId { get; set; }
    public Resource Resource { get; set; } = null!;
}