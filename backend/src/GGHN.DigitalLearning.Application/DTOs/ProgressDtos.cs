using System.ComponentModel.DataAnnotations;

namespace GGHN.DigitalLearning.Application.DTOs;

public class ProgressDto
{
    public Guid Id { get; set; }
    public Guid? CourseId { get; set; }
    public string? CourseTitle { get; set; }
    public Guid? LessonId { get; set; }
    public string? LessonTitle { get; set; }
    public Guid? PathwayId { get; set; }
    public string? PathwayTitle { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? CertificateUrl { get; set; }
}

public class MarkLessonCompleteRequest
{
    [Required]
    public Guid LessonId { get; set; }
    [Required]
    public Guid CourseId { get; set; }
}