using GGHN.DigitalLearning.Application.DTOs;

namespace GGHN.DigitalLearning.Application.Interfaces;

public interface ICourseService
{
    Task<IEnumerable<CourseDto>> GetAllAsync();
    Task<CourseDto?> GetByIdAsync(Guid id);
    Task<CourseDto> CreateAsync(CreateCourseRequest request);
    Task<CourseDto?> UpdateAsync(Guid id, CreateCourseRequest request);
    Task<bool> DeleteAsync(Guid id);
}