# Rule: Localization and Language Boundary Standards

## Metadata
- **ID**: RULE-014-LOCALIZATION
- **Scope**: Entire Solution
- **Target Types**: HTML Templates, TS Component UI, API Error Messages, Email Templates, Code Definitions
- **Status**: Active

## Overview
This rule defines language and localization boundaries across the codebase. Great Reports targets Portuguese-speaking users and clients, but maintains international standards for technical coding. Therefore, all code definitions (classes, variables, databases) must use US-English, while all user-facing systems (frontend UI, email templates, API error descriptions) must use BR-Portuguese.

---

## 1. Code Definitions (US-English Only)
All programming elements must be declared and written in **US-English**. This includes:
- C# Class, Method, and Property names (e.g., `ProviderCompany`, `DailyActivity`, `IsBlocked`).
- TypeScript files, classes, components, and variables (e.g., `group-list.component.ts`, `activeGroups`).
- Database schema names, tables, columns, indexes, and migrations (e.g., `"ProviderCompanies"`, `"IsPublished"`).
- Business Logic Error Codes dot notation keys (e.g., `"Auth.InvalidCredentials"`, `"User.NotFound"`).

---

## 2. User Interfaces and Visual Elements (BR-Portuguese Only)
All user-facing views, dashboards, and templates must be written in **BR-Portuguese**. This includes:
- Angular HTML Templates (labels, titles, buttons, descriptive texts, modals, form placeholders, tooltips).
- Dashboard Charts and Widgets (axis labels, legends, titles, metric descriptions).
- Form validation error notices shown directly in the UI (e.g., "O e-mail informado é inválido").

---

## 3. API Error Messages (BR-Portuguese Only)
When returning operation failures or validation errors through the `Result` pattern:
- The `Message` property of `Error` records and `ErrorMessage` of `ValidationError` records must contain descriptive text in **BR-Portuguese**.
- Example:
  ```csharp
  // Code is in English, but the message description is in Portuguese
  return Result.Failure<User>(new Error("User.NotFound", "Usuário não foi encontrado."));
  ```

---

## 4. Email Templates (BR-Portuguese Only)
All emails dispatched via the Resend API must be formatted in **BR-Portuguese**. This includes:
- Welcome and Email Verification validation code messages.
- Generated LLM Daily/Weekly/Monthly Group Activity reports.
- Gemini API fallback warning notices sent to Group Leaders, Managers, and Maintainers.
