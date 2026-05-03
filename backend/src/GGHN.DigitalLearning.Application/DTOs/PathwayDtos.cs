using System.ComponentModel.DataAnnotations;

namespace GGHN.DigitalLearning.Application.DTOs;

public class PathwayDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public string LearningObjective { get; set; } = string.Empty;
    public int EstimatedDurationMinutes { get; set; }
    public string? ImageUrl { get; set; }
    public int ResourceCount { get; set; }
    public List<ResourceSummaryDto> Resources { get; set; } = [];
    public DateTime CreatedAt { get; set; }
}

public class ResourceSummaryDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
}

public class CreatePathwayRequest
{
    [Required, MaxLength(500)]
    public string Title { get; set; } = string.Empty;
    [Required, MaxLength(2000)]
    public string Description { get; set; } = string.Empty;
    [Required, MaxLength(200)]
    public string Topic { get; set; } = string.Empty;
    [Required, MaxLength(2000)]
    public string LearningObjective { get; set; } = string.Empty;
    [Range(1, 100000)]
    public int EstimatedDurationMinutes { get; set; }
    public string? ImageUrl { get; set; }
    public List<Guid> ResourceIds { get; set; } = [];
}