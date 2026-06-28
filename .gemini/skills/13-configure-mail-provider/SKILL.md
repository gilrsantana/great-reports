---
name: configure-mail-provider
description: Guidelines and instructions for configuring, managing, and verifying the email provider integration in the project.
---

# Configure Email Provider

This skill provides step-by-step instructions for configuring, updating, and verifying the integration of the email provider within the project.

---

## 1. Overview of the Integration

The email integration is built using ASP.NET Core dependency injection and typed HTTP clients. It is structured across the following files:

*   **Configuration Schema**: Define a `MailProviderSettings.cs`options class inside the Infrastructure layer at Configurations folder and map from `appsettings` section or `environment variables`:
*   **Dependency Registration**: `DependencyInjection.cs` binds options, configures authorized HTTP clients for administrative tasks (`MailProviderManagerClient`) and sending emails (`MailProviderSenderClient`), and registers the email services.
*   **HTTP Client Factory**: `MailProviderHttpClientFactory.cs` implements the client factory that switches between administrative (manager) and email (sender) clients.
*   **Email Sender Logic**: `MailProviderEmailSender.cs` handles formatting and POSTing email payloads to the email provider API, and logging outcomes using `IEmailAuditLogRepository`.

---

## 2. Configuration Settings

To configure the provider, ensure that the `MailProviderSettings` section in the target configuration file (e.g., `appsettings.json` or user secrets) is fully populated.

### Configuration Template

```json
"MailProviderSettings": {
  "BaseAddress": "set a valid mail provider api base address",
  "Domain": "set a valid mail provider domain",
  "ManagerApiKey": "set a valid mail provider manager api key",
  "SenderApiKey": "set a valid mail provider sender api key",
  "ClientName": "set a valid mail provider client name"
}
```

### Configuration Class

To map appsettings to an options class, use the following class structure:

```csharp

public class MailProviderSettings
{
    public const string SectionName = "MailProviderSettings";
    public string BaseAddress { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public string ManagerApiKey { get; set; } = string.Empty;
    public string SenderApiKey { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
}
```

### Parameter Reference

| Parameter       | Type     | Default Value | Description                                                            |
| :----------------| :---------| :--------------| :-----------------------------------------------------------------------|
| `BaseAddress`   | `string` | `""`          | Base address of the mail provider API.                                 |
| `Domain`        | `string` | `""`          | Domain used for sending emails.                                        |
| `ManagerApiKey` | `string` | `""`          | API key with permissions for managing domains, contacts, or audiences. |
| `SenderApiKey`  | `string` | `""`          | API key with permissions to send emails.                               |
| `ClientName`    | `string` | `""`          | Client name/identifier used in integrations.                           |

> [!NOTE]
> For local development, avoid committing production API keys to `appsettings.json`. Instead, use .NET User Secrets or environment variables matching the structure: `MailProviderSettings__ManagerApiKey` and `MailProviderSettings__SenderApiKey`.


### Dependency Injection Configuration

Register the construction of `MailProviderSettings`, `IMailProviderHttpClientFactory`, `MailProviderManagerClient`, `MailProviderSenderClient` and `IEmailSender` in `DependencyInjection.cs`:

```csharp
// 1. Register MailProviderSettings from appsettings.json
services.Configure<MailProviderSettings>(configuration.GetSection(MailProviderSettings.SectionName));

// 2. Register IMailProviderHttpClientFactory
services.AddScoped<IMailProviderHttpClientFactory, MailProviderHttpClientFactory>();

// 3. Register MailProviderManagerClient for administrative operations
services.AddHttpClient<MailProviderManagerClient>()
    .ConfigurePrimaryHttpMessageHandler(sp =>
    {
        var settings = sp.GetRequiredService<IOptions<MailProviderSettings>>().Value;
        return new HttpClientHandler
        {
            Credentials = new NetworkCredential(settings.ManagerApiKey, "")
        };
    });

// 4. Register MailProviderSenderClient for sending emails
services.AddHttpClient<MailProviderSenderClient>()
    .ConfigurePrimaryHttpMessageHandler(sp =>
    {
        var settings = sp.GetRequiredService<IOptions<MailProviderSettings>>().Value;
        return new HttpClientHandler
        {
            Credentials = new NetworkCredential(settings.SenderApiKey, "")
        };
    });

// 5. Register IEmailSender implementation
services.AddScoped<IEmailSender, MailProviderEmailSender>();
```

---

## 3. Verification and Troubleshooting

To verify the Resend configuration:

1.  **Configure API Keys**: Add valid test keys to your local configuration/secrets.
2.  **Verify Service Registration**: Build and launch the application. Ensure that the dependency injection container successfully resolves `IEmailSender` and Http factory.
3.  **Send Test Email**: Invoke a flow that triggers an email (e.g., registering a new member).
