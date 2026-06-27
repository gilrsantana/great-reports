# Rule: Relational Database Configuration and Migration Lifecycle

## Metadata
- **ID**: RULE-006-RELATIONAL-DATABASE
- **Scope**: GreatReports.Infrastructure & appsettings.json
- **Target Types**: DbContext, DatabaseOptions, EF Core Mappings
- **Status**: Active

## Overview
All relational database integrations must follow a strict Code-First approach, separating operational runtime options (timeout, retries, logging) into a dedicated `DatabaseOptions` POCO and mapping database entities with robust relational rules (no cascade deletes on domain boundaries).

---

## 1. The Options Pattern for Database Configuration
- Operational database options must be defined in a POCO class named `DatabaseOptions` inside the `Configurations/` folder of the Infrastructure project.
- It must contain a `const string SectionName = "DatabaseOptions";` representing its configuration key.
- It must model:
  - Logging & detail options: `EnableDetailedErrors` (bool), `EnableSensitiveDataLogging` (bool).
  - Performance & Resiliency options: `CommandTimeout` (int), `EnableRetryOnFailure` (bool), `MaxRetryCount` (int), `MaxRetryDelaySeconds` (int), `MaxBatchSize` (int?).

---

## 2. Options Registration and Context Configuration
- Database options must be registered and bound inside the Infrastructure layer's DI class using:

  ```csharp
  public class DatabaseOptions
  {
      public const string SectionName = "DatabaseOptions";
  
      public string ConnectionString { get; set; } = string.Empty;
      public bool EnableDetailedErrors { get; set; }
      public bool EnableSensitiveDataLogging { get; set; }
      public int CommandTimeout { get; set; }
      public bool EnableRetryOnFailure { get; set; }
      public int MaxRetryCount { get; set; }
      public int MaxRetryDelaySeconds { get; set; }
  }
  ```

  ```csharp
  services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));
  ```
- The DbContext is configured using these options to enable connection resiliency and logging.
- Check for connection string existence early:
  ```csharp
  var connectionString = configuration.GetConnectionString("DefaultConnection");
  if (string.IsNullOrEmpty(connectionString))
      throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
  ```
- Database options must be registered in the DI container.
  ```csharp
  services.AddDbContext<GreatReportsDbContext>((serviceProvider, options) =>
  {
      var databaseOptions = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
      if (string.IsNullOrEmpty(databaseOptions.ConnectionString))
      {
          throw new InvalidOperationException("Database connection string not configured");
      }

      var connectionString = databaseOptions.ConnectionString;

      options.UseMySQL(connectionString, mysqlOptions =>
      {
          if (databaseOptions.CommandTimeout > 0)
          {
              mysqlOptions.CommandTimeout(databaseOptions.CommandTimeout);
          }
      });

      if (databaseOptions.EnableDetailedErrors)
      {
          options.EnableDetailedErrors();
      }

      if (databaseOptions.EnableSensitiveDataLogging)
      {
          options.EnableSensitiveDataLogging();
      }
  });
  ```

---

## 3. Code-First Migration Lifecycle
- All database schemas must be modeled code-first.
- Always apply migrations using the EF CLI tool:
  `dotnet ef migrations add <MigrationName> --project src/GreatReports.Infrastructure --startup-project src/GreatReports.Presentation`
- Do not make direct alterations to the database schema. All changes must go through Migrations.

---

## 4. Entity Mapping Standards
- Implement `IEntityTypeConfiguration<T>` in separate files inside `Persistence/Configurations/` format.
- **Cascade Control**: Set `DeleteBehavior.Restrict` for all foreign key mappings on domain boundaries.
- Example mapping rule:
  ```csharp
  builder.HasOne<User>()
      .WithMany()
      .HasForeignKey(x => x.UserId)
      .OnDelete(DeleteBehavior.Restrict);
  ```
