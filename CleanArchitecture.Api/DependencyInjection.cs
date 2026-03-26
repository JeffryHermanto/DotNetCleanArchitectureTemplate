using CleanArchitecture.Application;
using CleanArchitecture.Domain;
using CleanArchitecture.Infrastructure;

namespace CleanArchitecture.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddAppDI(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddDomainDI(configuration)
            .AddApplicationDI()
            .AddInfrastructureDI();

        return services;
    }
}
