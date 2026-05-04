using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using GGHN.DigitalLearning.Application.DTOs;
using GGHN.DigitalLearning.Application.Exceptions;
using GGHN.DigitalLearning.Application.Interfaces;
using GGHN.DigitalLearning.Domain.Entities;
using GGHN.DigitalLearning.Domain.Enums;
using GGHN.DigitalLearning.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace GGHN.DigitalLearning.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly AppDbContext _context;

    public AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration, AppDbContext context)
    {
        _userManager = userManager;
        _configuration = configuration;
        _context = context;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
            throw new InvalidOperationException("A user with this email already exists.");

        var user = new ApplicationUser
        {
            Email = request.Email,
            UserName = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Organization = request.Organization,
            JobTitle = request.JobTitle,
            Country = request.Country
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            throw new RegistrationValidationException(TransformIdentityErrors(result.Errors));

        await _userManager.AddToRoleAsync(user, "FreeUser");

        var (accessToken, expiresAt) = await GenerateAccessTokenAsync(user);
        var refreshToken = await GenerateRefreshTokenAsync(user);

        return new AuthResponse
        {
            Token = accessToken,
            ExpiresAt = expiresAt,
            RefreshToken = refreshToken.Token,
            User = MapToUserDto(user, ["FreeUser"])
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
            throw new InvalidCredentialsException();

        if (await _userManager.IsLockedOutAsync(user))
            throw new AccountLockoutException(user.LockoutEnd);

        if (!user.EmailConfirmed)
            throw new EmailNotConfirmedException(user.Email!);

        var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!passwordValid)
        {
            await _userManager.AccessFailedAsync(user);

            if (await _userManager.IsLockedOutAsync(user))
                throw new AccountLockoutException(user.LockoutEnd);

            throw new InvalidCredentialsException();
        }

        await _userManager.ResetAccessFailedCountAsync(user);

        var roles = await _userManager.GetRolesAsync(user);
        var (accessToken, expiresAt) = await GenerateAccessTokenAsync(user);
        var refreshToken = await GenerateRefreshTokenAsync(user);

        return new AuthResponse
        {
            Token = accessToken,
            ExpiresAt = expiresAt,
            RefreshToken = refreshToken.Token,
            User = MapToUserDto(user, roles)
        };
    }

    public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
    {
        var storedToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (storedToken == null || !storedToken.IsActive)
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");

        var user = await _userManager.FindByIdAsync(storedToken.UserId)
            ?? throw new UnauthorizedAccessException("User not found.");

        storedToken.IsRevoked = true;

        var (accessToken, expiresAt) = await GenerateAccessTokenAsync(user);
        var newRefreshToken = await GenerateRefreshTokenAsync(user);

        storedToken.ReplacedByTokenId = newRefreshToken.Id;

        await _context.SaveChangesAsync();

        var roles = await _userManager.GetRolesAsync(user);

        return new AuthResponse
        {
            Token = accessToken,
            ExpiresAt = expiresAt,
            RefreshToken = newRefreshToken.Token,
            User = MapToUserDto(user, roles)
        };
    }

    public async Task<UserDto> GetCurrentUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found.");

        var roles = await _userManager.GetRolesAsync(user);
        return MapToUserDto(user, roles);
    }

    public async Task<IEnumerable<string>> GetUserRolesAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found.");

        return await _userManager.GetRolesAsync(user);
    }

    public async Task AssignRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found.");

        var result = await _userManager.AddToRoleAsync(user, role);
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));
    }

    public async Task<PagedResult<UserDto>> GetAllUsersAsync(int page = 1, int pageSize = 20)
    {
        var query = _userManager.Users.OrderBy(u => u.CreatedAt);
        var totalCount = await query.CountAsync();

        var users = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var userDtos = new List<UserDto>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userDtos.Add(MapToUserDto(user, roles));
        }

        return new PagedResult<UserDto>
        {
            Items = userDtos,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<bool> UpdateUserRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        var result = await _userManager.AddToRoleAsync(user, role);

        return result.Succeeded;
    }

    public async Task<bool> UpdateUserTierAsync(string userId, string tier)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        if (Enum.TryParse<MembershipTier>(tier, out var membershipTier))
        {
            user.MembershipTier = membershipTier;
            user.UpdatedAt = DateTime.UtcNow;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        return false;
    }

    public async Task<bool> DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded;
    }

    public async Task<UserDto> UpdateProfileAsync(string userId, UpdateProfileRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found.");

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Organization = request.Organization;
        user.JobTitle = request.JobTitle;
        user.Country = request.Country;
        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));

        var roles = await _userManager.GetRolesAsync(user);
        return MapToUserDto(user, roles);
    }

    public async Task ResendEmailConfirmationAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null || user.EmailConfirmed)
            return;

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        // TODO: Send email with confirmation link containing token.
    }

    private static List<string> TransformIdentityErrors(IEnumerable<IdentityError> errors)
    {
        var friendly = new List<string>();
        foreach (var error in errors)
        {
            var message = error.Code switch
            {
                "PasswordTooShort" => "Password must be at least 8 characters long.",
                "PasswordRequiresNonAlphanumeric" => "Password must include at least one special character (e.g. !@#$%).",
                "PasswordRequiresDigit" => "Password must include at least one number.",
                "PasswordRequiresLower" => "Password must include at least one lowercase letter.",
                "PasswordRequiresUpper" => "Password must include at least one uppercase letter.",
                "PasswordRequiresUniqueChars" => "Password must not contain repeated characters.",
                "DuplicateUserName" or "DuplicateEmail" => "An account with this email already exists.",
                "InvalidEmail" => "Please enter a valid email address.",
                _ => error.Description
            };
            friendly.Add(message);
        }
        return friendly;
    }

    private async Task<(string token, DateTime expiresAt)> GenerateAccessTokenAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("firstName", user.FirstName),
            new("lastName", user.LastName),
            new("membershipTier", user.MembershipTier.ToString())
        };

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var expiresAt = DateTime.UtcNow.AddMinutes(15);
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }

    private async Task<RefreshToken> GenerateRefreshTokenAsync(ApplicationUser user)
    {
        var refreshToken = new RefreshToken
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        return refreshToken;
    }

    private static UserDto MapToUserDto(ApplicationUser user, IList<string> roles) => new()
    {
        Id = user.Id,
        Email = user.Email!,
        FirstName = user.FirstName,
        LastName = user.LastName,
        MembershipTier = user.MembershipTier.ToString(),
        Organization = user.Organization,
        JobTitle = user.JobTitle,
        Country = user.Country,
        Roles = roles
    };
}