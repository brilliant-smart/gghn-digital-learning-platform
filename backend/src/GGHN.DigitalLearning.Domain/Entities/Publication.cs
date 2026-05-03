using GGHN.DigitalLearning.Domain.Common;
using GGHN.DigitalLearning.Domain.Enums;

namespace GGHN.DigitalLearning.Domain.Entities;

public class Publication : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public ContentStatus Status { get; set; } = ContentStatus.Draft;
    public DateTime? PublishedAt { get; set; }
    public string? ImageUrl { get; set; }
    public string? PublicationType { get; set; }
    public string? Tags { get; set; }
    public string? KeyFindings { get; set; }
    public string? ExternalUrl { get; set; }
    public int? Year { get; set; }
}