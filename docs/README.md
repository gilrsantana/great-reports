---
type: architecture_document
title: Great Reports Architecture Overview
description: Ground truth technical overview, architecture design, and diagrams catalog for the Great Reports platform.
timestamp: 2026-06-27T06:46:00-03:00
---

# Great Reports Platform Architecture

Great Reports is a multi-tenant reporting and activity automation platform designed for B2B service providers. It automates daily routine reporting to client stakeholders by leveraging background processors, LLM summarization, and scheduled email campaigns.

---

## System Boundaries and Containers

The system is split into two primary applications: a standalone frontend client built with Angular 22, and a structured backend Web API built with .NET 10 following Clean Architecture guidelines. MySQL stores both application data and background queues.

For a visual breakdown of components and external integrations, refer to the:
👉 **[C4 Container Diagram](./diagrams/c4/system-context.md)**

---

## Role-Based Access & Workflows

System operations are strictly restricted by user roles (Manager, Maintainer, GroupLeader, Partner, and Client Stakeholder). Additionally, a secure verification step requires all registered users and contacts to confirm their email before unlocking full platform features.

For details on role-specific actions and the validation step:
👉 **[Roles Use Case Diagram](./diagrams/use-case/roles-use-case.md)**

---

## Domain Architecture

The core Domain layer is structured as rich entities containing internal invariants. Creation is managed by static factory methods returning `Result` outcomes. Key aggregates include Provider Companies, Client Companies, Projects, and Partner Groups.

For the visual database schema and relationships:
👉 **[Domain Model UML Class Diagram](./diagrams/class/domain-model.md)**

---

## Automated Reporting Pipeline

Every logged activity is aggregated periodically. The system schedules background jobs via Hangfire, queries raw logs, forwards them to the Gemini LLM for summarization, and delivers HTML-formatted emails to configured receivers using Resend.

For the step-by-step background processing logic:
👉 **[Routine Report Workflow](./diagrams/activity/report-workflow.md)**

---

## Core Technical Standards

1. **Clean Architecture**: Outward dependency flow (`Shared` <- `Domain` <- `Application` <- `Infrastructure` <- `Presentation`).
2. **Custom CQRS**: No third-party mediators. Command and Query Handlers are registered and injected manually.
3. **Database Provider**: MySQL configuration using the official `MySql.EntityFrameworkCore` package.
4. **Error Handling**: Standardized RFC 7807 `ProblemDetails` representation mapping from custom `Result` primitives.
5. **Background Engine**: Hangfire dashboard exposed only to authorized `Maintainer` role profiles.
6. **Frontend State**: Signals and computed properties managing component-level state reactively.
7. **Localization boundaries (RULE-014)**: Technical code uses US-English, user-facing UI and API validation errors are written in BR-Portuguese.

