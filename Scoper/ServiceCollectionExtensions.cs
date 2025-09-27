using Microsoft.Extensions.DependencyInjection;

namespace Scoper;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddScoper(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton(typeof(ScopedResolver<>));
        return serviceCollection;
    }

    public static ServiceProvider BuildAndValidateServiceProvider(IServiceCollection serviceCollection)
    {
        return serviceCollection.BuildServiceProvider(new ServiceProviderOptions()
        {
            ValidateOnBuild = true,
            ValidateScopes = true
        });
    }
}