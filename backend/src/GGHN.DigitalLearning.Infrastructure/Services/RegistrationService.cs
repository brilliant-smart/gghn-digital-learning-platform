using GGHN.DigitalLearning.Application.DTOs;
using GGHN.DigitalLearning.Application.Interfaces;
using GGHN.DigitalLearning.Domain.Entities;
using GGHN.DigitalLearning.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GGHN.DigitalLearning.Infrastructure.Services;

public class RegistrationService : IRegistrationService
{
    private readonly AppDbContext _context;

    public RegistrationService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<RegistrationDto> RegisterAsync(CreateRegistrationRequest request, string? userId)
    {
        var registration = new ConferenceRegistration
        {
            ConferenceId = request.ConferenceId,
            UserId = userId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Organization = request.Organization,
            JobTitle = request.JobTitle,
            Country = request.Country,
            PhoneNumber = request.PhoneNumber,
            RegistrationType = request.RegistrationType,
            DietaryRestrictions = request.DietaryRestrictions,
            AccessibilityNeeds = request.AccessibilityNeeds,
            SpecialRequests = request.SpecialRequests,
            Status = "Pending"
        };

        _context.ConferenceRegistrations.Add(registration);
        await _context.SaveChangesAsync();

        return MapToDto(registration);
    }

    public async Task<RegistrationDto?> GetByIdAsync(Guid id)
    {
        var registration = await _context.ConferenceRegistrations
            .Include(r => r.Conference)
            .FirstOrDefaultAsync(r => r.Id == id);
        return registration is null ? null : MapToDto(registration);
    }

    public async Task<IEnumerable<RegistrationDto>> GetByConferenceAsync(Guid conferenceId, string? status = null)
    {
        var query = _context.ConferenceRegistrations
            .Include(r => r.Conference)
            .Where(r => r.ConferenceId == conferenceId);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(r => r.Status == status);

        return await query
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => MapToDto(r))
            .ToListAsync();
    }

    public async Task<IEnumerable<RegistrationDto>> GetMyRegistrationsAsync(string userId)
    {
        return await _context.ConferenceRegistrations
            .Include(r => r.Conference)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => MapToDto(r))
            .ToListAsync();
    }

    public async Task<RegistrationDto?> UpdateStatusAsync(Guid id, UpdateRegistrationStatusRequest request, string reviewerId)
    {
        var registration = await _context.ConferenceRegistrations
            .Include(r => r.Conference)
            .FirstOrDefaultAsync(r => r.Id == id);
        if (registration is null) return null;

        registration.Status = request.Status;
        registration.RejectionReason = request.RejectionReason;
        registration.Notes = request.Notes;
        registration.ReviewedBy = reviewerId;
        registration.ReviewedAt = DateTime.UtcNow;

        if (request.Status == "Approved")
            registration.ConfirmationSentAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return MapToDto(registration);
    }

    public async Task<RegistrationStatsDto> GetStatsAsync(Guid conferenceId)
    {
        var registrations = await _context.ConferenceRegistrations
            .Where(r => r.ConferenceId == conferenceId)
            .Select(r => r.Status)
            .ToListAsync();

        return new RegistrationStatsDto
        {
            TotalRegistrations = registrations.Count,
            Pending = registrations.Count(s => s == "Pending"),
            Approved = registrations.Count(s => s == "Approved"),
            Rejected = registrations.Count(s => s == "Rejected"),
            Waitlisted = registrations.Count(s => s == "Waitlisted")
        };
    }

    private static RegistrationDto MapToDto(ConferenceRegistration r) => new()
    {
        Id = r.Id,
        ConferenceId = r.ConferenceId,
        ConferenceTitle = r.Conference?.Title,
        UserId = r.UserId,
        FirstName = r.FirstName,
        LastName = r.LastName,
        Email = r.Email,
        Organization = r.Organization,
        JobTitle = r.JobTitle,
        Country = r.Country,
        PhoneNumber = r.PhoneNumber,
        RegistrationType = r.RegistrationType,
        Status = r.Status,
        DietaryRestrictions = r.DietaryRestrictions,
        AccessibilityNeeds = r.AccessibilityNeeds,
        SpecialRequests = r.SpecialRequests,
        ReviewedBy = r.ReviewedBy,
        ReviewedAt = r.ReviewedAt,
        RejectionReason = r.RejectionReason,
        Notes = r.Notes,
        CreatedAt = r.CreatedAt
    };
}
