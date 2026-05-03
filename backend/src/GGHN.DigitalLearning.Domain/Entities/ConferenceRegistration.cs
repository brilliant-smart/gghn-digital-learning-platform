using GGHN.DigitalLearning.Domain.Common;

namespace GGHN.DigitalLearning.Domain.Entities;

public class ConferenceRegistration : BaseEntity
{
    public Guid ConferenceId { get; set; }
    public Conference Conference { get; set; } = null!;
    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }

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

    public DateTime? ConfirmationSentAt { get; set; }
    public string? Notes { get; set; }
}
