using System.ComponentModel.DataAnnotations;

namespace GGHN.DigitalLearning.Application.DTOs;

public class CreateConferenceRequest
{
    [Required, MaxLength(500)]
    public string Title { get; set; } = string.Empty;
    [Required, MaxLength(200)]
    public string Theme { get; set; } = string.Empty;
    [Required, MaxLength(2000)]
    public string Description { get; set; } = string.Empty;
    [Required]
    public DateTime StartDate { get; set; }
    [Required]
    public DateTime EndDate { get; set; }
    [Required, MaxLength(200)]
    public string Venue { get; set; } = string.Empty;
    public string? RegistrationUrl { get; set; }
    [Range(2000, 2100)]
    public int Year { get; set; }
}

public class CreateSpeakerRequest
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    public string? Bio { get; set; }
    [Required, MaxLength(200)]
    public string Organization { get; set; } = string.Empty;
}

public class UpdateSpeakerRequest
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    public string? Bio { get; set; }
    [Required, MaxLength(200)]
    public string Organization { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
}

public class UpdateConferenceRequest
{
    [Required, MaxLength(500)]
    public string Title { get; set; } = string.Empty;
    [Required, MaxLength(200)]
    public string Theme { get; set; } = string.Empty;
    [Required, MaxLength(2000)]
    public string Description { get; set; } = string.Empty;
    [Required]
    public DateTime StartDate { get; set; }
    [Required]
    public DateTime EndDate { get; set; }
    [Required, MaxLength(200)]
    public string Venue { get; set; } = string.Empty;
    [MaxLength(500)]
    public string? RegistrationUrl { get; set; }
    [Range(2000, 2100)]
    public int Year { get; set; }
    public bool IsArchived { get; set; }
    [MaxLength(1000)]
    public string? ImageUrl { get; set; }
}

public class CreateSponsorRequest
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    [Required]
    public string Tier { get; set; } = string.Empty;
    [MaxLength(500)]
    public string? LogoUrl { get; set; }
    [MaxLength(500)]
    public string? WebsiteUrl { get; set; }
    [Required]
    public Guid ConferenceId { get; set; }
}

public class UpdateSponsorRequest
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    [Required]
    public string Tier { get; set; } = string.Empty;
    [MaxLength(500)]
    public string? LogoUrl { get; set; }
    [MaxLength(500)]
    public string? WebsiteUrl { get; set; }
}