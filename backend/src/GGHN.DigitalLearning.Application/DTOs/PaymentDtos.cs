using System.ComponentModel.DataAnnotations;

namespace GGHN.DigitalLearning.Application.DTOs;

public class InitializePaymentRequest
{
    [Required]
    public Guid TemplateId { get; set; }
}

public class InitializePaymentResponse
{
    public string AuthorizationUrl { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
}

public class VerifyPaymentResponse
{
    public string Status { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public Guid? TemplateId { get; set; }
    public string? DownloadUrl { get; set; }
}