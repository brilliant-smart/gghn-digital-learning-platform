using System.Threading.RateLimiting;
using GGHN.DigitalLearning.Api.Endpoints;
using GGHN.DigitalLearning.Api.Extensions;
using GGHN.DigitalLearning.Api.Middleware;
using GGHN.DigitalLearning.Domain.Entities;
using GGHN.DigitalLearning.Domain.Enums;
using GGHN.DigitalLearning.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Allow environment variables to override appsettings
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddInfrastructureServices(builder.Configuration);

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? ["http://localhost:5173", "http://localhost:8080", "http://localhost:3000", "http://localhost:4173"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();
builder.Services.AddValidation();
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddSlidingWindowLimiter("auth-login", opt =>
    {
        opt.PermitLimit = 5;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.SegmentsPerWindow = 6;
        opt.QueueLimit = 0;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    options.AddSlidingWindowLimiter("auth-register", opt =>
    {
        opt.PermitLimit = 3;
        opt.Window = TimeSpan.FromMinutes(5);
        opt.SegmentsPerWindow = 10;
        opt.QueueLimit = 0;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    options.AddSlidingWindowLimiter("api-general", opt =>
    {
        opt.PermitLimit = 100;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.SegmentsPerWindow = 6;
        opt.QueueLimit = 0;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    options.OnRejected = async (context, ct) =>
    {
        context.HttpContext.Response.ContentType = "application/json";
        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            type = "https://tools.ietf.org/html/rfc6585#section-4",
            title = "Too Many Requests",
            status = 429,
            detail = "Rate limit exceeded. Please try again later."
        }, ct);
    };
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.MapAuthEndpoints();
app.MapResourceEndpoints();
app.MapCourseEndpoints();
app.MapPathwayEndpoints();
app.MapConferenceEndpoints();
app.MapSessionEndpoints();
app.MapTemplateEndpoints();
app.MapProgressEndpoints();
app.MapPublicationEndpoints();
app.MapAdminEndpoints();
app.MapDiscussionEndpoints();
app.MapEditorialEndpoints();
app.MapAnalyticsEndpoints();
app.MapPaymentEndpoints();
app.MapRegistrationEndpoints();

app.MapHealthChecks("/health").AllowAnonymous();

var seedConfig = builder.Configuration.GetSection("SeedData");
if (seedConfig.GetValue<bool>("Enabled", true))
    await SeedAsync(app.Services, seedConfig.GetValue<bool>("ResetOnStartup", false));

app.Run();

async Task SeedAsync(IServiceProvider services, bool resetOnStartup = false)
{
    using var scope = services.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    string[] roles = ["Admin", "Editor", "Member", "Institutional", "FreeUser"];
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    var adminEmail = "admin@gghn.org";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FirstName = "System",
            LastName = "Admin",
            EmailConfirmed = true
        };
        var result = await userManager.CreateAsync(adminUser, "Admin@2026!");
        if (result.Succeeded)
            await userManager.AddToRoleAsync(adminUser, "Admin");
    }

    if (!await dbContext.Resources.AnyAsync())
    {
        var resources = new[]
        {
            new Resource
            {
                Title = "Strengthening HIV Service Delivery in Sub-Saharan Africa",
                Summary = "An evidence-based overview of differentiated service delivery models that improve HIV treatment outcomes while reducing health system burden.",
                PlainLanguageSummary = "This resource explains how different approaches to providing HIV services—such as community-based refills and simplified clinic visits—can help more people stay on treatment while making better use of limited health resources.",
                SourceUrl = "https://example.org/hiv-dsd",
                Topic = "HIV/AIDS Programs",
                Audience = Audience.Clinicians,
                Difficulty = Difficulty.Intermediate,
                Status = ContentStatus.Published,
                Takeaways =
                [
                    new ResourceTakeaway { Content = "Differentiated service delivery improves retention in care", Order = 1 },
                    new ResourceTakeaway { Content = "Community ART refill models reduce facility congestion", Order = 2 },
                    new ResourceTakeaway { Content = "Viral load monitoring remains central to treatment success", Order = 3 }
                ]
            },
            new Resource
            {
                Title = "Disease Surveillance Frameworks for Outbreak Preparedness",
                Summary = "A practical guide to building integrated disease surveillance systems, drawing on lessons from recent epidemics across Africa.",
                PlainLanguageSummary = "This guide shows how countries can build better systems to detect and respond to disease outbreaks early, using real-world examples from recent epidemics in Africa.",
                SourceUrl = "https://example.org/surveillance",
                Topic = "Disease Surveillance",
                Audience = Audience.PolicyMakers,
                Difficulty = Difficulty.Advanced,
                Status = ContentStatus.Published,
                Takeaways =
                [
                    new ResourceTakeaway { Content = "Early warning systems reduce outbreak response times", Order = 1 },
                    new ResourceTakeaway { Content = "Data integration across facilities is essential", Order = 2 },
                    new ResourceTakeaway { Content = "Community-based surveillance strengthens detection", Order = 3 }
                ]
            },
            new Resource
            {
                Title = "Maternal Health Indicators: A Policy Brief",
                Summary = "Plain-language summary of key maternal mortality indicators and their implications for national health policy in Nigeria.",
                PlainLanguageSummary = "This brief summarizes the most important numbers related to maternal health—such as skilled birth attendance rates and antenatal care coverage—and explains what they mean for policy decisions.",
                SourceUrl = "https://example.org/maternal-brief",
                Topic = "Maternal & Child Health",
                Audience = Audience.PolicyMakers,
                Difficulty = Difficulty.Basic,
                Status = ContentStatus.Published,
                Takeaways =
                [
                    new ResourceTakeaway { Content = "Skilled birth attendance correlates strongly with reduced mortality", Order = 1 },
                    new ResourceTakeaway { Content = "Antenatal care coverage gaps remain in rural areas", Order = 2 },
                    new ResourceTakeaway { Content = "Investment in midwifery yields high returns", Order = 3 }
                ]
            },
            new Resource
            {
                Title = "Building Resilient Primary Healthcare Systems",
                Summary = "Frameworks for strengthening primary healthcare delivery as the foundation of universal health coverage.",
                PlainLanguageSummary = "This resource describes how strong primary healthcare systems—community clinics, health posts, and district hospitals—form the backbone of universal health coverage and how countries can make them more resilient.",
                SourceUrl = "https://example.org/phc",
                Topic = "Health Systems Strengthening",
                Audience = Audience.Researchers,
                Difficulty = Difficulty.Intermediate,
                Status = ContentStatus.Published,
                Takeaways =
                [
                    new ResourceTakeaway { Content = "Primary care is the most cost-effective entry point", Order = 1 },
                    new ResourceTakeaway { Content = "Health workforce planning is critical", Order = 2 },
                    new ResourceTakeaway { Content = "Financing reform must accompany service reform", Order = 3 }
                ]
            },
            new Resource
            {
                Title = "Community Health Worker Programs at Scale",
                Summary = "Lessons learned from scaling community health worker programs across multiple African contexts.",
                PlainLanguageSummary = "This resource shares practical lessons from countries that have successfully expanded their community health worker programs, covering training, supervision, and digital tools.",
                SourceUrl = "https://example.org/chw",
                Topic = "Community Health",
                Audience = Audience.CommunityHealthWorkers,
                Difficulty = Difficulty.Basic,
                Status = ContentStatus.Published,
                Takeaways =
                [
                    new ResourceTakeaway { Content = "Standardized training improves quality of care", Order = 1 },
                    new ResourceTakeaway { Content = "Supportive supervision drives retention", Order = 2 },
                    new ResourceTakeaway { Content = "Digital tools enhance CHW productivity", Order = 3 }
                ]
            },
            new Resource
            {
                Title = "Tuberculosis Control: Updated WHO Guidelines",
                Summary = "Synthesis of the latest WHO tuberculosis treatment guidelines with implementation considerations for low-resource settings.",
                PlainLanguageSummary = "This resource summarizes the newest WHO recommendations for TB treatment, including shorter treatment courses and better drug-resistant TB management, and explains how they can be applied in settings with limited resources.",
                SourceUrl = "https://example.org/tb-guidelines",
                Topic = "Infectious Disease Control",
                Audience = Audience.Clinicians,
                Difficulty = Difficulty.Advanced,
                Status = ContentStatus.Published,
                Takeaways =
                [
                    new ResourceTakeaway { Content = "Shorter regimens improve adherence", Order = 1 },
                    new ResourceTakeaway { Content = "Drug-resistant TB requires specialized programs", Order = 2 },
                    new ResourceTakeaway { Content = "Active case finding remains a priority", Order = 3 }
                ]
            }
        };

        dbContext.Resources.AddRange(resources);
    }

    if (!await dbContext.Courses.AnyAsync())
    {
        var courses = new[]
        {
            new Course
            {
                Title = "Foundations of Health Systems Strengthening",
                Description = "An introductory course covering the WHO health system building blocks and their application in African contexts.",
                Topic = "Health Systems Strengthening",
                Difficulty = Difficulty.Intermediate,
                DurationMinutes = 87,
                RequiredTier = MembershipTier.Free,
                Status = ContentStatus.Published,
                Lessons =
                [
                    new Lesson { Title = "Introduction to health systems", DurationMinutes = 12, Order = 1, IsPublished = true },
                    new Lesson { Title = "Service delivery models", DurationMinutes = 18, Order = 2, IsPublished = true },
                    new Lesson { Title = "Health workforce planning", DurationMinutes = 22, Order = 3, IsPublished = true },
                    new Lesson { Title = "Health financing", DurationMinutes = 20, Order = 4, IsPublished = true },
                    new Lesson { Title = "Information systems & governance", DurationMinutes = 15, Order = 5, IsPublished = false }
                ]
            },
            new Course
            {
                Title = "HIV Treatment & Care Essentials",
                Description = "A clinical training course on current HIV treatment protocols, viral load monitoring, and patient-centered care.",
                Topic = "HIV/AIDS Programs",
                Difficulty = Difficulty.Intermediate,
                DurationMinutes = 63,
                RequiredTier = MembershipTier.Free,
                Status = ContentStatus.Published,
                Lessons =
                [
                    new Lesson { Title = "HIV epidemiology overview", DurationMinutes = 10, Order = 1, IsPublished = true },
                    new Lesson { Title = "ART initiation", DurationMinutes = 20, Order = 2, IsPublished = false },
                    new Lesson { Title = "Viral load monitoring", DurationMinutes = 15, Order = 3, IsPublished = false },
                    new Lesson { Title = "Managing co-infections", DurationMinutes = 18, Order = 4, IsPublished = false }
                ]
            },
            new Course
            {
                Title = "Outbreak Investigation Fundamentals",
                Description = "Step-by-step methods for investigating disease outbreaks, with case studies from recent African epidemics.",
                Topic = "Disease Surveillance",
                Difficulty = Difficulty.Advanced,
                DurationMinutes = 75,
                RequiredTier = MembershipTier.Member,
                Status = ContentStatus.Published,
                Lessons =
                [
                    new Lesson { Title = "Defining an outbreak", DurationMinutes = 12, Order = 1, IsPublished = true },
                    new Lesson { Title = "Case definitions & line lists", DurationMinutes = 20, Order = 2, IsPublished = true },
                    new Lesson { Title = "Descriptive epidemiology", DurationMinutes = 25, Order = 3, IsPublished = false },
                    new Lesson { Title = "Control measures", DurationMinutes = 18, Order = 4, IsPublished = false }
                ]
            }
        };

        dbContext.Courses.AddRange(courses);
    }

    if (!await dbContext.Pathways.AnyAsync())
    {
        var pathways = new[]
        {
            new Pathway
            {
                Title = "Health Systems Strengthening",
                Description = "A curated learning journey through the building blocks of resilient health systems, from financing to workforce.",
                Topic = "Health Systems Strengthening",
                LearningObjective = "Understand the six building blocks of health systems and apply frameworks for strengthening health service delivery in low-resource settings.",
                EstimatedDurationMinutes = 240,
                Status = ContentStatus.Published,
                PathwayResources = []
            },
            new Pathway
            {
                Title = "Infectious Disease Control",
                Description = "Build expertise in preventing, detecting, and responding to infectious disease threats in African contexts.",
                Topic = "Infectious Disease Control",
                LearningObjective = "Develop skills in disease surveillance, outbreak investigation, and infection prevention and control strategies.",
                EstimatedDurationMinutes = 180,
                Status = ContentStatus.Published,
                PathwayResources = []
            },
            new Pathway
            {
                Title = "Community Health Programs",
                Description = "Learn to design, implement, and scale community-based health interventions with measurable impact.",
                Topic = "Community Health",
                LearningObjective = "Master the principles of community health program design, CHW supervision, and community-based surveillance.",
                EstimatedDurationMinutes = 160,
                Status = ContentStatus.Published,
                PathwayResources = []
            },
            new Pathway
            {
                Title = "Maternal & Child Health",
                Description = "Evidence-based approaches to reducing maternal and child mortality across the continuum of care.",
                Topic = "Maternal & Child Health",
                LearningObjective = "Understand key maternal and child health indicators, interventions, and policy frameworks for improving outcomes.",
                EstimatedDurationMinutes = 200,
                Status = ContentStatus.Published,
                PathwayResources = []
            }
        };

        dbContext.Pathways.AddRange(pathways);
    }

    if (!await dbContext.Templates.AnyAsync())
    {
        var templates = new[]
        {
            new Template
            {
                Title = "Monitoring & Evaluation Framework Template",
                Description = "A ready-to-adapt M&E framework for public health programs, with indicator examples.",
                Format = "DOCX",
                Tier = TemplateTier.Free,
                Status = ContentStatus.Published
            },
            new Template
            {
                Title = "Training Facilitator Guide",
                Description = "Structured facilitator guide for delivering 1- to 3-day public health training workshops.",
                Format = "PDF",
                Tier = TemplateTier.Free,
                Status = ContentStatus.Published
            },
            new Template
            {
                Title = "Community Health Worker Supervision Checklist",
                Description = "Field-tested supervision checklist for routine CHW support and quality assurance.",
                Format = "PDF",
                Tier = TemplateTier.Free,
                Status = ContentStatus.Published
            },
            new Template
            {
                Title = "Outbreak Investigation Line List",
                Description = "Standard line list template for capturing case data during outbreak investigations.",
                Format = "XLSX",
                Tier = TemplateTier.Premium,
                Price = 29.99m,
                Status = ContentStatus.Published
            }
        };

        dbContext.Templates.AddRange(templates);
    }

    if (!await dbContext.Speakers.AnyAsync())
    {
        var speakers = new[]
        {
            new Speaker { Name = "Dr. Ibrahim Bola Gobir", Title = "Chief Executive Officer", Organization = "GGHN" },
            new Speaker { Name = "Ms. Piring'ar Mercy Niyang", Title = "Chief Technical Officer", Organization = "GGHN" },
            new Speaker { Name = "Emeka Madubuko", Title = "Director, Health Informatics", Organization = "GGHN" },
            new Speaker { Name = "Ms. Ochanya Sonia Ogbeh", Title = "Advisor, Digital Health Innovations & Gender", Organization = "GGHN" },
            new Speaker { Name = "Adebola Akinjeji", Title = "Health Informatics Advisor", Organization = "GGHN" },
            new Speaker { Name = "Dr. Winifred Ukponu", Title = "Associate Director, Global Health Security", Organization = "GGHN" }
        };

        dbContext.Speakers.AddRange(speakers);
    }

    if (!await dbContext.Conferences.AnyAsync())
    {
        var speakersList = await dbContext.Speakers.ToListAsync();
        var ibrahim = speakersList.First(s => s.Name.Contains("Ibrahim"));
        var piringar = speakersList.First(s => s.Name.Contains("Piring'ar"));
        var emeka = speakersList.First(s => s.Name.Contains("Emeka"));
        var sonia = speakersList.First(s => s.Name.Contains("Ochanya"));
        var adebola = speakersList.First(s => s.Name.Contains("Adebola"));
        var winifred = speakersList.First(s => s.Name.Contains("Winifred"));

        var conference = new Conference
        {
            Title = "GGHN Annual Global Health Conference 2026",
            Theme = "Bridging Gaps: From Evidence to Action in Global Health",
            Description = "Join leading global health professionals for three days of keynotes, workshops, and networking. This year's conference focuses on translating research evidence into practical health interventions across Africa.",
            StartDate = new DateTime(2026, 9, 15),
            EndDate = new DateTime(2026, 9, 17),
            Venue = "Transcorp Hilton, Abuja, Nigeria",
            Year = 2026,
            IsArchived = false,
            Sponsors =
            [
                new Sponsor { Name = "WHO Nigeria", Tier = SponsorTier.Gold },
                new Sponsor { Name = "UNICEF", Tier = SponsorTier.Gold },
                new Sponsor { Name = "Gavi", Tier = SponsorTier.Silver },
                new Sponsor { Name = "BMGF", Tier = SponsorTier.Silver },
                new Sponsor { Name = "DoS", Tier = SponsorTier.Bronze }
            ],
            Sessions = new List<Session>
            {
                new Session { Title = "Opening Ceremony & Keynote: The Future of Global Health in Africa", Track = "Plenary", StartTime = new DateTime(2026, 9, 15, 9, 0, 0), EndTime = new DateTime(2026, 9, 15, 10, 30, 0), Location = "Main Hall", SpeakerId = ibrahim.Id, Description = "Welcome address and keynote setting the vision for global health transformation across Africa over the next decade." },
                new Session { Title = "Strengthening Epidemic Preparedness Through Cross-Border Collaboration", Track = "Global Health Security", StartTime = new DateTime(2026, 9, 15, 11, 0, 0), EndTime = new DateTime(2026, 9, 15, 12, 30, 0), Location = "Room A", SpeakerId = winifred.Id, Description = "Examining regional coordination mechanisms for disease surveillance and outbreak response across West African borders." },
                new Session { Title = "Digital Health Tools for Real-Time Disease Surveillance", Track = "Digital Health Innovation", StartTime = new DateTime(2026, 9, 15, 11, 0, 0), EndTime = new DateTime(2026, 9, 15, 12, 30, 0), Location = "Room B", SpeakerId = emeka.Id, Description = "Exploring how digital platforms and mobile tools are transforming disease detection and reporting in resource-limited settings." },
                new Session { Title = "Health Financing Models for Universal Health Coverage in LMICs", Track = "Health Systems Strengthening", StartTime = new DateTime(2026, 9, 15, 11, 0, 0), EndTime = new DateTime(2026, 9, 15, 12, 30, 0), Location = "Room C", SpeakerId = piringar.Id, Description = "Reviewing innovative health financing approaches that expand coverage while ensuring financial sustainability in low-resource contexts." },
                new Session { Title = "Lunch Break", Track = "Break", StartTime = new DateTime(2026, 9, 15, 12, 30, 0), EndTime = new DateTime(2026, 9, 15, 13, 30, 0), Location = "Grand Ballroom Foyer", Description = "Networking lunch with poster presentations in the foyer." },
                new Session { Title = "Lassa Fever Prevention: From Research to Community Action", Track = "Global Health Security", StartTime = new DateTime(2026, 9, 15, 13, 30, 0), EndTime = new DateTime(2026, 9, 15, 15, 0, 0), Location = "Room A", SpeakerId = ibrahim.Id, Description = "Translating research findings into community-level Lassa fever prevention programs, with case studies from Ondo State." },
                new Session { Title = "AI and Machine Learning for Disease Prediction in Resource-Limited Settings", Track = "Digital Health Innovation", StartTime = new DateTime(2026, 9, 15, 13, 30, 0), EndTime = new DateTime(2026, 9, 15, 15, 0, 0), Location = "Room B", SpeakerId = sonia.Id, Description = "Practical applications of AI/ML for predicting disease outbreaks and optimizing health resource allocation." },
                new Session { Title = "Building Resilient Primary Health Care Systems in Fragile States", Track = "Health Systems Strengthening", StartTime = new DateTime(2026, 9, 15, 13, 30, 0), EndTime = new DateTime(2026, 9, 15, 15, 0, 0), Location = "Room C", SpeakerId = adebola.Id, Description = "Strategies for strengthening primary healthcare delivery in conflict-affected and fragile settings across Africa." },
                new Session { Title = "Networking Reception", Track = "Special", StartTime = new DateTime(2026, 9, 15, 17, 0, 0), EndTime = new DateTime(2026, 9, 15, 19, 0, 0), Location = "Rooftop Terrace", Description = "Welcome reception with cultural performances and informal networking opportunities." },

                new Session { Title = "Keynote Panel: One Health Approaches to Pandemic Prevention", Track = "Plenary", StartTime = new DateTime(2026, 9, 16, 9, 0, 0), EndTime = new DateTime(2026, 9, 16, 10, 30, 0), Location = "Main Hall", SpeakerId = winifred.Id, Description = "Distinguished panel exploring the intersection of human, animal, and environmental health in preventing future pandemics." },
                new Session { Title = "Antimicrobial Resistance Surveillance in West Africa", Track = "Global Health Security", StartTime = new DateTime(2026, 9, 16, 11, 0, 0), EndTime = new DateTime(2026, 9, 16, 12, 30, 0), Location = "Room A", SpeakerId = ibrahim.Id, Description = "Current state of AMR surveillance systems in West Africa and strategies for strengthening laboratory capacity and data sharing." },
                new Session { Title = "Mobile Health (mHealth) Interventions for Medication Adherence", Track = "Digital Health Innovation", StartTime = new DateTime(2026, 9, 16, 11, 0, 0), EndTime = new DateTime(2026, 9, 16, 12, 30, 0), Location = "Room B", SpeakerId = piringar.Id, Description = "Evidence review of mHealth tools that improve medication adherence among chronic disease patients in African settings." },
                new Session { Title = "Task Shifting and Community Health Worker Integration", Track = "Health Systems Strengthening", StartTime = new DateTime(2026, 9, 16, 11, 0, 0), EndTime = new DateTime(2026, 9, 16, 12, 30, 0), Location = "Room C", SpeakerId = adebola.Id, Description = "Evaluating the impact of task shifting policies and community health worker programs on health system performance." },
                new Session { Title = "Lunch Break", Track = "Break", StartTime = new DateTime(2026, 9, 16, 12, 30, 0), EndTime = new DateTime(2026, 9, 16, 13, 30, 0), Location = "Grand Ballroom Foyer", Description = "Networking lunch with exhibitor booths and technology demonstrations." },
                new Session { Title = "Climate Change and Infectious Disease Emergence", Track = "Global Health Security", StartTime = new DateTime(2026, 9, 16, 13, 30, 0), EndTime = new DateTime(2026, 9, 16, 15, 0, 0), Location = "Room A", SpeakerId = sonia.Id, Description = "Understanding the links between climate variability, environmental change, and the emergence of infectious diseases in Africa." },
                new Session { Title = "Blockchain for Health Supply Chain Integrity", Track = "Digital Health Innovation", StartTime = new DateTime(2026, 9, 16, 13, 30, 0), EndTime = new DateTime(2026, 9, 16, 15, 0, 0), Location = "Room B", SpeakerId = emeka.Id, Description = "Exploring blockchain applications for ensuring pharmaceutical supply chain transparency and reducing counterfeit medicines." },
                new Session { Title = "Public-Private Partnerships in Health Infrastructure Development", Track = "Health Systems Strengthening", StartTime = new DateTime(2026, 9, 16, 13, 30, 0), EndTime = new DateTime(2026, 9, 16, 15, 0, 0), Location = "Room C", SpeakerId = piringar.Id, Description = "Case studies of successful PPP models for building and maintaining health infrastructure in Nigeria and across Africa." },
                new Session { Title = "Evening Gala Dinner & Awards", Track = "Special", StartTime = new DateTime(2026, 9, 16, 19, 0, 0), EndTime = new DateTime(2026, 9, 16, 21, 0, 0), Location = "Main Hall", Description = "Recognition of outstanding contributions to global health with keynote address and three-course dinner." },

                new Session { Title = "Plenary: Policy Recommendations for the Next Decade", Track = "Plenary", StartTime = new DateTime(2026, 9, 17, 9, 0, 0), EndTime = new DateTime(2026, 9, 17, 10, 30, 0), Location = "Main Hall", SpeakerId = ibrahim.Id, Description = "Synthesis of conference insights into actionable policy recommendations for governments, donors, and implementing partners." },
                new Session { Title = "Vaccine Equity and Last-Mile Delivery Challenges", Track = "Global Health Security", StartTime = new DateTime(2026, 9, 17, 11, 0, 0), EndTime = new DateTime(2026, 9, 17, 12, 30, 0), Location = "Room A", SpeakerId = winifred.Id, Description = "Addressing the persistent challenges of equitable vaccine distribution and innovative last-mile delivery solutions." },
                new Session { Title = "Telemedicine in Conflict Zones: Lessons from Northern Nigeria", Track = "Digital Health Innovation", StartTime = new DateTime(2026, 9, 17, 11, 0, 0), EndTime = new DateTime(2026, 9, 17, 12, 30, 0), Location = "Room B", SpeakerId = sonia.Id, Description = "Operational lessons from deploying telemedicine services in insecure and hard-to-reach areas of northern Nigeria." },
                new Session { Title = "Maternal and Child Health Service Integration", Track = "Health Systems Strengthening", StartTime = new DateTime(2026, 9, 17, 11, 0, 0), EndTime = new DateTime(2026, 9, 17, 12, 30, 0), Location = "Room C", SpeakerId = adebola.Id, Description = "Models for integrating maternal, newborn, and child health services to improve continuity of care and outcomes." },
                new Session { Title = "Lunch Break", Track = "Break", StartTime = new DateTime(2026, 9, 17, 12, 30, 0), EndTime = new DateTime(2026, 9, 17, 13, 30, 0), Location = "Grand Ballroom Foyer", Description = "Final networking lunch with closing poster session." },
                new Session { Title = "Interactive Workshop: Designing Community-Led Health Interventions", Track = "Plenary", StartTime = new DateTime(2026, 9, 17, 13, 30, 0), EndTime = new DateTime(2026, 9, 17, 15, 30, 0), Location = "Main Hall", SpeakerId = emeka.Id, Description = "Hands-on workshop where participants co-design community engagement strategies using human-centered design principles." },
                new Session { Title = "Closing Ceremony: Commitments & Way Forward", Track = "Plenary", StartTime = new DateTime(2026, 9, 17, 15, 30, 0), EndTime = new DateTime(2026, 9, 17, 16, 30, 0), Location = "Main Hall", SpeakerId = ibrahim.Id, Description = "Closing remarks, conference declaration, and announcement of GGHN Conference 2027." }
            }
        };

        dbContext.Conferences.Add(conference);
    }

    if (!await dbContext.Publications.AnyAsync())
    {
        var publications = new[]
        {
            new Publication
            {
                Title = "Patients and Healthcare Workers' Preferences for Smart Locker-Based Medication Access in Nigeria",
                Summary = "Explores the acceptability and usability of smart locker systems for dispensing chronic disease medication in Nigeria, highlighting patient convenience and system efficiency.",
                Content = "This study investigates patient and healthcare worker preferences for smart locker-based medication access systems in Nigerian healthcare facilities. The research found that smart lockers significantly reduced wait times and improved medication adherence among chronic disease patients, while healthcare workers reported reduced facility congestion and improved workflow efficiency.",
                Author = "Ibrahim Bola Gobir, Piring'ar Mercy Niyang, Samson Agboola",
                Status = ContentStatus.UnderReview,
                PublishedAt = null,
                PublicationType = "Journal Article",
                Tags = "[\"Digital Health\",\"Health Systems\"]",
                KeyFindings = "[\"Patients reported high satisfaction with 24/7 medication pickup convenience\",\"Healthcare workers noted reduced facility congestion and shorter queues\",\"Smart lockers improved adherence among patients with chronic conditions\"]",
                ExternalUrl = "https://example.org/publications/smart-locker-preferences",
                Year = 2024
            },
            new Publication
            {
                Title = "Acceptability of Smart Locker Technology for Chronic Disease Medication Delivery",
                Summary = "Evaluates patient and provider perspectives on technology-enabled medication access solutions in resource-constrained settings.",
                Content = "This research evaluates the acceptability of smart locker technology among patients with chronic diseases and their healthcare providers in Nigeria. The study examines user experience, perceived benefits, barriers to adoption, and implementation considerations for scaling technology-enabled medication delivery solutions in resource-limited settings.",
                Author = "Ibrahim Bola Gobir, Mercy Niyang, Havilah Nnadozie",
                Status = ContentStatus.Published,
                PublishedAt = new DateTime(2024, 9, 22),
                PublicationType = "Journal Article",
                Tags = "[\"Digital Health\",\"Service Delivery\"]",
                KeyFindings = "[\"Over 80% of surveyed patients expressed willingness to use the technology\",\"Providers cited workflow integration as a key implementation factor\",\"Cost and infrastructure remain primary scale-up considerations\"]",
                ExternalUrl = "https://example.org/publications/smart-locker-acceptability",
                Year = 2024
            },
            new Publication
            {
                Title = "Strengthening Lassa Fever Prevention Through Community Engagement in Ondo State",
                Summary = "Demonstrates how community-led interventions improved awareness and behavioral practices in Lassa fever prevention.",
                Content = "This study documents the implementation and outcomes of a community engagement strategy for Lassa fever prevention in Ondo State, Nigeria. The intervention trained community health volunteers to deliver prevention messaging, distribute supplies, and support household sanitation improvements, reaching over 50,000 residents across targeted communities.",
                Author = "Ibrahim Bola Gobir, Mercy Niyang",
                Status = ContentStatus.Published,
                PublishedAt = new DateTime(2025, 2, 10),
                PublicationType = "Conference Abstract",
                Tags = "[\"Infectious Disease\",\"Community Health\"]",
                KeyFindings = "[\"Community health volunteers reached over 50,000 residents with prevention messaging\",\"Household sanitation practices improved measurably post-intervention\",\"Local leadership engagement was central to behavior change\"]",
                ExternalUrl = "https://example.org/publications/lassa-ondo",
                Year = 2025
            },
            new Publication
            {
                Title = "Bridging the Gap Between Awareness and Behavior in Lassa Fever Prevention",
                Summary = "Examines behavioral gaps in disease prevention despite high awareness levels in endemic communities.",
                Content = "This research examines the persistent gap between Lassa fever awareness and protective behavioral practices in endemic communities in Nigeria. Despite awareness levels exceeding 85%, the study found that structural barriers, resource constraints, and cultural factors significantly limited adoption of recommended prevention practices, highlighting the need for tailored behavior-change communication strategies.",
                Author = "GGHN Research Team",
                Status = ContentStatus.Published,
                PublishedAt = new DateTime(2025, 3, 5),
                PublicationType = "Conference Abstract",
                Tags = "[\"Infectious Disease\"]",
                KeyFindings = "[\"Awareness of Lassa fever exceeded 85% but protective behaviors lagged\",\"Structural barriers limited adoption of recommended practices\",\"Tailored behavior-change communication is needed alongside awareness campaigns\"]",
                ExternalUrl = "https://example.org/publications/lassa-awareness-behavior",
                Year = 2025
            },
            new Publication
            {
                Title = "Improving Retention in HIV Care Through Ancillary Support Services",
                Summary = "Analyzes the role of support services in improving retention and outcomes for HIV patients in Nigeria.",
                Content = "This study analyzes the impact of ancillary support services—including transport assistance, nutrition support, and peer counseling—on retention in HIV care programs across multiple sites in Nigeria. The findings demonstrate that integrated support models significantly improve 12-month retention rates and viral suppression outcomes.",
                Author = "Mercy Niyang, Ibrahim Bola Gobir",
                Status = ContentStatus.Published,
                PublishedAt = new DateTime(2024, 11, 8),
                PublicationType = "Journal Article",
                Tags = "[\"HIV/AIDS\",\"Health Systems\"]",
                KeyFindings = "[\"Patients receiving ancillary support showed higher 12-month retention\",\"Transport and nutrition support were the most-cited enablers\",\"Integrated service models outperformed siloed delivery\"]",
                ExternalUrl = "https://example.org/publications/hiv-retention",
                Year = 2024
            },
            new Publication
            {
                Title = "Community-Based PMTCT Models in Northern Nigeria: Operational Challenges and Lessons",
                Summary = "Compares different approaches to prevention of mother-to-child transmission of HIV in underserved communities.",
                Content = "This comparative study evaluates community-based prevention of mother-to-child transmission (PMTCT) models across northern Nigeria. The research identifies key operational challenges including workforce training gaps, supply chain interruptions, and cultural barriers, while documenting effective strategies such as mother-mentor approaches and community health worker integration.",
                Author = "GGHN Research Team",
                Status = ContentStatus.Published,
                PublishedAt = new DateTime(2024, 8, 20),
                PublicationType = "Journal Article",
                Tags = "[\"Maternal Health\",\"HIV/AIDS\"]",
                KeyFindings = "[\"Community-based models reached women who did not access facility-based care\",\"Workforce training and supervision were the largest operational gaps\",\"Mother-mentor approaches strengthened retention through pregnancy and postpartum\"]",
                ExternalUrl = "https://example.org/publications/pmtct-northern-nigeria",
                Year = 2024
            },
            new Publication
            {
                Title = "Digital Health Innovations for Medication Access in Low-Resource Settings",
                Summary = "Highlights innovative approaches to improving medication access through digital health interventions.",
                Content = "This report highlights innovative digital health approaches to improving medication access in low-resource settings, including smart locker systems, mobile prescription platforms, and supply chain digitization tools. The report evaluates implementation outcomes, cost-effectiveness, and scalability across multiple African contexts.",
                Author = "Sonia Ogbeh, GGHN Digital Health Team",
                Status = ContentStatus.Draft,
                PublishedAt = null,
                PublicationType = "Report",
                Tags = "[\"Digital Health\"]",
                KeyFindings = "[\"Digital tools can reduce stock-out durations when integrated with supply chain data\",\"User-centered design is critical to adoption in low-resource settings\",\"Sustainable financing models remain the key barrier to scale\"]",
                ExternalUrl = "https://example.org/publications/digital-health-access",
                Year = 2023
            },
            new Publication
            {
                Title = "Annual Report: Advancing Health Systems Strengthening in Nigeria",
                Summary = "Overview of programs, research impact, and health system interventions implemented across Nigeria.",
                Content = "GGHN's annual report provides a comprehensive overview of health systems strengthening programs, research outputs, and community health interventions implemented across Nigeria. The report documents progress in HIV service delivery, disease surveillance, maternal and child health, and digital health innovation, while outlining strategic priorities for the coming year.",
                Author = "Georgetown Global Health Nigeria",
                Status = ContentStatus.Published,
                PublishedAt = new DateTime(2023, 6, 30),
                PublicationType = "Report",
                Tags = "[\"Health Systems\"]",
                KeyFindings = "[\"Programs reached over 1 million beneficiaries across multiple states\",\"Research outputs informed national HIV and disease surveillance policy\",\"Partnerships with government and academia expanded implementation reach\"]",
                ExternalUrl = "https://example.org/publications/annual-report-2023",
                Year = 2023
            }
        };

        dbContext.Publications.AddRange(publications);
    }

    await dbContext.SaveChangesAsync();
}