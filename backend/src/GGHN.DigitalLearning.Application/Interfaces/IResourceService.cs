using GGHN.DigitalLearning.Application.DTOs;

namespace GGHN.DigitalLearning.Application.Interfaces;

public interface IResourceService
{
    Task<PagedResult<ResourceDto>> GetAllAsync(ResourceFilterParams filter);
    Task<ResourceDto?> GetByIdAsync(Guid id);
    Task<ResourceDto> CreateAsync(CreateResourceRequest request);
    Task<ResourceDto?> UpdateAsync(Guid id, UpdateResourceRequest request);
    Task<bool> DeleteAsync(Guid id);
    Task RecordViewAsync(Guid id, string? userId = null);
}