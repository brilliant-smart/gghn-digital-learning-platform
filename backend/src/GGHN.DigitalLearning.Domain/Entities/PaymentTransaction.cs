using GGHN.DigitalLearning.Domain.Common;

namespace GGHN.DigitalLearning.Domain.Entities;

public class PaymentTransaction : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;
    public Guid? TemplateId { get; set; }
    public Template? Template { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "NGN";
    public string Reference { get; set; } = string.Empty;
    public string Status { get; set; } = "pending";
    public string? PaystackResponse { get; set; }
}