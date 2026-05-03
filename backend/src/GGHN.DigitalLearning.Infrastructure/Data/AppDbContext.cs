using GGHN.DigitalLearning.Domain.Entities;
using GGHN.DigitalLearning.Infrastructure.Data.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GGHN.DigitalLearning.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Resource> Resources => Set<Resource>();
    public DbSet<ResourceTakeaway> ResourceTakeaways => Set<ResourceTakeaway>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<Pathway> Pathways => Set<Pathway>();
    public DbSet<PathwayResource> PathwayResources => Set<PathwayResource>();
    public DbSet<Template> Templates => Set<Template>();
    public DbSet<Publication> Publications => Set<Publication>();
    public DbSet<Conference> Conferences => Set<Conference>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<Speaker> Speakers => Set<Speaker>();
    public DbSet<Sponsor> Sponsors => Set<Sponsor>();
    public DbSet<UserProgress> UserProgress => Set<UserProgress>();
    public DbSet<Discussion> Discussions => Set<Discussion>();
    public DbSet<EditorialReview> EditorialReviews => Set<EditorialReview>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<ResourceView> ResourceViews => Set<ResourceView>();
    public DbSet<PaymentTransaction> PaymentTransactions => Set<PaymentTransaction>();
    public DbSet<ConferenceRegistration> ConferenceRegistrations => Set<ConferenceRegistration>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationUserConfiguration).Assembly);

        modelBuilder.Entity<PathwayResource>(entity =>
        {
            entity.HasKey(pr => new { pr.PathwayId, pr.ResourceId });
            entity.HasOne(pr => pr.Pathway).WithMany(p => p.PathwayResources).HasForeignKey(pr => pr.PathwayId);
            entity.HasOne(pr => pr.Resource).WithMany(r => r.PathwayResources).HasForeignKey(pr => pr.ResourceId);
        });

        modelBuilder.Entity<UserProgress>(entity =>
        {
            entity.HasOne(up => up.User).WithMany(u => u.Progress).HasForeignKey(up => up.UserId);
            entity.HasOne(up => up.Course).WithMany(c => c.Progress).HasForeignKey(up => up.CourseId);
            entity.HasOne(up => up.Lesson).WithMany(l => l.Progress).HasForeignKey(up => up.LessonId);
            entity.HasOne(up => up.Pathway).WithMany().HasForeignKey(up => up.PathwayId);
        });

        modelBuilder.Entity<Discussion>(entity =>
        {
            entity.HasOne(d => d.Parent).WithMany(d => d.Replies).HasForeignKey(d => d.ParentId);
            entity.HasOne(d => d.Resource).WithMany(r => r.Discussions).HasForeignKey(d => d.ResourceId);
            entity.HasOne(d => d.User).WithMany(u => u.Discussions).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<EditorialReview>(entity =>
        {
            entity.HasOne(er => er.Resource).WithMany(r => r.Reviews).HasForeignKey(er => er.ResourceId);
            entity.HasOne(er => er.Reviewer).WithMany().HasForeignKey(er => er.ReviewerId);
        });

        modelBuilder.Entity<Resource>(entity =>
        {
            entity.HasOne(r => r.Contributor).WithMany(u => u.ContributedResources).HasForeignKey(r => r.ContributorId);
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasOne(s => s.Speaker).WithMany(sp => sp.Sessions).HasForeignKey(s => s.SpeakerId);
        });

        modelBuilder.Entity<Sponsor>(entity =>
        {
            entity.HasOne(s => s.Conference).WithMany(c => c.Sponsors).HasForeignKey(s => s.ConferenceId);
        });

        modelBuilder.Entity<ResourceTakeaway>(entity =>
        {
            entity.HasOne(rt => rt.Resource).WithMany(r => r.Takeaways).HasForeignKey(rt => rt.ResourceId);
        });

        modelBuilder.Entity<Lesson>(entity =>
        {
            entity.HasOne(l => l.Course).WithMany(c => c.Lessons).HasForeignKey(l => l.CourseId);
        });

        modelBuilder.Entity<ResourceView>(entity =>
        {
            entity.HasOne(rv => rv.Resource).WithMany().HasForeignKey(rv => rv.ResourceId);
            entity.HasOne(rv => rv.User).WithMany().HasForeignKey(rv => rv.UserId);
            entity.HasIndex(rv => rv.ResourceId);
            entity.HasIndex(rv => rv.ViewedAt);
        });

        modelBuilder.Entity<PaymentTransaction>(entity =>
        {
            entity.HasOne(pt => pt.User).WithMany().HasForeignKey(pt => pt.UserId);
            entity.HasOne(pt => pt.Template).WithMany().HasForeignKey(pt => pt.TemplateId);
            entity.HasIndex(pt => pt.Reference).IsUnique();
            entity.Property(pt => pt.Amount).HasPrecision(18, 2);
        });

        modelBuilder.Entity<ConferenceRegistration>(entity =>
        {
            entity.HasOne(r => r.Conference).WithMany().HasForeignKey(r => r.ConferenceId);
            entity.HasOne(r => r.User).WithMany().HasForeignKey(r => r.UserId);
            entity.HasIndex(r => r.ConferenceId);
            entity.HasIndex(r => r.Status);
            entity.HasIndex(r => r.Email);
        });
    }
}