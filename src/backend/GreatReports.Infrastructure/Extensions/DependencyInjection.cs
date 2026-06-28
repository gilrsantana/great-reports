using System;
using System.Text;
using GreatReports.Application.Common.Interfaces;
using GreatReports.Infrastructure.Configurations;
using GreatReports.Infrastructure.Identity;
using GreatReports.Infrastructure.Persistence;
using GreatReports.Infrastructure.Persistence.Repositories;
using GreatReports.Infrastructure.Mailer;
using Hangfire;
using Hangfire.MySql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace GreatReports.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .ConfigureOptions(configuration)
            .SetupDatabase(configuration)
            .AddIdentityCore<Account>()
            .AddRoles<Role>()
            .AddEntityFrameworkStores<GreatReportsDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IProviderCompanyRepository, ProviderCompanyRepository>();
        services.AddScoped<IClientCompanyRepository, ClientCompanyRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IGroupRepository, GroupRepository>();
        services.AddScoped<IDailyActivityRepository, DailyActivityRepository>();
        services.AddScoped<IClientContactRepository, ClientContactRepository>();
        services.AddScoped<IEmailVerificationService, EmailVerificationService>();

        services
            .SetAuthentication()
            .SetJwtBearerOptions()
            .SetHangfire(configuration);

        return services;
    }

    private static IServiceCollection ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName))
            .Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName))
            .Configure<IdentityOptions>(configuration.GetSection("IdentityOptions"))
            .Configure<DataProtectionTokenProviderOptions>(configuration.GetSection("DataProtectionTokenProviderOptions"));
    }

    private static IServiceCollection SetupDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        var dbOptions = configuration.GetSection(DatabaseOptions.SectionName).Get<DatabaseOptions>() ?? new DatabaseOptions();

        services.AddDbContext<GreatReportsDbContext>(options =>
        {
            options.UseMySQL(connectionString, mysqlOptions =>
            {
                mysqlOptions.CommandTimeout(dbOptions.CommandTimeout);
                if (dbOptions.EnableRetryOnFailure)
                {
                    mysqlOptions.EnableRetryOnFailure(
                        maxRetryCount: dbOptions.MaxRetryCount,
                        maxRetryDelay: TimeSpan.FromSeconds(dbOptions.MaxRetryDelaySeconds),
                        errorNumbersToAdd: null);
                }
                if (dbOptions.MaxBatchSize.HasValue)
                {
                    mysqlOptions.MaxBatchSize(dbOptions.MaxBatchSize.Value);
                }
            });

            if (dbOptions.EnableDetailedErrors)
            {
                options.EnableDetailedErrors();
            }

            if (dbOptions.EnableSensitiveDataLogging)
            {
                options.EnableSensitiveDataLogging();
            }
        });

        return services;
    }

    private static IServiceCollection SetAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer();

        return services;
    }

    private static IServiceCollection SetJwtBearerOptions(this IServiceCollection services)
    {
        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<JwtSettings>>((options, jwtSettingsOptions) =>
            {
                var jwtSettings = jwtSettingsOptions.Value;

                if (string.IsNullOrEmpty(jwtSettings.Secret))
                {
                    throw new InvalidOperationException("Secret not configured");
                }

                var secret = jwtSettings.Secret;
                var key = Encoding.UTF8.GetBytes(secret);

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

        return services;
    }

    private static IServiceCollection SetHangfire(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire((config) =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseStorage(new MySqlStorage(
                    connectionString,
                    new MySqlStorageOptions
                    {
                        QueuePollInterval = TimeSpan.FromSeconds(15),
                        JobExpirationCheckInterval = TimeSpan.FromHours(1),
                        CountersAggregateInterval = TimeSpan.FromMinutes(5),
                        PrepareSchemaIfNecessary = true,
                        DashboardJobListLimit = 5000,
                        TransactionTimeout = TimeSpan.FromMinutes(1),
                        TablesPrefix = "Hangfire"
                    }));
        });

        services.AddHangfireServer();

        return services;
    }
}
