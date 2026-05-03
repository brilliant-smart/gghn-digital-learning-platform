using System.ComponentModel.DataAnnotations;

namespace GGHN.DigitalLearning.Application.DTOs;

public class DiscussionDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
    public Guid ResourceId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<DiscussionDto> Replies { get; set; } = [];
}

public class CreateDiscussionRequest
{
    [Required]
    public Guid ResourceId { get; set; }
    [Required, MaxLength(5000)]
    public string Content { get; set; } = string.Empty;
}

public class CreateReplyRequest
{
    [Required, MaxLength(5000)]
    public string Content { get; set; } = string.Empty;
}

public class UpdateDiscussionRequest
{
    [Required, MaxLength(5000)]
    public string Content { get; set; } = string.Empty;
}