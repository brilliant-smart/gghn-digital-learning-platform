using GGHN.DigitalLearning.Application.DTOs;

namespace GGHN.DigitalLearning.Application.Interfaces;

public interface ITemplateService
{
    Task<IEnumerable<TemplateDto>> GetAllAsync();
    Task<TemplateDto?> GetByIdAsync(Guid id);
    Task<TemplateDto> CreateAsync(CreateTemplateRequest request);
    Task<TemplateDto?> UpdateAsync(Guid id, CreateTemplateRequest request);
    Task<bool> DeleteAsync(Guid id);
}