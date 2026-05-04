# GGHN Digital Learning Platform

A full-stack digital learning and content management platform for the Global Health Hub Network (GGHN). Built with .NET 10 Clean Architecture and React, it delivers online courses, a digital resource library, research publications, conference management, editorial workflows, and analytics for global health organizations.

---

## Features

- **Course & Pathway Management** — Structured learning with lessons, progress tracking, and certificate generation
- **Digital Library** — Searchable resource catalog with topic, audience, and difficulty filters
- **Publications** — Research articles, reports, and conference abstracts with editorial review workflow
- **Conference Management** — Session scheduling, speaker profiles, partner carousel, and registration
- **Templates & Tools** — Downloadable M&E frameworks and field guides with premium content gating
- **Discussion Forums** — Threaded discussions on resources with reply support
- **User Roles & Access Tiers** — Admin, Editor, Member, Institutional, FreeUser roles with membership-tier content gating
- **Analytics Dashboard** — Usage statistics, top content, geography, and audience breakdowns
- **Payment Integration** — Paystack-powered template purchases

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Backend | ASP.NET Core 10, EF Core, SQL Server, ASP.NET Identity + JWT |
| Frontend | React 19, TanStack Router & Start, Vite, Tailwind CSS 4, shadcn/ui |
| Architecture | .NET Clean Architecture (Domain → Application → Infrastructure → API) |

## Getting Started

### Prerequisites

- .NET 10 SDK
- Node.js 20+
- SQL Server 2022+ (Express or full)

### Backend

```bash
cd backend/src/GGHN.DigitalLearning.Api
dotnet ef database update --project ../GGHN.DigitalLearning.Infrastructure
dotnet run
```

The API starts at `http://localhost:5289` with Swagger UI at `/swagger`.

### Frontend

```bash
cd frontend
npm install
npm run dev
```

The app starts at `http://localhost:8080`.

### Default Admin

| Field | Value |
|-------|-------|
| Email | `admin@gghn.org` |
| Password | `Admin@2026!` |

Seed data (resources, courses, pathways, speakers, conference, publications) is created automatically on first run.

## Project Structure

```
gghn-digital-learning-platform/
├── backend/
│   └── src/
│       ├── GGHN.DigitalLearning.Domain/        # Entities, enums, common types
│       ├── GGHN.DigitalLearning.Application/    # DTOs, service interfaces
│       ├── GGHN.DigitalLearning.Infrastructure/ # EF Core DbContext, services, migrations
│       └── GGHN.DigitalLearning.Api/            # Minimal API endpoints, middleware, DI
└── frontend/
    └── src/
        ├── api/          # API client modules
        ├── components/  # Reusable UI components
        ├── data/         # Static data configs
        ├── lib/          # Auth utilities
        └── routes/       # TanStack Router page components
```

## API Endpoints

| Group | Endpoints | Auth |
|-------|-----------|------|
| Auth | Register, Login, Refresh, Me, Profile | Mixed |
| Admin | User management, role/tier updates | Admin |
| Resources | CRUD + filtering | Mixed |
| Courses | CRUD + tier-gated access | Mixed |
| Pathways | CRUD | Mixed |
| Publications | CRUD + type/tag/year filtering | Mixed |
| Conferences | CRUD + sessions | Mixed |
| Speakers | CRUD | Mixed |
| Templates | CRUD + premium gating | Mixed |
| Progress | Tracking + certificate | Authenticated |
| Discussions | Threaded CRUD + replies | Mixed |
| Editorial | Review queue, approve/reject | AdminOrEditor |
| Analytics | Dashboard stats, top content, geography | AdminOrEditor |
| Payments | Initialize + verify + webhook | Mixed |
| Registrations | Submit + manage + stats | Mixed |
| Health | Health check | Anonymous |

## License

Proprietary — All rights reserved.