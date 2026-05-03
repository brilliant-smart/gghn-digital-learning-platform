namespace GGHN.DigitalLearning.Application.DTOs;

public class ConferenceDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Theme { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Venue { get; set; } = string.Empty;
    public string? RegistrationUrl { get; set; }
    public int Year { get; set; }
    public bool IsArchived { get; set; }
    public string? ImageUrl { get; set; }
    public List<SessionDto> Sessions { get; set; } = [];
    public List<SponsorDto> Sponsors { get; set; } = [];
    public DateTime CreatedAt { get; set; }
}

public class SessionDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Track { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? Location { get; set; }
    public string? VirtualLink { get; set; }
    public string? RecordingUrl { get; set; }
    public string? SlideDeckUrl { get; set; }
    public string? SessionSummary { get; set; }
    public bool IsPublished { get; set; }
    public SpeakerDto? Speaker { get; set; }
}

public class SpeakerDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string Organization { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
}

public class SponsorDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Tier { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string? WebsiteUrl { get; set; }
}