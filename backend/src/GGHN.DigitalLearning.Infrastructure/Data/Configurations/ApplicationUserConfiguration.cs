using GGHN.DigitalLearning.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GGHN.DigitalLearning.Infrastructure.Data.Configurations;

public class ResourceConfiguration : IEntityTypeConfiguration<Resource>
{
    public void Configure(EntityTypeBuilder<Resource> builder)
    {
        builder.Property(r => r.Title).IsRequired().HasMaxLength(500);
        builder.Property(r => r.Summary).IsRequired().HasMaxLength(2000);
        builder.Property(r => r.PlainLanguageSummary).IsRequired().HasMaxLength(3000);
        builder.Property(r => r.SourceUrl).IsRequired().HasMaxLength(1000);
        builder.Property(r => r.Topic).IsRequired().HasMaxLength(200);
        builder.Property(r => r.Geography).HasMaxLength(200);
        builder.Property(r => r.Format).HasMaxLength(50);

        builder.HasIndex(r => r.Topic);
        builder.HasIndex(r => r.Audience);
        builder.HasIndex(r => r.Difficulty);
        builder.HasIndex(r => r.Status);
    }
}

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.Property(c => c.Title).IsRequired().HasMaxLength(500);
        builder.Property(c => c.Description).IsRequired().HasMaxLength(2000);
        builder.Property(c => c.Topic).IsRequired().HasMaxLength(200);
        builder.Property(c => c.ImageUrl).HasMaxLength(1000);

        builder.HasIndex(c => c.Topic);
        builder.HasIndex(c => c.Status);
    }
}

public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
{
    public void Configure(EntityTypeBuilder<Lesson> builder)
    {
        builder.Property(l => l.Title).IsRequired().HasMaxLength(500);
        builder.Property(l => l.ContentUrl).HasMaxLength(1000);
        builder.Property(l => l.Description).HasMaxLength(2000);

        builder.HasIndex(l => new { l.CourseId, l.Order }).IsUnique();
    }
}

public class PathwayConfiguration : IEntityTypeConfiguration<Pathway>
{
    public void Configure(EntityTypeBuilder<Pathway> builder)
    {
        builder.Property(p => p.Title).IsRequired().HasMaxLength(500);
        builder.Property(p => p.Description).IsRequired().HasMaxLength(2000);
        builder.Property(p => p.Topic).IsRequired().HasMaxLength(200);
        builder.Property(p => p.LearningObjective).IsRequired().HasMaxLength(1000);
        builder.Property(p => p.ImageUrl).HasMaxLength(1000);

        builder.HasIndex(p => p.Topic);
    }
}

public class TemplateConfiguration : IEntityTypeConfiguration<Template>
{
    public void Configure(EntityTypeBuilder<Template> builder)
    {
        builder.Property(t => t.Title).IsRequired().HasMaxLength(500);
        builder.Property(t => t.Description).IsRequired().HasMaxLength(2000);
        builder.Property(t => t.Format).IsRequired().HasMaxLength(20);
        builder.Property(t => t.FileUrl).HasMaxLength(1000);
        builder.Property(t => t.GuidanceNotesUrl).HasMaxLength(1000);
        builder.Property(t => t.WorkedExampleUrl).HasMaxLength(1000);

        builder.Property(t => t.Price).HasPrecision(18, 2);
        builder.HasIndex(t => t.Tier);
    }
}

public class PublicationConfiguration : IEntityTypeConfiguration<Publication>
{
    public void Configure(EntityTypeBuilder<Publication> builder)
    {
        builder.Property(p => p.Title).IsRequired().HasMaxLength(500);
        builder.Property(p => p.Summary).IsRequired().HasMaxLength(2000);
        builder.Property(p => p.Author).IsRequired().HasMaxLength(200);
        builder.Property(p => p.ImageUrl).HasMaxLength(1000);
        builder.Property(p => p.PublicationType).HasMaxLength(100);
        builder.Property(p => p.Tags).HasMaxLength(1000);
        builder.Property(p => p.KeyFindings).HasMaxLength(3000);
        builder.Property(p => p.ExternalUrl).HasMaxLength(1000);

        builder.HasIndex(p => p.Status);
    }
}

