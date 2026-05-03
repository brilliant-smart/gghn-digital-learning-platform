using GGHN.DigitalLearning.Application.DTOs;

namespace GGHN.DigitalLearning.Application.Interfaces;

public interface ICertificateService
{
    Task<CertificateDto?> GetCertificateAsync(Guid progressId, string userId);
}