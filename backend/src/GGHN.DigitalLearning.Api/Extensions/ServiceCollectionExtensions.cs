using GGHN.DigitalLearning.Application.Interfaces;
using GGHN.DigitalLearning.Infrastructure.Data;
using GGHN.DigitalLearning.Infrastructure.Services;
using GGHN.DigitalLearning.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;

namespace GGHN.DigitalLearning.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 8;
            options.User.RequireUniqueEmail = true;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        var jwtKey = configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                RequireExpirationTime = true,
                RequireSignedTokens = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = key,
                ClockSkew = TimeSpan.FromMinutes(1)
            };
        });

        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            options.AddPolicy("Admin", policy =>
                policy.RequireRole("Admin"));

            options.AddPolicy("AdminOrEditor", policy =>
                policy.RequireRole("Admin", "Editor"));
        });

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IResourceService, ResourceService>();
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<IPathwayService, PathwayService>();
        services.AddScoped<IConferenceService, ConferenceService>();
        services.AddScoped<ITemplateService, TemplateService>();
        services.AddScoped<IProgressService, ProgressService>();
        services.AddScoped<IPublicationService, PublicationService>();
        services.AddScoped<IDiscussionService, DiscussionService>();
        services.AddScoped<IEditorialService, EditorialService>();
        services.AddScoped<IAnalyticsService, AnalyticsService>();
        services.AddScoped<ICertificateService, CertificateService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IRegistrationService, RegistrationService>();
        services.AddHttpClient();

        return services;
    }
}