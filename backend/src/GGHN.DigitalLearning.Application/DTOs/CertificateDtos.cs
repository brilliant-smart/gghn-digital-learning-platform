namespace GGHN.DigitalLearning.Application.DTOs;

public class CertificateDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string ItemTitle { get; set; } = string.Empty;
    public string ItemType { get; set; } = string.Empty;
    public DateTime CompletedAt { get; set; }
    public string CertificateUrl { get; set; } = string.Empty;
}