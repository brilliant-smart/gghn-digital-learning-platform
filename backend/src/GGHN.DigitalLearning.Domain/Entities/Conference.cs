using GGHN.DigitalLearning.Domain.Common;

namespace GGHN.DigitalLearning.Domain.Entities;

public class Conference : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Theme { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Venue { get; set; } = string.Empty;
    public string? RegistrationUrl { get; set; }
    public int Year { get; set; }
    public bool IsArchived { get; set; }
    public string? ImageUrl { get; set; }

    public ICollection<Session> Sessions { get; set; } = [];
    public ICollection<Sponsor> Sponsors { get; set; } = [];
}