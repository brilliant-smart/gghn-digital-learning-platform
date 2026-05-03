using GGHN.DigitalLearning.Application.DTOs;

namespace GGHN.DigitalLearning.Application.Interfaces;

public interface IRegistrationService
{
    Task<RegistrationDto> RegisterAsync(CreateRegistrationRequest request, string? userId);
    Task<RegistrationDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<RegistrationDto>> GetByConferenceAsync(Guid conferenceId, string? status = null);
    Task<IEnumerable<RegistrationDto>> GetMyRegistrationsAsync(string userId);
    Task<RegistrationDto?> UpdateStatusAsync(Guid id, UpdateRegistrationStatusRequest request, string reviewerId);
    Task<RegistrationStatsDto> GetStatsAsync(Guid conferenceId);
}
