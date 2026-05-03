using GGHN.DigitalLearning.Domain.Common;

namespace GGHN.DigitalLearning.Domain.Entities;

public class Speaker : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string Organization { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }

    public ICollection<Session> Sessions { get; set; } = [];
}