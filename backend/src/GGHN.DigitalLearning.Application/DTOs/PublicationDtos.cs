using System.ComponentModel.DataAnnotations;

namespace GGHN.DigitalLearning.Application.DTOs;

public class PublicationDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? PublishedAt { get; set; }
    public string? ImageUrl { get; set; }
    public string? PublicationType { get; set; }
    public List<string> Tags { get; set; } = [];
    public List<string> KeyFindings { get; set; } = [];
    public string? ExternalUrl { get; set; }
    public int? Year { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreatePublicationRequest
{
    [Required, MaxLength(500)]
    public string Title { get; set; } = string.Empty;
    [Required, MaxLength(2000)]
    public string Summary { get; set; } = string.Empty;
    [Required]
    public string Content { get; set; } = string.Empty;
    [Required, MaxLength(200)]
    public string Author { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    [MaxLength(100)]
    public string? PublicationType { get; set; }
    public List<string>? Tags { get; set; }
    public List<string>? KeyFindings { get; set; }
    [MaxLength(1000)]
    public string? ExternalUrl { get; set; }
    public int? Year { get; set; }
}

public class UpdatePublicationRequest
{
    [Required, MaxLength(500)]
    public string Title { get; set; } = string.Empty;
    [Required, MaxLength(2000)]
    public string Summary { get; set; } = string.Empty;
    [Required]
    public string Content { get; set; } = string.Empty;
    [Required, MaxLength(200)]
    public string Author { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    [MaxLength(100)]
    public string? PublicationType { get; set; }
    public List<string>? Tags { get; set; }
    public List<string>? KeyFindings { get; set; }
    [MaxLength(1000)]
    public string? ExternalUrl { get; set; }
    public int? Year { get; set; }
}

public class PublicationFilterParams
{
    public string? Search { get; set; }
    public string? Type { get; set; }
    public string? Tag { get; set; }
    public int? Year { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }
}