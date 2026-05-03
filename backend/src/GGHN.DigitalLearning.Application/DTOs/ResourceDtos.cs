using System.ComponentModel.DataAnnotations;

namespace GGHN.DigitalLearning.Application.DTOs;

public class ResourceDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string PlainLanguageSummary { get; set; } = string.Empty;
    public string SourceUrl { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Geography { get; set; }
    public string? Format { get; set; }
    public DateTime? PublicationDate { get; set; }
    public List<string> Takeaways { get; set; } = [];
    public DateTime CreatedAt { get; set; }
}

public class CreateResourceRequest
{
    [Required, MaxLength(500)]
    public string Title { get; set; } = string.Empty;
    [Required, MaxLength(2000)]
    public string Summary { get; set; } = string.Empty;
    [Required, MaxLength(3000)]
    public string PlainLanguageSummary { get; set; } = string.Empty;
    [Required, MaxLength(1000)]
    public string SourceUrl { get; set; } = string.Empty;
    [Required, MaxLength(200)]
    public string Topic { get; set; } = string.Empty;
    [Required]
    public string Audience { get; set; } = string.Empty;
    [Required]
    public string Difficulty { get; set; } = string.Empty;
    public string? Geography { get; set; }
    public string? Format { get; set; }
    public DateTime? PublicationDate { get; set; }
    public List<string> Takeaways { get; set; } = [];
}

public class UpdateResourceRequest
{
    [Required, MaxLength(500)]
    public string Title { get; set; } = string.Empty;
    [Required, MaxLength(2000)]
    public string Summary { get; set; } = string.Empty;
    [Required, MaxLength(3000)]
    public string PlainLanguageSummary { get; set; } = string.Empty;
    [Required, MaxLength(1000)]
    public string SourceUrl { get; set; } = string.Empty;
    [Required, MaxLength(200)]
    public string Topic { get; set; } = string.Empty;
    [Required]
    public string Audience { get; set; } = string.Empty;
    [Required]
    public string Difficulty { get; set; } = string.Empty;
    public string? Geography { get; set; }
    public string? Format { get; set; }
    public DateTime? PublicationDate { get; set; }
    public List<string> Takeaways { get; set; } = [];
}

public class ResourceFilterParams
{
    public string? Topic { get; set; }
    public string? Audience { get; set; }
    public string? Difficulty { get; set; }
    public string? Search { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}