using System.ComponentModel.DataAnnotations;

namespace GGHN.DigitalLearning.Application.DTOs;

public class CreateSessionRequest
{
    [Required, MaxLength(500)]
    public string Title { get; set; } = string.Empty;
    [MaxLength(2000)]
    public string? Description { get; set; }
    [Required, MaxLength(200)]
    public string Track { get; set; } = string.Empty;
    [Required]
    public DateTime StartTime { get; set; }
    [Required]
    public DateTime EndTime { get; set; }
    [MaxLength(500)]
    public string? Location { get; set; }
    [MaxLength(1000)]
    public string? VirtualLink { get; set; }
    [Required]
    public Guid ConferenceId { get; set; }
    public Guid? SpeakerId { get; set; }
}

public class UpdateSessionRequest
{
    [Required, MaxLength(500)]
    public string Title { get; set; } = string.Empty;
    [MaxLength(2000)]
    public string? Description { get; set; }
    [Required, MaxLength(200)]
    public string Track { get; set; } = string.Empty;
    [Required]
    public DateTime StartTime { get; set; }
    [Required]
    public DateTime EndTime { get; set; }
    [MaxLength(500)]
    public string? Location { get; set; }
    [MaxLength(1000)]
    public string? VirtualLink { get; set; }
    [MaxLength(1000)]
    public string? RecordingUrl { get; set; }
    [MaxLength(1000)]
    public string? SlideDeckUrl { get; set; }
    [MaxLength(3000)]
    public string? SessionSummary { get; set; }
    public bool IsPublished { get; set; }
    public Guid? SpeakerId { get; set; }
}