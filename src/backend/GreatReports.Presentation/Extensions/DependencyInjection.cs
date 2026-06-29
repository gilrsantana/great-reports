using GreatReports.Application.Extensions;
using GreatReports.Infrastructure.Configurations;
using GreatReports.Infrastructure.Extensions;
using GreatReports.Presentation.Filters;
using Hangfire;
using Microsoft.Extensions.Options;
using Scalar.AspNetCore;

namespace GreatReports.Presentation.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddControllers();
        services.AddOpenApi();

        // Chain register downstream layers
        services.AddApplication()
                .AddInfrastructure(configuration);

        return services;
    }

    public static WebApplication UsePresentationPipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference(options =>
            {
                options.WithTitle("Great Reports API Reference")
                       .WithTheme(ScalarTheme.Mars)
                       .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
            });
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        var jwtSettings = app.Services.GetRequiredService<IOptions<JwtSettings>>().Value;
        app.MapHangfireDashboard("/hangfire", new DashboardOptions
        {
            Authorization = new[]
            {
                new HangfireDashboardAuthorizationFilter(
                    jwtSettings.Secret,
                    jwtSettings.Issuer,
                    jwtSettings.Audience)
            },
            IgnoreAntiforgeryToken = true
        });

        app.MapControllers();

        return app;
    }
}
