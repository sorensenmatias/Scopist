using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Scoper;

public static class Program
{
    public static void Main(string[] args)
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoper();

        serviceCollection.AddSingleton<MyService>();
        serviceCollection.AddScoped<MyScopedService>();

        var serviceProvider = serviceCollection.BuildServiceProvider(new ServiceProviderOptions()
            { ValidateOnBuild = true, ValidateScopes = true });

        var myService = serviceProvider.GetRequiredService<MyService>();
        
        myService.Execute();
    }
}

public class MyService(ScopedResolver<MyScopedService> myScopedService, IServiceScopeFactory serviceScopeFactory)
{
    public void Execute()
    {
        var scope = serviceScopeFactory.CreateScope();
        var scopedService = myScopedService.Resolve(scope);
        Console.WriteLine(scopedService.GetValue());
    }
}

public class MyScopedService
{
    public int GetValue()
    {
        return 3;
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddScoper(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton(typeof(ScopedResolver<>));
        return serviceCollection;
    }
}

public class ScopedResolver<T> 
    where T : notnull
{
    public T Resolve(IServiceScope serviceScope)
    {
        return serviceScope.ServiceProvider.GetRequiredService<T>();
    }
}