---
type: architecture_document
title: Routine Report Workflow & Lockout Jobs
description: Activity diagrams showing background activity summarization, email dispatch, and the 11:50 PM daily activity lockout workflow.
timestamp: 2026-06-27T07:01:00-03:00
---

# Background Processing Workflows

This document outlines the two core background processing jobs executed by the Hangfire server:
1. **Activity Report Compilation and Email Dispatch Workflow (with Gemini Fallback Alert)**
2. **End-of-Day Daily Activity Lockout (Publishing) Workflow**

---

## 1. Activity Report Workflow

The state diagram below documents the logical flow executed by the Hangfire background worker when compiling, summarizing, and delivering periodic activities (Daily, Weekly, 10 Days, 12 Days, 15 Days, Monthly, or Specific Days of Month).

```mermaid
stateDiagram-v2
    [*] --> HangfireTrigger : Cron Schedule Fired (8:00 AM in Group Timezone)
    
    HangfireTrigger --> QueryGroupActivities : Fetch Group Config & Partners
    QueryGroupActivities --> CheckLoggedTasks : Load Published Activities within interval
    
    state CheckLoggedTasks {
        [*] --> EvaluateCount
        EvaluateCount --> NoTasksLogged : Count == 0
        EvaluateCount --> TasksLogged : Count > 0
    }

    NoTasksLogged --> LogSkippedJob : Skip Summarization
    TasksLogged --> SendToGeminiAPI : Post aggregated raw logs of all partners to Gemini API
    
    SendToGeminiAPI --> ValidateLLMResponse
    
    state ValidateLLMResponse {
        [*] --> EvaluateStatus
        EvaluateStatus --> APIError : Exception / Timeout
        EvaluateStatus --> APISuccess : Valid JSON Summary
    }
    
    APIError --> EvaluateRetries
    
    state EvaluateRetries {
        [*] --> CheckAttemptCount
        CheckAttemptCount --> RetriesRemaining : Attempt < 3
        CheckAttemptCount --> RetriesExhausted : Attempt >= 3
    }

    RetriesRemaining --> SendToGeminiAPI : Retry with Backoff
    RetriesExhausted --> DispatchFailureAlert : Call Resend to email GL, Managers & Maintainers
    
    APISuccess --> BuildEmailTemplate : Format HTML Body with Group activity summary
    
    BuildEmailTemplate --> DispatchViaResend : Call Resend API
    
    state DispatchViaResend {
        [*] --> EvaluateDelivery
        EvaluateDelivery --> SendSuccess : Email Delivered
        EvaluateDelivery --> SendFailed : API Error
    }
    
    SendFailed --> LogDeliveryFailure : Record error in audit logs
    SendSuccess --> LogDeliverySuccess : Record success in audit logs
    
    LogSkippedJob --> [*]
    LogDeliveryFailure --> [*]
    LogDeliverySuccess --> [*]
    DispatchFailureAlert --> [*]
```

### Execution Steps

#### 1.1 Job Trigger
- Hangfire's recurring engine triggers the job (`CompileDailyActivityJob`, `CompileWeeklyActivityJob`, etc.) based on the cron schedule configured by the Group Leader, evaluated in the **Group's Timezone**.

#### 1.2 Routine Loading
- Gathers tasks logged during the specific timeframe:
  - **Daily**: Activities logged on the previous calendar day.
  - **Weekly**: Activities logged during the previous calendar week.
  - **TenDays (10)**: Activities logged from the 1st to the 10th of the current month.
  - **TwelveDays (12)**: Activities logged from the 1st to the 12th of the current month.
  - **FifteenDays (15)**: Activities logged from the 1st to the 15th of the current month.
  - **Monthly**: Activities logged during the previous calendar month.
  - **Specific Day X**: Activities logged from Day X of the previous month to Day X of the current month.
- Aggregates tasks from *all partners* in the group.

#### 1.3 Gemini LLM Summarization
- Sends the consolidated raw logs of all partners to the Gemini API. If the API fails, the worker retries up to 3 times.

#### 1.4 Fallback Alert
- If all 3 attempts fail, the worker immediately suspends delivery and calls the Resend API to send a diagnostic alert to the Group Leader, Managers, and Maintainers.

#### 1.5 Email Delivery
- Upon API success, builds the compiled HTML report and routes it via Resend to the group's configured receivers.

---

## 2. End-of-Day Daily Activity Lockout Workflow

This job triggers automatically every day at 11:50 PM in the **Group's Timezone** to mark all active daily activity tasks as **Published**, locking them against further additions or edits.

```mermaid
stateDiagram-v2
    [*] --> LockoutTrigger : 11:50 PM local Timezone Cron Fired
    LockoutTrigger --> QueryActiveRoutines : Find all DailyActivities for Today where IsPublished == false
    
    state EvaluateRoutines {
        [*] --> CheckLength
        CheckLength --> RoutinesFound : Count > 0
        CheckLength --> NoRoutinesActive : Count == 0
    }
    
    QueryActiveRoutines --> EvaluateRoutines
    
    NoRoutinesActive --> LogNoAction : Log "No active activities to publish today"
    RoutinesFound --> UpdatePublishFlag : Set IsPublished = true & Update Timestamp
    
    UpdatePublishFlag --> SaveToDatabase : Commit Transaction via Unit of Work
    
    state SaveToDatabase {
        [*] --> DbTransaction
        DbTransaction --> DbSuccess : Success
        DbTransaction --> DbError : Database Exception
    }
    
    DbError --> LogLockoutError : Log error and notify system admin
    DbSuccess --> LogLockoutSuccess : Log "Successfully published N partner activities"
    
    LogNoAction --> [*]
    LogLockoutError --> [*]
    LogLockoutSuccess --> [*]
```

### Execution Steps

#### 2.1 Trigger
- A Hangfire recurring cron job (`PublishDailyActivitiesJob`) runs every day at 11:50 PM in the group's local timezone.

#### 2.2 Routine Locking
- Fetches all `DailyActivity` records logged today (or timezone reference date) that have `IsPublished == false`.
- Updates `IsPublished = true` on each entity record, which blocks further modifications.
- Commits changes to the database. Subsequent requests by partners to edit or add activities for that day will be rejected by domain validation checks.
