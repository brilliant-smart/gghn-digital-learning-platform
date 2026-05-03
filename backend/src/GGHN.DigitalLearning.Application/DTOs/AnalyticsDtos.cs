namespace GGHN.DigitalLearning.Application.DTOs;

public class DashboardStatsDto
{
    public int TotalUsers { get; set; }
    public int TotalResources { get; set; }
    public int TotalCourses { get; set; }
    public int TotalPathways { get; set; }
    public int TotalCompletions { get; set; }
    public int TotalDiscussions { get; set; }
    public int TotalPublications { get; set; }
    public int TotalConferences { get; set; }
}

public class TopResourceDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public int ViewCount { get; set; }
}

public class TopPathwayDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int CompletionCount { get; set; }
}

public class GeographyStatDto
{
    public string Country { get; set; } = string.Empty;
    public int UserCount { get; set; }
}

public class AudienceStatDto
{
    public string MembershipTier { get; set; } = string.Empty;
    public int UserCount { get; set; }
}