---
name: setup-identity-and-auth
description: Set up ASP.NET Core Identity, JWT authentication, token validation, and refresh token rotation in a new .NET Web API project.
---

# Skill: Setting Up Identity and JWT Authentication

This skill guides you through setting up ASP.NET Core Identity, JWT Bearer authentication, and refresh token rotation.

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
By default, EF Core creates tables named `AspNetUsers`, `AspNetRoles`, etc. Override this behavior by applying Entity Type Configurations in `Persistence/Configurations/` (for each entity you want to rename, you need to create a configuration). Examples:

- **AccountConfiguration**:
  ```csharp
  builder.ToTable("Accounts");
  ```
- **RoleConfiguration**:
  ```csharp
  builder.ToTable("Roles");
  ```
- **AccountRoleConfiguration**:
  ```csharp
  builder.ToTable("AccountRoles");
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

1. **Add Identity Services with Options Pattern**:
   Configure options from `appsettings.json`:
   ```csharp
   services.Configure<IdentityOptions>(configuration.GetSection("IdentityOptions"));
   services.Configure<DataProtectionTokenProviderOptions>(configuration.GetSection("DataProtectionTokenProviderOptions"));

   services.AddIdentityCore<Account>()
       .AddRoles<Role>()
       .AddEntityFrameworkStores<GreatReportsDbContext>()
       .AddDefaultTokenProviders();
   ```

2. **Configure Authentication & JWT Bearer Options**:
   Register authentication and bind JWT bearer settings using options pattern:
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
Create an interface `IIdentityService` in the Application layer, and implement it in `GreatReports.Infrastructure/Identity/IdentityService.cs`. Use `UserManager<Account>` and `RoleManager<Role>` to manage credentials:

- **GenerateAccessToken**: Generate claims (Sub, Jti, Email, roles) and write the token using `JwtSecurityTokenHandler`.
- **GenerateRefreshToken**: Create a cryptographically secure random token:
  ```csharp
  var randomNumber = new byte[64];
  using var rng = RandomNumberGenerator.Create();
  rng.GetBytes(randomNumber);
  var refreshToken = Convert.ToBase64String(randomNumber);
  ```
- **Rotate Tokens**: Upon receiving a validation request, call `GetPrincipalFromExpiredToken`, load the associated `Account`, match the `RefreshToken` and its expiration, and issue a fresh Access Token + rotated Refresh Token.
