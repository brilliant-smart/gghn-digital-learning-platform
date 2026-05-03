using GGHN.DigitalLearning.Domain.Common;
using GGHN.DigitalLearning.Domain.Enums;

namespace GGHN.DigitalLearning.Domain.Entities;

public class Template : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Format { get; set; } = string.Empty;
    public TemplateTier Tier { get; set; } = TemplateTier.Free;
    public decimal? Price { get; set; }
    public string? FileUrl { get; set; }
    public string? GuidanceNotesUrl { get; set; }
    public string? WorkedExampleUrl { get; set; }
    public ContentStatus Status { get; set; } = ContentStatus.Published;
}