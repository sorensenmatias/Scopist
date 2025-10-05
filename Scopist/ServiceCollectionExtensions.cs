using Microsoft.Extensions.DependencyInjection;

namespace Scopist;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddScopist(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton(typeof(IScopedResolver<>), typeof(ScopedResolver<>));

        serviceCollection.AddScopistChecker();
        
        return serviceCollection;
    }

    /// <summary>
    /// Adds a startup checker that validates all usages of IScopedResolver<T>
    /// have T registered with Scoped lifetime. The check runs once when the host starts.
    /// </summary>
    private static IServiceCollection AddScopistChecker(this IServiceCollection serviceCollection)
    {
        var scopistServicesCollection = new ScopistServicesCollection(serviceCollection);
        serviceCollection.AddSingleton(scopistServicesCollection);
        
        // Run validation once the container is built and this singleton is activated.
        serviceCollection.AddActivatedSingleton<ScopistChecker>(sp =>
        {
            var collection = sp.GetRequiredService<ScopistServicesCollection>();
            var scopistChecker = new ScopistChecker(collection);
            scopistChecker.Validate();
            return scopistChecker;
        });

        return serviceCollection;
    }

    /// <summary>
    /// Runs Scopist validation immediately. Use this in tests or manual setups.
    /// </summary>
    public static void ValidateScopist(this IServiceProvider serviceProvider)
    {
        var scopistChecker = serviceProvider.GetRequiredService<ScopistChecker>();
        scopistChecker.Validate();
    }

    public static ServiceProvider BuildAndValidateServiceProvider(this IServiceCollection serviceCollection)
    {
        return serviceCollection.BuildServiceProvider(new ServiceProviderOptions()
        {
            ValidateOnBuild = true,
            ValidateScopes = true
        });
    }
}