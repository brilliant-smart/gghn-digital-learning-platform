using GGHN.DigitalLearning.Application.DTOs;

namespace GGHN.DigitalLearning.Application.Interfaces;

public interface IPublicationService
{
    Task<PagedResult<PublicationDto>> GetAllAsync(PublicationFilterParams filter);
    Task<PublicationDto?> GetByIdAsync(Guid id);
    Task<PublicationDto> CreateAsync(CreatePublicationRequest request);
    Task<PublicationDto?> UpdateAsync(Guid id, UpdatePublicationRequest request);
    Task<bool> DeleteAsync(Guid id);
}