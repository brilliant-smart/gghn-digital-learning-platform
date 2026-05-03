using GGHN.DigitalLearning.Domain.Common;

namespace GGHN.DigitalLearning.Domain.Entities;

public class Session : BaseEntity
{
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

    public Guid ConferenceId { get; set; }
    public Conference Conference { get; set; } = null!;

    public Guid? SpeakerId { get; set; }
    public Speaker? Speaker { get; set; }
}