using GGHN.DigitalLearning.Application.DTOs;
using GGHN.DigitalLearning.Application.Interfaces;
using GGHN.DigitalLearning.Domain.Entities;
using GGHN.DigitalLearning.Domain.Enums;
using GGHN.DigitalLearning.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GGHN.DigitalLearning.Infrastructure.Services;

public class ConferenceService : IConferenceService
{
    private readonly AppDbContext _context;

    public ConferenceService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ConferenceDto>> GetAllAsync()
    {
        return await _context.Conferences
            .Include(c => c.Sessions).ThenInclude(s => s.Speaker)
            .Include(c => c.Sponsors)
            .OrderByDescending(c => c.Year)
            .Select(c => MapToDto(c))
            .ToListAsync();
    }

    public async Task<ConferenceDto?> GetByIdAsync(Guid id)
    {
        var conference = await _context.Conferences
            .Include(c => c.Sessions).ThenInclude(s => s.Speaker)
            .Include(c => c.Sponsors)
            .FirstOrDefaultAsync(c => c.Id == id);

        return conference == null ? null : MapToDto(conference);
    }

    public async Task<ConferenceDto> CreateAsync(CreateConferenceRequest request)
    {
        var conference = new Conference
        {
            Title = request.Title,
            Theme = request.Theme,
            Description = request.Description,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Venue = request.Venue,
            RegistrationUrl = request.RegistrationUrl,
            Year = request.Year
        };

        _context.Conferences.Add(conference);
        await _context.SaveChangesAsync();

        return MapToDto(conference);
    }

    public async Task<IEnumerable<SpeakerDto>> GetAllSpeakersAsync()
    {
        return await _context.Speakers
            .OrderBy(s => s.Name)
            .Select(s => new SpeakerDto
            {
                Id = s.Id,
                Name = s.Name,
                Title = s.Title,
                Bio = s.Bio,
                Organization = s.Organization,
                PhotoUrl = s.PhotoUrl
            }).ToListAsync();
    }

    public async Task<SpeakerDto> CreateSpeakerAsync(CreateSpeakerRequest request)
    {
        var speaker = new Speaker
        {
            Name = request.Name,
            Title = request.Title,
            Bio = request.Bio,
            Organization = request.Organization
        };

        _context.Speakers.Add(speaker);
        await _context.SaveChangesAsync();

        return new SpeakerDto
        {
            Id = speaker.Id,
            Name = speaker.Name,
            Title = speaker.Title,
            Bio = speaker.Bio,
            Organization = speaker.Organization
        };
    }

    public async Task<SessionDto> CreateSessionAsync(CreateSessionRequest request)
    {
        var session = new Session
        {
            Title = request.Title,
            Description = request.Description,
            Track = request.Track,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            Location = request.Location,
            VirtualLink = request.VirtualLink,
            ConferenceId = request.ConferenceId,
            SpeakerId = request.SpeakerId
        };

        _context.Sessions.Add(session);
        await _context.SaveChangesAsync();

        var saved = await _context.Sessions
            .Include(s => s.Speaker)
            .FirstAsync(s => s.Id == session.Id);

        return MapSessionToDto(saved);
    }

    public async Task<SessionDto?> UpdateSessionAsync(Guid id, UpdateSessionRequest request)
    {
        var session = await _context.Sessions.FindAsync(id);
        if (session == null) return null;

        session.Title = request.Title;
        session.Description = request.Description;
        session.Track = request.Track;
        session.StartTime = request.StartTime;
        session.EndTime = request.EndTime;
        session.Location = request.Location;
        session.VirtualLink = request.VirtualLink;
        session.RecordingUrl = request.RecordingUrl;
        session.SlideDeckUrl = request.SlideDeckUrl;
        session.SessionSummary = request.SessionSummary;
        session.IsPublished = request.IsPublished;
        session.SpeakerId = request.SpeakerId;
        session.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        var updated = await _context.Sessions.Include(s => s.Speaker).FirstAsync(s => s.Id == id);
        return MapSessionToDto(updated);
    }

