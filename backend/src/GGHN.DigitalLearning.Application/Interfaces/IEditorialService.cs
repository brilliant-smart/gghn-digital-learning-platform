using GGHN.DigitalLearning.Application.DTOs;

namespace GGHN.DigitalLearning.Application.Interfaces;

public interface IEditorialService
{
    Task<IEnumerable<ResourceInReviewDto>> GetQueueAsync();
    Task<EditorialReviewDto> SubmitForReviewAsync(Guid resourceId, string userId);
    Task<EditorialReviewDto> CreateReviewAsync(CreateReviewRequest request, string reviewerId);
    Task<EditorialReviewDto?> UpdateReviewAsync(Guid id, UpdateReviewRequest request);
    Task<bool> ApproveAsync(Guid resourceId, string reviewerId);
    Task<bool> RejectAsync(Guid resourceId, string reviewerId, string reason);

    Task<IEnumerable<PublicationDto>> GetPublicationQueueAsync();
    Task<bool> SubmitPublicationForReviewAsync(Guid publicationId, string userId);
    Task<bool> ApprovePublicationAsync(Guid publicationId, string reviewerId);
    Task<bool> RejectPublicationAsync(Guid publicationId, string reviewerId, string reason);
}