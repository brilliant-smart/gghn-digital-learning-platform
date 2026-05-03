using GGHN.DigitalLearning.Application.DTOs;

namespace GGHN.DigitalLearning.Application.Interfaces;

public interface IProgressService
{
    Task<IEnumerable<ProgressDto>> GetUserProgressAsync(string userId);
    Task<ProgressDto> MarkLessonCompleteAsync(string userId, MarkLessonCompleteRequest request);
    Task<ProgressDto> MarkPathwayCompleteAsync(string userId, Guid pathwayId);
}