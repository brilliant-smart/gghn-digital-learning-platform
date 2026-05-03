using GGHN.DigitalLearning.Domain.Common;
using GGHN.DigitalLearning.Domain.Enums;

namespace GGHN.DigitalLearning.Domain.Entities;

public class Sponsor : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public SponsorTier Tier { get; set; }
    public string? LogoUrl { get; set; }
    public string? WebsiteUrl { get; set; }

    public Guid ConferenceId { get; set; }
    public Conference Conference { get; set; } = null!;
}