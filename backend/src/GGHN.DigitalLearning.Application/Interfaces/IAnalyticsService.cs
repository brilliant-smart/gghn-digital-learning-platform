using GGHN.DigitalLearning.Application.DTOs;

namespace GGHN.DigitalLearning.Application.Interfaces;

public interface IAnalyticsService
{
    Task<DashboardStatsDto> GetDashboardStatsAsync();
    Task<IEnumerable<TopResourceDto>> GetTopResourcesAsync(int count = 10);
    Task<IEnumerable<TopPathwayDto>> GetTopPathwaysAsync(int count = 10);
    Task<IEnumerable<GeographyStatDto>> GetByGeographyAsync();
    Task<IEnumerable<AudienceStatDto>> GetByAudienceAsync();
}