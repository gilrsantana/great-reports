# Product Definition

## Project Name

Great Reports

## Overview

Great Reports is a reports orchestration platform designed for B2B service providers. It keeps group leaders, managers, and client contacts continually updated on project routines.

Partners log their daily activities (what is "doing" and what is "done"). The system automatically compiles these entries on a group level, calls an external Gemini LLM to summarize the entire group's activities, and sends scheduled email reports (daily, weekly, 10 days, 12 days, 15 days, monthly, or specific days of the month) to designated stakeholders using the Resend email provider.

## Problem Solved

Traditional client reporting is manual, delayed, and prone to gaps, resulting in:
- Lack of immediate visibility into partner roadblocks and project health.
- Gaps in alignment between providers, group leaders, and client contacts.
- Reactive management where issues are addressed only after causing delays.
- Heavy manual effort compiling daily work logs for clients.

By automating routine analysis and reporting, Great Reports enables leaders and client contacts to identify and resolve blockers early, ensuring proactive project management.

## Target Audience

- **Provider Companies**: Multi-tenant organizations managing various clients.
- **Managers**: Stakeholders orchestrating resources, projects, and clients.
- **Group Leaders**: Supervisors coordinating groups of partners and configuring client reporting frequencies.
- **Partners**: Professionals logging daily activities.
- **Clients**: Client-side stakeholders receiving summaries and monitoring progress.

---

## Role & Access Control Model

Great Reports implements a strict role-based control model:

1. **Manager**:
   - Register Provider Companies, Client Companies, Client Contacts, Projects, Partners, and Group Leaders.
   - View all reports from all partners and all groups across all Group Leaders.
2. **Maintainer**:
   - Inherits all Manager permissions.
   - Accesses the Hangfire Background Dashboard.
3. **GroupLeader**:
   - Build **Groups**: A group aggregates one or more partners, one client company, one project, client contacts, and scheduled email rules. Registers the Group Leader as the group's owner.
   - View all reports from partners belonging to groups that they created. Cannot access reports from other Group Leaders' groups.
   - Define scheduled email profiles and associate receivers.
4. **Partner**:
   - Write daily activities. Can belong to multiple groups.
   - Add/update daily activities during the day until 11:50 PM.
   - View and search/filter their own published activities/reports by Title, Theme, Content, and Date.
5. **Client**:
   - View read-only dashboards specifically filtered for their company.
   - Receive scheduled email reports according to the group's profile.

---

## Core Capabilities

- **Separated User & Account Management**:
  - Accounts handle secure login credentials and authentication roles.
  - Users handle profile/personal details inside the domain.
  - Fail-safe registration guarantees that if the account authentication configuration fails, the user registry is reverted automatically.
- **Multi-Tenant Administration**: The highest organizational layer is the Provider Company. Each Provider Company is isolated and managed by its own Managers.
- **Client Company & Contact Hierarchy**: A Provider Company works with one or more Client Companies. A Client Company has one or more Client Contacts (of type `Commercial` or `Tech`).
- **Group & Schedule Configuration**: Group Leaders configure groups. For each group, they define:
   - Associated Partners, Projects, Client Contacts, and **Timezone** context.
   - **Scheduled Email Profiles**: All background events (lockout at 11:50 PM and email delivery at 8:00 AM) are calculated relative to the group's local timezone.
     - **Daily**: Triggered every day at 8:00 AM, containing activities logged during the previous calendar day.
     - **Weekly**: Triggered every Monday at 8:00 AM, containing activities logged during the previous calendar week.
     - **TenDays (10)**: Triggered on the 11th day of each month at 8:00 AM, containing activities logged from the 1st to the 10th.
     - **TwelveDays (12)**: Triggered on the 13th day of each month at 8:00 AM, containing activities logged from the 1st to the 12th.
     - **FifteenDays (15)**: Triggered on the 16th day of each month at 8:00 AM, containing activities logged from the 1st to the 15th.
     - **Monthly**: Triggered on the 1st day of each month at 8:00 AM, containing activities logged during the previous calendar month.
     - **Specific Day of Month (Day X)**: Triggered on Day X of the month at 8:00 AM. Gathers activities logged from Day X of the previous month to Day X of the current month. Multiple specific days can be configured in addition to the standard presets.
   - Receivers: Define which group members (Manager, Group Leader, Partners, Client Contacts) receive which type of scheduled email. For example, a Manager and Client Commercial Contact might receive only monthly summary reports, while the Group Leader receives all daily and weekly reports.
