using System.ComponentModel.DataAnnotations;

namespace GGHN.DigitalLearning.Application.DTOs;

public class CreateRegistrationRequest
{
    [Required] public Guid ConferenceId { get; set; }
    [Required, MaxLength(200)] public string FirstName { get; set; } = string.Empty;
    [Required, MaxLength(200)] public string LastName { get; set; } = string.Empty;
    [Required, MaxLength(300), EmailAddress] public string Email { get; set; } = string.Empty;
    [Required, MaxLength(300)] public string Organization { get; set; } = string.Empty;
    [Required, MaxLength(200)] public string JobTitle { get; set; } = string.Empty;
    [Required, MaxLength(100)] public string Country { get; set; } = string.Empty;
    [MaxLength(50)] public string? PhoneNumber { get; set; }
    [MaxLength(50)] public string RegistrationType { get; set; } = "General";
    [MaxLength(500)] public string? DietaryRestrictions { get; set; }
    [MaxLength(500)] public string? AccessibilityNeeds { get; set; }
    [MaxLength(1000)] public string? SpecialRequests { get; set; }
}

public class RegistrationDto
{
    public Guid Id { get; set; }
    public Guid ConferenceId { get; set; }
    public string? ConferenceTitle { get; set; }
    public string? UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Organization { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string RegistrationType { get; set; } = "General";
    public string Status { get; set; } = "Pending";
    public string? DietaryRestrictions { get; set; }
    public string? AccessibilityNeeds { get; set; }
    public string? SpecialRequests { get; set; }
    public string? ReviewedBy { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? RejectionReason { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class UpdateRegistrationStatusRequest
{
    [Required] public string Status { get; set; } = string.Empty;
    [MaxLength(1000)] public string? RejectionReason { get; set; }
    [MaxLength(1000)] public string? Notes { get; set; }
}

public class RegistrationStatsDto
{
    public int TotalRegistrations { get; set; }
    public int Pending { get; set; }
    public int Approved { get; set; }
    public int Rejected { get; set; }
    public int Waitlisted { get; set; }
}
