using System.ComponentModel.DataAnnotations;

namespace GGHN.DigitalLearning.Application.DTOs;

public class EditorialReviewDto
{
    public Guid Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ReviewNotes { get; set; }
    public Guid ResourceId { get; set; }
    public string? ReviewerId { get; set; }
    public string? ReviewerName { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ResourceInReviewDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? ContributorId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class SubmitForReviewRequest
{
    [Required]
    public Guid ResourceId { get; set; }
}

public class CreateReviewRequest
{
    [Required]
    public Guid ResourceId { get; set; }
    [Required, MaxLength(5000)]
    public string ReviewNotes { get; set; } = string.Empty;
}

public class UpdateReviewRequest
{
    [Required, MaxLength(5000)]
    public string ReviewNotes { get; set; } = string.Empty;
}