- **Activity Logging & Lockouts**: 
  - Partners can log multiple task/activity entries during the day, specifying properties: `Title`, `Theme` (category), `Content` (details), `Status` (enum: `Doing`, `Done`), and `IsBlocked` (boolean).
  - At 11:50 PM in the group's local timezone, a recurring Hangfire job locks all logged activities for the day by setting them to **Published** (immutable).
  - After 11:50 PM, partners cannot add new activities or edit existing activities for that day.
  - Partners can view and filter all their published activities by Title, Theme, Content, and Date.
- **Email Validation**: Every registered person (Manager, Group Leader, Partner, Client Contact) must confirm their email via a secure email verification workflow before logging into the dashboard or receiving regular summaries.
- **Automated Group Summary Delivery**: A background Hangfire worker collects daily activity logs from all partners in the group, compiles them, and requests a unified group-level summary report from the Gemini LLM API, which is sent via the Resend API to the configured receivers.
- **Gemini Fallback Notification Workflow**:
  - If the Gemini API fails, the system retries 3 times.
  - Upon persistent failure, the delivery is suspended, and the system immediately dispatches an alert email/notification containing the raw logs and diagnostic error details to the Group Leader, Managers, and Maintainers to coordinate resolution.
- **Dashboard & Charts**: Visual data representations showing B2B partner metrics:
  - **Activity Distribution by Theme**: Categorizes logged work (e.g. Support vs. Feature Development) to track time utilization.
  - **Group Progress Trend**: Visualizes the completed (`Done`) vs. active (`Doing`) task ratios over time.
  - **Blockers & Roadblocks Indicator**: Summarizes activities flagged with `IsBlocked = true` to isolate project issues.
  - **Activity Volume per Partner**: Monitors partner logging frequency to detect workload overloading or participation issues.

- **Localization & Languages (RULE-014)**:
  - Frontend UI, buttons, validation states, and charts are rendered in **BR-Portuguese**.
  - All email notifications, reports, and error alerts are compiled in **BR-Portuguese**.
  - API business validation messages returned to clients are in **BR-Portuguese**.
  - Technical code declarations, namespaces, parameters, and tables are written in **US-English**.

---

## Ubiquitous Language / Glossary

- **Provider Company (Provedor)**: The root organization using the platform (the service provider).
- **Client Company (Empresa Cliente)**: A client organization serviced by the Provider Company.
- **Client Contact (Contato Cliente)**: A point of contact inside the Client Company (Commercial or Tech contact type).
- **Group (Grupo)**: A project boundary created by a Group Leader linking partners, a project, client contacts, group timezone, and scheduled reports.
- **Daily Activity (Atividade Diária)**: The partner's logged activity item containing title, theme, content, status (`Doing`/`Done`), and blockage status (`IsBlocked`).
- **Scheduled Email (E-mail Agendado)**: Background job profile triggering periodic reports (Daily, Weekly, TenDays, TwelveDays, FifteenDays, Monthly, or Specific Days of the Month) to target recipients.
- **Email Validation (Confirmação de E-mail)**: Security workflow requiring email verification before system activation.
- **Account (Conta)**: Credentials and security identifiers managed by `Microsoft.AspNetCore.Identity`.
- **User (Usuário)**: Domain entity containing personal and professional registration data.
