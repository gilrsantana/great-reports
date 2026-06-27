using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;
using GreatReports.Application.Extensions;
using GreatReports.Infrastructure.Extensions;

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

        app.MapControllers();

        return app;
    }
}
