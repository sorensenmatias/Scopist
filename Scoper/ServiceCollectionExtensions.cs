using Microsoft.Extensions.DependencyInjection;

namespace Scoper;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddScoper(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton(typeof(ScopedResolver<>));
        return serviceCollection;
    }
}