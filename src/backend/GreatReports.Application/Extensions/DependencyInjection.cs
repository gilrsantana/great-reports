using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using GreatReports.Application.Common.CQRS;

namespace GreatReports.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        
        return services
            .RegisterCommandHandlers(assembly)
            .RegisterQueryHandlers(assembly);
    }

    private static IServiceCollection RegisterQueryHandlers(this IServiceCollection services, Assembly assembly)
    {
        var queryHandlerTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && 
                            i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>))
                .Select(i => new { Service = i, Implementation = t }));

        foreach (var type in queryHandlerTypes)
        {
            services.AddScoped(type.Service, type.Implementation);
        }

        return services;
    }

    private static IServiceCollection RegisterCommandHandlers(this IServiceCollection services, Assembly assembly)
    {
        var commandHandlerTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && 
                            (i.GetGenericTypeDefinition() == typeof(ICommandHandler<>) || 
                             i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)))
                .Select(i => new { Service = i, Implementation = t }));

        foreach (var type in commandHandlerTypes)
        {
            services.AddScoped(type.Service, type.Implementation);
        }

        return services;
    }
}
