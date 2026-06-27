---
type: architecture_document
title: Domain Model UML Class Diagram
description: Domain entity models and their database mapping relationships, showing User, Account, and Identity Roles under B2B service generalized concepts with activity statuses, blockages, and timezone controls.
timestamp: 2026-06-27T07:00:00-03:00
---

# Domain Model Class Diagram

The class diagram below outlines the core domain model representing Great Reports entities, their attributes, and relationships. It highlights the separation of `User` (Domain/Profile) and `Account` (Identity), with roles mapped directly to ASP.NET Core Identity.

```mermaid
classDiagram
    class BaseEntity {
        <<abstract>>
        +Guid Id
        +DateTime CreatedAt
        +DateTime? UpdatedAt
        +bool Active
        +DateTime? UnActivateDate
        +Update()
        +Activate()
        +UnActivate()
    }

    class IdentityUserGuid {
        <<external>>
        +Guid Id
        +string UserName
        +string Email
        +bool EmailConfirmed
        +string PasswordHash
        +string SecurityStamp
        +string ConcurrencyStamp
    }

    class IdentityRoleGuid {
        <<external>>
        +Guid Id
        +string Name
        +string NormalizedName
    }

    class Account {
        +string RefreshToken
        +DateTime RefreshTokenExpiryTime
        +UpdateRefreshToken()
        +UpdateLockoutStatus()
    }

    class Role {
        +string Description
    }

    class ProviderCompany {
        +string Name
        +string TaxId
        +List~ClientCompany~ ClientCompanies
        +List~User~ Users
    }

    class ClientCompany {
        +Guid ProviderCompanyId
        +string Name
        +List~Project~ Projects
        +List~ClientContact~ Contacts
    }

    class ClientContact {
        +Guid ClientCompanyId
        +string Name
        +string Email
        +bool EmailConfirmed
        +string? VerificationToken
        +ContactType Type
    }

    class ContactType {
        <<enumeration>>
        Commercial
        Tech
    }

    class Project {
        +Guid ClientCompanyId
        +string Name
        +string Description
    }

    class User {
        +Guid ProviderCompanyId
        +string DisplayName
        +string Email
        +bool EmailConfirmed
        +string? VerificationToken
    }

    class Group {
        +Guid GroupLeaderId
        +Guid ClientCompanyId
        +Guid ProjectId
        +string Name
        +string Timezone
        +List~User~ AssociatedPartners
        +List~ClientContact~ TargetContacts
        +List~ScheduledEmail~ Schedules
    }

    class ScheduledEmail {
        +Guid GroupId
        +string Name
        +string CronExpression
        +ReportFrequency Frequency
        +int? SpecificDayOfMonth
        +List~ScheduledEmailReceiver~ Receivers
    }

    class ReportFrequency {
        <<enumeration>>
        Daily
        Weekly
        TenDays
        TwelveDays
        FifteenDays
        Monthly
        SpecificDay
    }

    class ScheduledEmailReceiver {
        +Guid ScheduledEmailId
        +Guid UserId
        +Guid? ClientContactId
    }

    class DailyActivity {
        +Guid PartnerId
        +string Title
        +string Theme
        +string Content
        +DateTime ReferenceDate
        +ActivityStatus Status
        +bool IsBlocked
        +bool IsPublished
        +string? SummarizedContent
        +DateTime? ProcessedAt
    }

    class ActivityStatus {
        <<enumeration>>
        Doing
        Done
    }

    class EmailAuditLog {
        +Guid ScheduledEmailId
        +DateTime SentAt
        +string Status
        +string? ErrorMessage
    }

    %% Inheritances
    BaseEntity <|-- ProviderCompany
    BaseEntity <|-- ClientCompany
    BaseEntity <|-- ClientContact
    BaseEntity <|-- Project
    BaseEntity <|-- User
    BaseEntity <|-- Group
    BaseEntity <|-- ScheduledEmail
    BaseEntity <|-- DailyActivity
    BaseEntity <|-- EmailAuditLog
    IdentityUserGuid <|-- Account
    IdentityRoleGuid <|-- Role

    %% Relationships
    ProviderCompany "1" *-- "*" ClientCompany : Manages
    ProviderCompany "1" *-- "*" User : Employs
    ClientCompany "1" *-- "*" Project : Owns
    ClientCompany "1" *-- "*" ClientContact : Employs
    ClientContact --> ContactType : CategorizedBy
    Account "1" ..> "1" User : Links (via Email)
    Account "*" o-- "*" Role : AssignedTo
    Group "1" o-- "*" User : Contains GroupLeader & Partners
    Group "1" o-- "1" ClientCompany : ScopedTo
    Group "1" o-- "1" Project : Tracks
    Group "1" *-- "*" ScheduledEmail : ConfiguredWith
    ScheduledEmail --> ReportFrequency : TriggersAt
    ScheduledEmail "1" *-- "*" ScheduledEmailReceiver : RoutesTo
    DailyActivity "*" --> "1" User : LoggedBy
    DailyActivity --> ActivityStatus : HasStatus
    EmailAuditLog "*" --> "1" ScheduledEmail : Tracks
```

## Entity Descriptions

### 1. Account (Identity)
- Extends the ASP.NET Core Identity class `IdentityUser<Guid>`. Mapped to the `"Accounts"` table. Handles passwords, token creation, lockouts, and authentication claims.

### 2. Role (Identity)
- Extends the ASP.NET Core Identity class `IdentityRole<Guid>`. Mapped to the `"Roles"` table. Handles role authorization groups (e.g. `Manager`, `Maintainer`, `GroupLeader`, `Partner`, `Client`).

### 3. User (Domain / Profile)
- Domain entity inheriting from `BaseEntity` and mapped to `"Users"`. Linked to `Account` via the unique email. Represents personal information. Does not directly manage authorization roles (these are delegated to the Identity `Account`).

### 4. DailyActivity
- Logged daily by partners. Contains individual activity titles, themes, contents, reference dates, status (`Doing`/`Done`), blockage flags (`IsBlocked`), and publishing flags (`IsPublished`). Marks tasks as draft/active during the day and locks them as Published at 11:50 PM in the group's timezone.
