using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Application.Utilities.SimpleMediator;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRequestHandlersFromAssembly(
        this IServiceCollection services,
        Assembly assembly)
    {
        var handlerTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .SelectMany(t => t.GetInterfaces(), (type, iface) => new { type, iface })
            .Where(t => t.iface.IsGenericType &&
                        t.iface.GetGenericTypeDefinition() == typeof(IRequestHandler<,>));

        foreach (var handler in handlerTypes)
        {
            services.AddTransient(handler.iface, handler.type);
        }

        return services;
    }
}