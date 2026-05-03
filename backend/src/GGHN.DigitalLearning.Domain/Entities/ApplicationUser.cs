using GGHN.DigitalLearning.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace GGHN.DigitalLearning.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public MembershipTier MembershipTier { get; set; } = MembershipTier.Free;
    public string? Organization { get; set; }
    public string? JobTitle { get; set; }
    public string? Country { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<UserProgress> Progress { get; set; } = [];
    public ICollection<Resource> ContributedResources { get; set; } = [];
    public ICollection<Discussion> Discussions { get; set; } = [];
    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
}