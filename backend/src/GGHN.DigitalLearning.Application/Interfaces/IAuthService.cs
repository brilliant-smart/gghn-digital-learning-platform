using GGHN.DigitalLearning.Application.DTOs;

namespace GGHN.DigitalLearning.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RefreshTokenAsync(string refreshToken);
    Task<UserDto> GetCurrentUserAsync(string userId);
    Task<IEnumerable<string>> GetUserRolesAsync(string userId);
    Task AssignRoleAsync(string userId, string role);
    Task<PagedResult<UserDto>> GetAllUsersAsync(int page = 1, int pageSize = 20);
    Task<bool> UpdateUserRoleAsync(string userId, string role);
    Task<bool> UpdateUserTierAsync(string userId, string tier);
    Task<bool> DeleteUserAsync(string userId);
    Task<UserDto> UpdateProfileAsync(string userId, UpdateProfileRequest request);
}