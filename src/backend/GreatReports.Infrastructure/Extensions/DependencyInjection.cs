using System.Net;
using System.Text;
using GreatReports.Application.Common.Interfaces;
using GreatReports.Infrastructure.Configurations;
using GreatReports.Infrastructure.Identity;
using GreatReports.Infrastructure.Mailer;
using GreatReports.Infrastructure.BackgroundJobs;
using GreatReports.Infrastructure.Persistence;
using GreatReports.Infrastructure.Persistence.Repositories;
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

        services
            .SetAuthentication()
            .SetJwtBearerOptions()
            .SetHangfire(configuration);

        services.ConfigureHttpClient()
            .ConfigureRepository()
            .ConfigureInfrastructureServices();

        return services;
    }

    private static IServiceCollection ConfigureInfrastructureServices(this IServiceCollection services)
    {
        return
            services.AddScoped<IIdentityService, IdentityService>()
                .AddScoped<IMailProviderHttpClientFactory, MailProviderHttpClientFactory>()
                .AddScoped<IEmailSender, MailProviderEmailSender>()
                .AddScoped<IBackgroundJobService, BackgroundJobService>()
                .AddScoped<IEmailSender<Account>, IdentityEmailSender>();
    }

    private static IServiceCollection ConfigureRepository(this IServiceCollection services)
    {
        return
            services.AddScoped<IProviderCompanyRepository, ProviderCompanyRepository>()
                .AddScoped<IClientCompanyRepository, ClientCompanyRepository>()
                .AddScoped<IProjectRepository, ProjectRepository>()
                .AddScoped<IUserRepository, UserRepository>()
                .AddScoped<IGroupRepository, GroupRepository>()
                .AddScoped<IDailyActivityRepository, DailyActivityRepository>()
                .AddScoped<IClientContactRepository, ClientContactRepository>()
                .AddScoped<IEmailAuditLogRepository, EmailAuditLogRepository>()
                .AddScoped<IScheduledEmailRepository, ScheduledEmailRepository>()
                .AddScoped<IScheduledEmailReceiverRepository, ScheduledEmailReceiverRepository>();
    }

    private static IServiceCollection ConfigureHttpClient(this IServiceCollection services)
    {
        services.AddHttpClient<MailProviderManagerClient>((sp, client) =>
            {
                var settings = sp.GetRequiredService<IOptions<MailProviderSettings>>().Value;
                if (!string.IsNullOrEmpty(settings.BaseAddress))
                {
                    client.BaseAddress = new Uri(settings.BaseAddress);
                }
            })
            .ConfigurePrimaryHttpMessageHandler(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<MailProviderSettings>>().Value;
                return new HttpClientHandler
                {
                    Credentials = new NetworkCredential(settings.ManagerApiKey, "")
                };
            });

        services.AddHttpClient<MailProviderSenderClient>((sp, client) =>
            {
                var settings = sp.GetRequiredService<IOptions<MailProviderSettings>>().Value;
                if (!string.IsNullOrEmpty(settings.BaseAddress))
                {
                    client.BaseAddress = new Uri(settings.BaseAddress);
                }
            })
            .ConfigurePrimaryHttpMessageHandler(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<MailProviderSettings>>().Value;
                return new HttpClientHandler
                {
                    Credentials = new NetworkCredential(settings.SenderApiKey, "")
                };
            });
        return services;
    }

    private static IServiceCollection ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName))
            .Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName))
            .Configure<IdentityOptions>(configuration.GetSection("IdentityOptions"))
            .Configure<DataProtectionTokenProviderOptions>(configuration.GetSection("DataProtectionTokenProviderOptions"))
            .Configure<MailProviderSettings>(configuration.GetSection(MailProviderSettings.SectionName))
            .Configure<ClientSettings>(configuration.GetSection(ClientSettings.SectionName));
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

        connectionString = CleanConnectionString(connectionString, isHangfire: false);
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

            connectionString = CleanConnectionString(connectionString, isHangfire: true);

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

        services.AddHangfireServer(options => options.WorkerCount = 2);

        return services;
    }

    private static string CleanConnectionString(string connectionString, bool isHangfire)
    {
        var cleaned = connectionString;

        if (isHangfire)
        {
            // MySqlConnector (used by Hangfire) expects SslMode=None (Disabled is not supported)
            if (cleaned.Contains("SslMode=Disabled", StringComparison.OrdinalIgnoreCase))
            {
                cleaned = cleaned.Replace("SslMode=Disabled", "SslMode=None", StringComparison.OrdinalIgnoreCase);
            }
            
            // Hangfire.MySqlStorage requires "Allow User Variables=True"
            if (!cleaned.Contains("Allow User Variables=True", StringComparison.OrdinalIgnoreCase))
            {
                if (!cleaned.EndsWith(";")) cleaned += ";";
                cleaned += "Allow User Variables=True;";
            }
        }
        else
        {
            // Oracle's MySql.Data 9.5.0+ (used by MySql.EntityFrameworkCore) removed SslMode=None, throwing an ArgumentException.
            // We must use SslMode=Disabled for the EF Core connection.
            if (cleaned.Contains("SslMode=None", StringComparison.OrdinalIgnoreCase))
            {
                cleaned = cleaned.Replace("SslMode=None", "SslMode=Disabled", StringComparison.OrdinalIgnoreCase);
            }
        }

        return cleaned;
    }
}
