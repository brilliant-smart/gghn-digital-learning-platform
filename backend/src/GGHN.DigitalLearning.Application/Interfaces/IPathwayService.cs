using GGHN.DigitalLearning.Application.DTOs;

namespace GGHN.DigitalLearning.Application.Interfaces;

public interface IPathwayService
{
    Task<IEnumerable<PathwayDto>> GetAllAsync();
    Task<PathwayDto?> GetByIdAsync(Guid id);
    Task<PathwayDto> CreateAsync(CreatePathwayRequest request);
    Task<PathwayDto?> UpdateAsync(Guid id, CreatePathwayRequest request);
    Task<bool> DeleteAsync(Guid id);
}