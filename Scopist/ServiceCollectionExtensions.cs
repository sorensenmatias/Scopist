using Microsoft.Extensions.DependencyInjection;

namespace Scopist;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddScopist(this IServiceCollection serviceCollection)
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