public class ConferenceConfiguration : IEntityTypeConfiguration<Conference>
{
    public void Configure(EntityTypeBuilder<Conference> builder)
    {
        builder.Property(c => c.Title).IsRequired().HasMaxLength(500);
        builder.Property(c => c.Theme).IsRequired().HasMaxLength(500);
        builder.Property(c => c.Description).IsRequired().HasMaxLength(3000);
        builder.Property(c => c.Venue).IsRequired().HasMaxLength(500);
        builder.Property(c => c.RegistrationUrl).HasMaxLength(1000);
        builder.Property(c => c.ImageUrl).HasMaxLength(1000);

        builder.HasIndex(c => c.Year);
    }
}

public class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.Property(s => s.Title).IsRequired().HasMaxLength(500);
        builder.Property(s => s.Description).HasMaxLength(2000);
        builder.Property(s => s.Track).IsRequired().HasMaxLength(200);
        builder.Property(s => s.Location).HasMaxLength(500);
        builder.Property(s => s.VirtualLink).HasMaxLength(1000);
        builder.Property(s => s.RecordingUrl).HasMaxLength(1000);
        builder.Property(s => s.SlideDeckUrl).HasMaxLength(1000);
        builder.Property(s => s.SessionSummary).HasMaxLength(3000);

        builder.HasIndex(s => s.ConferenceId);
    }
}

public class SpeakerConfiguration : IEntityTypeConfiguration<Speaker>
{
    public void Configure(EntityTypeBuilder<Speaker> builder)
    {
        builder.Property(s => s.Name).IsRequired().HasMaxLength(200);
        builder.Property(s => s.Title).HasMaxLength(200);
        builder.Property(s => s.Bio).HasMaxLength(3000);
        builder.Property(s => s.Organization).IsRequired().HasMaxLength(200);
        builder.Property(s => s.PhotoUrl).HasMaxLength(1000);
    }
}

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(u => u.LastName).IsRequired().HasMaxLength(100);
        builder.Property(u => u.Organization).HasMaxLength(200);
        builder.Property(u => u.JobTitle).HasMaxLength(200);
        builder.Property(u => u.Country).HasMaxLength(100);

        builder.HasIndex(u => u.MembershipTier);

        builder.HasMany(u => u.RefreshTokens)
            .WithOne()
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class SponsorConfiguration : IEntityTypeConfiguration<Sponsor>
{
    public void Configure(EntityTypeBuilder<Sponsor> builder)
    {
        builder.Property(s => s.Name).IsRequired().HasMaxLength(200);
        builder.Property(s => s.Tier).IsRequired().HasConversion<string>().HasMaxLength(50);
        builder.Property(s => s.LogoUrl).HasMaxLength(500);
        builder.Property(s => s.WebsiteUrl).HasMaxLength(500);

        builder.HasIndex(s => s.ConferenceId);
    }
}

public class ConferenceRegistrationConfiguration : IEntityTypeConfiguration<ConferenceRegistration>
{
    public void Configure(EntityTypeBuilder<ConferenceRegistration> builder)
    {
        builder.Property(r => r.FirstName).HasMaxLength(200);
        builder.Property(r => r.LastName).HasMaxLength(200);
        builder.Property(r => r.Email).HasMaxLength(300);
        builder.Property(r => r.Organization).HasMaxLength(300);
        builder.Property(r => r.JobTitle).HasMaxLength(200);
        builder.Property(r => r.Country).HasMaxLength(100);
        builder.Property(r => r.PhoneNumber).HasMaxLength(50);
        builder.Property(r => r.RegistrationType).HasMaxLength(50);
        builder.Property(r => r.Status).HasMaxLength(50);
        builder.Property(r => r.DietaryRestrictions).HasMaxLength(500);
        builder.Property(r => r.AccessibilityNeeds).HasMaxLength(500);
        builder.Property(r => r.SpecialRequests).HasMaxLength(1000);
        builder.Property(r => r.RejectionReason).HasMaxLength(1000);
        builder.Property(r => r.Notes).HasMaxLength(1000);
    }
}

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.Property(rt => rt.Token).IsRequired().HasMaxLength(500);
        builder.Property(rt => rt.UserId).IsRequired();

        builder.HasIndex(rt => rt.Token).IsUnique();
        builder.HasIndex(rt => rt.UserId);
    }
}