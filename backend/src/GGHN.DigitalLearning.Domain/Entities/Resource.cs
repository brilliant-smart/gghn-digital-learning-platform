using GGHN.DigitalLearning.Domain.Common;
using GGHN.DigitalLearning.Domain.Enums;

namespace GGHN.DigitalLearning.Domain.Entities;

public class Resource : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string PlainLanguageSummary { get; set; } = string.Empty;
    public string SourceUrl { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public Audience Audience { get; set; }
    public Difficulty Difficulty { get; set; }
    public ContentStatus Status { get; set; } = ContentStatus.Draft;
    public string? Geography { get; set; }
    public string? Format { get; set; }
    public DateTime? PublicationDate { get; set; }

    public string? ContributorId { get; set; }

    public ApplicationUser? Contributor { get; set; }
    public ICollection<ResourceTakeaway> Takeaways { get; set; } = [];
    public ICollection<PathwayResource> PathwayResources { get; set; } = [];
    public ICollection<Discussion> Discussions { get; set; } = [];
    public ICollection<EditorialReview> Reviews { get; set; } = [];
}