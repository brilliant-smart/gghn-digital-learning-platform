using GGHN.DigitalLearning.Domain.Common;
using GGHN.DigitalLearning.Domain.Enums;

namespace GGHN.DigitalLearning.Domain.Entities;

public class Pathway : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public string LearningObjective { get; set; } = string.Empty;
    public int EstimatedDurationMinutes { get; set; }
    public string? ImageUrl { get; set; }
    public ContentStatus Status { get; set; } = ContentStatus.Published;

    public ICollection<PathwayResource> PathwayResources { get; set; } = [];
}