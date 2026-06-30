---
name: setup-identity-and-auth
description: Set up ASP.NET Core Identity, JWT authentication, token validation, refresh token rotation, controller endpoints, and background job mail workflows.
---

# Skill: Setting Up Identity and JWT Authentication

This skill guides you through setting up ASP.NET Core Identity, JWT Bearer authentication, refresh token rotation, presentation controllers, error mapping, and background mail delivery workflows.

---

## Interactive Setup Workflow

Before writing code or planning the authentication setup, you **MUST** ask the user:
1. **Will it use a background job to send e-mail confirmation?**
2. **Will it use Hangfire?**

Depending on the choices:
- **If background job is chosen, but NOT Hangfire**: You must use the dedicated skill [create-background-job](../15-create-background-job/SKILL.md) to implement a native background task queue using `BackgroundService`.
- **If Hangfire is chosen**: You must use the [hangfire-background-jobs](../14-hangfire-background-jobs/SKILL.md) skill to configure Hangfire and enqueue tasks.
- **If neither is chosen**: Perform direct, synchronous calls to your email dispatch service (e.g., direct SMTP/API request).

---

## Steps

### 1. Install Required NuGet Packages
Ensure the following packages are installed:
- **GreatReports.Infrastructure**:
  - `Microsoft.AspNetCore.Identity.EntityFrameworkCore`
  - `System.IdentityModel.Tokens.Jwt`
- **GreatReports.Presentation**:
  - `Microsoft.AspNetCore.Authentication.JwtBearer`

---

### 2. Define the Custom Identity Entities
In your Infrastructure project (under `Identity/`), create the Identity entities:

- **`Account.cs`** (inheriting from `IdentityUser<Guid>`):
  ```csharp
  using Microsoft.AspNetCore.Identity;

  namespace GreatReports.Infrastructure.Identity;

  public class Account : IdentityUser<Guid>
  {
      public string RefreshToken { get; private set; } = string.Empty;
      public DateTime RefreshTokenExpiryTime { get; private set; }

      public Account() { }

      private Account(Guid id, string email)
      {
          Id = id;
          Email = email;
          UserName = email;
      }

      public static Account Create(Guid id, string email) => new Account(id, email);

      public void UpdateRefreshToken(string refreshToken, DateTime expiryTime)
      {
          RefreshToken = refreshToken;
          RefreshTokenExpiryTime = expiryTime;
      }

      public void UpdateLockoutStatus(bool blocked)
      {
          LockoutEnd = blocked ? DateTimeOffset.UtcNow.AddYears(100) : null;
      }
  }
  ```

- **`Role.cs`** (inheriting from `IdentityRole<Guid>`):
  ```csharp
  using Microsoft.AspNetCore.Identity;

  namespace GreatReports.Infrastructure.Identity;

  public class Role : IdentityRole<Guid>
  {
      public string Description { get; private set; } = string.Empty;

      private Role() { }

      private Role(string name, string description)
      {
          Name = name;
          NormalizedName = name.ToUpperInvariant();
          Description = description;
      }

      public static Role Create(string name, string description) => new Role(name, description);
  }
  ```

---

### 3. Rename Identity Database Tables
By default, EF Core creates tables named `AspNetUsers`, `AspNetRoles`, etc. Override this behavior to rename all Identity tables to use `identity` as a prefix (e.g. `identityRoles`, `identityUsers`) by applying Entity Type Configurations in `Persistence/Configurations/`:

- **AccountConfiguration**:
  ```csharp
  builder.ToTable("identityUsers");
  ```
- **RoleConfiguration**:
  ```csharp
  builder.ToTable("identityRoles");
  ```
- **AccountRoleConfiguration**:
  ```csharp
  builder.ToTable("identityUserRoles");
  ```
- **AccountClaimConfiguration**:
  ```csharp
  builder.ToTable("identityUserClaims");
  ```
- **AccountLoginConfiguration**:
  ```csharp
  builder.ToTable("identityUserLogins");
  ```
- **RoleClaimConfiguration**:
  ```csharp
  builder.ToTable("identityRoleClaims");
  ```
- **AccountTokenConfiguration**:
  ```csharp
  builder.ToTable("identityUserTokens");
  ```

Apply the configurations using `builder.ApplyConfigurationsFromAssembly` inside the `GreatReportsDbContext` class.

---

