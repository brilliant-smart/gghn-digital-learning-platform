using GGHN.DigitalLearning.Application.DTOs;

namespace GGHN.DigitalLearning.Application.Interfaces;

public interface IDiscussionService
{
    Task<PagedResult<DiscussionDto>> GetByResourceAsync(Guid resourceId, int page = 1, int pageSize = 20);
    Task<DiscussionDto?> GetByIdAsync(Guid id);
    Task<DiscussionDto> CreateAsync(CreateDiscussionRequest request, string userId);
    Task<DiscussionDto> ReplyAsync(Guid parentId, CreateReplyRequest request, string userId);
    Task<DiscussionDto?> UpdateAsync(Guid id, UpdateDiscussionRequest request, string userId);
    Task<bool> DeleteAsync(Guid id, string userId, bool isAdmin);
}