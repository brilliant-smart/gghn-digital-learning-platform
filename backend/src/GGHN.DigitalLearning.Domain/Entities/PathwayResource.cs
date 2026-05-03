namespace GGHN.DigitalLearning.Domain.Entities;

public class PathwayResource
{
    public Guid PathwayId { get; set; }
    public Pathway Pathway { get; set; } = null!;

    public Guid ResourceId { get; set; }
    public Resource Resource { get; set; } = null!;

    public int Order { get; set; }
}