### 4. Create JWT Token Configuration
Define a `JwtSettings.cs` options class inside the Infrastructure layer:
```csharp
namespace GreatReports.Infrastructure.Configurations;

public class JwtSettings
{
    public const string SectionName = "JwtSettings";

    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpiryInMinutes { get; set; }
    public int RefreshTokenExpireInDays { get; set; }
    public bool ValidateIssuer { get; set; }
    public bool ValidateAudience { get; set; }
    public bool ValidateLifetime { get; set; }
    public bool ValidateIssuerSigningKey { get; set; }
}
```
Add settings to `appsettings.json`:
```json
"JwtSettings": {
  "Secret": "A_SUPER_LONG_JWT_SIGNING_KEY_EXCEEDING_256_BITS",
  "Issuer": "GreatReportsAPI",
  "Audience": "GreatReportsAPI",
  "ExpiryInMinutes": 60,
  "RefreshTokenExpireInDays": 7,
  "ValidateIssuer": true,
  "ValidateAudience": true,
  "ValidateLifetime": true,
  "ValidateIssuerSigningKey": true
}
```

---

### 5. Wire Identity and Authentication in Dependency Injection
Open the Infrastructure layer's Dependency Injection config (`DependencyInjection.cs`):

1. **Configure Identity Core**:
   ```csharp
   services.Configure<IdentityOptions>(configuration.GetSection("IdentityOptions"));
   services.Configure<DataProtectionTokenProviderOptions>(configuration.GetSection("DataProtectionTokenProviderOptions"));

   services.AddIdentityCore<Account>()
       .AddRoles<Role>()
       .AddEntityFrameworkStores<GreatReportsDbContext>()
       .AddDefaultTokenProviders();
   ```

2. **Configure Authentication & JWT Bearer Options**:
   ```csharp
   services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

   services.AddAuthentication(options =>
   {
       options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
       options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
   })
   .AddJwtBearer();

   services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
       .Configure<IOptions<JwtSettings>>((options, jwtSettingsOptions) =>
       {
           var jwtSettings = jwtSettingsOptions.Value;
           var key = Encoding.UTF8.GetBytes(jwtSettings.Secret);
           options.TokenValidationParameters = new TokenValidationParameters
           {
               ValidateIssuer = jwtSettings.ValidateIssuer,
               ValidateAudience = jwtSettings.ValidateAudience,
               ValidateLifetime = jwtSettings.ValidateLifetime,
               ValidateIssuerSigningKey = jwtSettings.ValidateIssuerSigningKey,
               ValidIssuer = jwtSettings.Issuer,
               ValidAudience = jwtSettings.Audience,
               IssuerSigningKey = new SymmetricSecurityKey(key),
               ClockSkew = TimeSpan.Zero
           };
       });
   ```

3. **Map Pipeline Middlewares** inside Presentation's composition root:
   ```csharp
   app.UseAuthentication();
   app.UseAuthorization();
   ```

---

### 6. Implement the IdentityService
Implement the `IIdentityService` interface (defined in the Application layer) inside `GreatReports.Infrastructure/Identity/IdentityService.cs`. It must handle:
- **`AuthenticateAsync`**: Find account by email, check password using `userManager.CheckPasswordAsync`, verify email is confirmed and account is not locked out, then issue access and refresh tokens.
- **`ChangePasswordAsync`**: Find account by ID and call `userManager.ChangePasswordAsync`. Map any failures into a `ValidationError` collection.
- **`GeneratePasswordResetTokenAsync`**: Generate token using `userManager.GeneratePasswordResetTokenAsync`, build the reset URL link, and send the email (synchronously, natively via background queue, or Hangfire).
- **`ResetPasswordAsync`**: Redefine user password using `userManager.ResetPasswordAsync`.
- **`RotateTokensAsync`**: Authenticate client refresh token, check expiration, and rotate tokens.

---

### 7. Define Controller Endpoints
Expose these actions in an `AuthController` inheriting from `ApiControllerBase`:

- **Login**: `POST /api/auth/login` (decorated with `[AllowAnonymous]`)
- **Refresh Token**: `POST /api/auth/refresh-token` (decorated with `[AllowAnonymous]`)
- **Change Password**: `POST /api/auth/change-password` (decorated with `[Authorize]`)
- **Forget Password**: `POST /api/auth/forget-password` / `[HttpPost("forgot-password")]` (decorated with `[AllowAnonymous]`)
- **Reset Password**: `POST /api/auth/reset-password` (decorated with `[AllowAnonymous]`)

---

### 8. Implement Error Status Mapping in ApiControllerBase
To align with clean architecture guidelines, error handling must occur globally or inside the base API controller. Map domain error codes into HTTP status codes:
- Match `Auth.InvalidCredentials`, `Auth.InvalidToken`, `Token.Expired`, `Auth.InvalidRefreshToken`, `Auth.EmailNotConfirmed`, `Auth.AccountLocked` -> `401 Unauthorized`
- Match `NotFound` -> `404 Not Found`
- Validation errors -> `400 BadRequest` containing validation failures in `ProblemDetails`.