    public async Task<bool> DeleteSessionAsync(Guid id)
    {
        var session = await _context.Sessions.FindAsync(id);
        if (session == null) return false;
        _context.Sessions.Remove(session);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<SessionDto?> GetSessionByIdAsync(Guid id)
    {
        var session = await _context.Sessions.Include(s => s.Speaker).FirstOrDefaultAsync(s => s.Id == id);
        return session == null ? null : MapSessionToDto(session);
    }

    public async Task<SpeakerDto?> UpdateSpeakerAsync(Guid id, UpdateSpeakerRequest request)
    {
        var speaker = await _context.Speakers.FindAsync(id);
        if (speaker == null) return null;

        speaker.Name = request.Name;
        speaker.Title = request.Title;
        speaker.Bio = request.Bio;
        speaker.Organization = request.Organization;
        speaker.PhotoUrl = request.PhotoUrl;
        speaker.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return new SpeakerDto
        {
            Id = speaker.Id,
            Name = speaker.Name,
            Title = speaker.Title,
            Bio = speaker.Bio,
            Organization = speaker.Organization,
            PhotoUrl = speaker.PhotoUrl
        };
    }

    public async Task<bool> DeleteSpeakerAsync(Guid id)
    {
        var speaker = await _context.Speakers.FindAsync(id);
        if (speaker == null) return false;
        _context.Speakers.Remove(speaker);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<ConferenceDto?> UpdateConferenceAsync(Guid id, UpdateConferenceRequest request)
    {
        var conf = await _context.Conferences.FindAsync(id);
        if (conf is null) return null;
        conf.Title = request.Title;
        conf.Theme = request.Theme;
        conf.Description = request.Description;
        conf.StartDate = request.StartDate;
        conf.EndDate = request.EndDate;
        conf.Venue = request.Venue;
        conf.RegistrationUrl = request.RegistrationUrl;
        conf.Year = request.Year;
        conf.IsArchived = request.IsArchived;
        conf.ImageUrl = request.ImageUrl;
        conf.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return MapToDto(conf);
    }

    public async Task<bool> DeleteConferenceAsync(Guid id)
    {
        var conf = await _context.Conferences.FindAsync(id);
        if (conf is null) return false;
        _context.Conferences.Remove(conf);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<SponsorDto> CreateSponsorAsync(CreateSponsorRequest request)
    {
        var sponsor = new Sponsor
        {
            Name = request.Name,
            Tier = Enum.Parse<SponsorTier>(request.Tier),
            LogoUrl = request.LogoUrl,
            WebsiteUrl = request.WebsiteUrl,
            ConferenceId = request.ConferenceId
        };
        _context.Sponsors.Add(sponsor);
        await _context.SaveChangesAsync();
        return new SponsorDto
        {
            Id = sponsor.Id,
            Name = sponsor.Name,
            Tier = sponsor.Tier.ToString(),
            LogoUrl = sponsor.LogoUrl,
            WebsiteUrl = sponsor.WebsiteUrl
        };
    }

    public async Task<SponsorDto?> UpdateSponsorAsync(Guid id, UpdateSponsorRequest request)
    {
        var sponsor = await _context.Sponsors.FindAsync(id);
        if (sponsor is null) return null;
        sponsor.Name = request.Name;
        sponsor.Tier = Enum.Parse<SponsorTier>(request.Tier);
        sponsor.LogoUrl = request.LogoUrl;
        sponsor.WebsiteUrl = request.WebsiteUrl;
        sponsor.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return new SponsorDto
        {
            Id = sponsor.Id,
            Name = sponsor.Name,
            Tier = sponsor.Tier.ToString(),
            LogoUrl = sponsor.LogoUrl,
            WebsiteUrl = sponsor.WebsiteUrl
        };
    }

    public async Task<bool> DeleteSponsorAsync(Guid id)
    {
        var sponsor = await _context.Sponsors.FindAsync(id);
        if (sponsor is null) return false;
        _context.Sponsors.Remove(sponsor);
        await _context.SaveChangesAsync();
        return true;
    }

    private static SessionDto MapSessionToDto(Session s) => new()
    {
        Id = s.Id,
        Title = s.Title,
        Description = s.Description,
        Track = s.Track,
        StartTime = s.StartTime,
        EndTime = s.EndTime,
        Location = s.Location,
        VirtualLink = s.VirtualLink,
        RecordingUrl = s.RecordingUrl,
        SlideDeckUrl = s.SlideDeckUrl,
        SessionSummary = s.SessionSummary,
        IsPublished = s.IsPublished,
        Speaker = s.Speaker != null ? new SpeakerDto
        {
            Id = s.Speaker.Id,
            Name = s.Speaker.Name,
            Title = s.Speaker.Title,
            Bio = s.Speaker.Bio,
            Organization = s.Speaker.Organization,
            PhotoUrl = s.Speaker.PhotoUrl
        } : null
    };

    private static ConferenceDto MapToDto(Conference c) => new()
    {
        Id = c.Id,
        Title = c.Title,
        Theme = c.Theme,
        Description = c.Description,
        StartDate = c.StartDate,
        EndDate = c.EndDate,
        Venue = c.Venue,
        RegistrationUrl = c.RegistrationUrl,
        Year = c.Year,
        IsArchived = c.IsArchived,
        ImageUrl = c.ImageUrl,
        Sessions = c.Sessions?.Select(s => new SessionDto
        {
            Id = s.Id,
            Title = s.Title,
            Description = s.Description,
            Track = s.Track,
            StartTime = s.StartTime,
            EndTime = s.EndTime,
            Location = s.Location,
            VirtualLink = s.VirtualLink,
            RecordingUrl = s.RecordingUrl,
            SlideDeckUrl = s.SlideDeckUrl,
            SessionSummary = s.SessionSummary,
            IsPublished = s.IsPublished,
            Speaker = s.Speaker != null ? new SpeakerDto
            {
                Id = s.Speaker.Id,
                Name = s.Speaker.Name,
                Title = s.Speaker.Title,
                Bio = s.Speaker.Bio,
                Organization = s.Speaker.Organization,
                PhotoUrl = s.Speaker.PhotoUrl
            } : null
        }).ToList() ?? [],
        Sponsors = c.Sponsors?.Select(s => new SponsorDto
        {
            Id = s.Id,
            Name = s.Name,
            Tier = s.Tier.ToString(),
            LogoUrl = s.LogoUrl,
            WebsiteUrl = s.WebsiteUrl
        }).ToList() ?? [],
        CreatedAt = c.CreatedAt
    };
}