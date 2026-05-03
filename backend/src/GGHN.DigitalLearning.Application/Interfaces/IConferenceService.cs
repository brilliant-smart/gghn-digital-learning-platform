using GGHN.DigitalLearning.Application.DTOs;

namespace GGHN.DigitalLearning.Application.Interfaces;

public interface IConferenceService
{
    Task<IEnumerable<ConferenceDto>> GetAllAsync();
    Task<ConferenceDto?> GetByIdAsync(Guid id);
    Task<ConferenceDto> CreateAsync(CreateConferenceRequest request);
    Task<ConferenceDto?> UpdateConferenceAsync(Guid id, UpdateConferenceRequest request);
    Task<bool> DeleteConferenceAsync(Guid id);
    Task<IEnumerable<SpeakerDto>> GetAllSpeakersAsync();
    Task<SpeakerDto> CreateSpeakerAsync(CreateSpeakerRequest request);
    Task<SessionDto> CreateSessionAsync(CreateSessionRequest request);
    Task<SessionDto?> UpdateSessionAsync(Guid id, UpdateSessionRequest request);
    Task<bool> DeleteSessionAsync(Guid id);
    Task<SessionDto?> GetSessionByIdAsync(Guid id);
    Task<SpeakerDto?> UpdateSpeakerAsync(Guid id, UpdateSpeakerRequest request);
    Task<bool> DeleteSpeakerAsync(Guid id);
    Task<SponsorDto> CreateSponsorAsync(CreateSponsorRequest request);
    Task<SponsorDto?> UpdateSponsorAsync(Guid id, UpdateSponsorRequest request);
    Task<bool> DeleteSponsorAsync(Guid id);
}