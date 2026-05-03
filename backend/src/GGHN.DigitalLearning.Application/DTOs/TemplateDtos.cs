using System.ComponentModel.DataAnnotations;

namespace GGHN.DigitalLearning.Application.DTOs;

public class TemplateDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Format { get; set; } = string.Empty;
    public string Tier { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public string? FileUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateTemplateRequest
{
    [Required, MaxLength(500)]
    public string Title { get; set; } = string.Empty;
    [Required, MaxLength(2000)]
    public string Description { get; set; } = string.Empty;
    [Required, MaxLength(100)]
    public string Format { get; set; } = string.Empty;
    [Required]
    public string Tier { get; set; } = "Free";
    [Range(0, 999999)]
    public decimal? Price { get; set; }
    public string? FileUrl { get; set; }
    public string? GuidanceNotesUrl { get; set; }
    public string? WorkedExampleUrl { get; set; }
}