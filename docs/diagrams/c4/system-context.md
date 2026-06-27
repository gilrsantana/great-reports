---
type: architecture_document
title: C4 Container Diagram
description: Level 1 and 2 container diagram representing Great Reports containers, databases, and integrations.
timestamp: 2026-06-27T06:47:00-03:00
---

# C4 Container Diagram

The diagram below details the primary containers of the Great Reports system, their communication boundaries, and interactions with external API services.

```mermaid
graph TB
    subgraph Users ["Users & Clients"]
        Manager["Manager"]
        GroupLeader["Group Leader"]
        Partner["Partner"]
        Maintainer["Maintainer"]
        Client["Client Stakeholder"]
    end

    subgraph SystemBoundary ["Great Reports Platform Boundary"]
        SPA["Frontend SPA (Angular 22)"]
        API["Backend API (ASP.NET Core .NET 10)"]
        Hangfire["Background Worker (Hangfire Engine)"]
        DB[(Database - MySQL)]
    end

    subgraph ExternalServices ["External Integrations"]
        Gemini["Gemini LLM API (Google)"]
        Resend["Resend Email API"]
    end

    %% User Interactions
    Manager -->|Configures clients, projects & users| SPA
    GroupLeader -->|Creates groups & schedules| SPA
    Partner -->|Logs daily activities| SPA
    Maintainer -->|Accesses dashboard & admin tools| SPA
    Client -->|Views read-only reports & dashboards| SPA

    %% Container Communications
    SPA -->|HTTPS REST JSON Calls| API
    API -->|Reads / Writes Application Data| DB
    Hangfire -->|Queries activities & schedules| DB
    Hangfire -->|Triggers summarization| Gemini
    Hangfire -->|Dispatches reports| Resend

    %% Worker Management
    API -->|Enqueues & registers jobs| Hangfire

    %% Styling
    style SystemBoundary fill:none,stroke:#333,stroke-width:2px,stroke-dasharray: 5 5
    style SPA fill:#1168bd,stroke:#0b4c8a,color:#fff
    style API fill:#1168bd,stroke:#0b4c8a,color:#fff
    style Hangfire fill:#1168bd,stroke:#0b4c8a,color:#fff
    style DB fill:#168039,stroke:#115e2a,color:#fff
    style Gemini fill:#d97706,stroke:#b45309,color:#fff
    style Resend fill:#d97706,stroke:#b45309,color:#fff
```

## Container Descriptions

### 1. Frontend SPA (Angular 22)
- **Role**: Client interface providing dashboard visualizations, form registry management, activity logging pages, and read-only reports for client contacts.
- **Technologies**: TypeScript, Angular 22 (Standalone components, reactive signals), Tailwind CSS v4, and chart rendering modules.

### 2. Backend API (.NET 10)
- **Role**: Handles HTTP requests, enforces role-based access control, dispatches custom CQRS commands/queries, manages data transactions, and hosts the Hangfire management dashboard.
- **Technologies**: C#, .NET 10 Web API, ASP.NET Core Identity.

### 3. Background Worker (Hangfire Engine)
- **Role**: Process runner executing recurring and queued jobs without blocking HTTP threads. Triggers daily, weekly, 10 days, 12 days, 15 days, monthly, and specific day activity report compilations.
- **Technologies**: Hangfire Server, C#.

### 4. Database (MySQL)
- **Role**: Relational store housing application tables (Provider, Companies, Clients, Projects, activities, groups, credentials) and Hangfire job schemas.
- **Technologies**: MySQL, Entity Framework Core via `MySql.EntityFrameworkCore`.

### 5. Gemini LLM API
- **Role**: Accepts partners' raw "done" and "doing" activity logs and returns concise, structured professional reports.
- **Technologies**: External REST API, JSON.

### 6. Resend Email API
- **Role**: Sends verification codes and compiles final activity summaries to stakeholders.
- **Technologies**: External REST API.
