using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Scopist.Tests;

public class ServiceTests
{
    [Fact]
    public void SingletonServiceWithScopedDependency_CanBeResolvedAndExecuted()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScopist();

        serviceCollection.AddSingleton<MyService>();
        serviceCollection.AddScoped<MyScopedService>();

        var serviceProvider = ServiceCollectionExtensions.BuildAndValidateServiceProvider(serviceCollection);

        // Act
        var myService = serviceProvider.GetRequiredService<MyService>();
        
        // Assert
        myService.GetResult().Should().Be(3);
    }
}

public class MyService(ScopedResolver<MyScopedService> myScopedService, IServiceScopeFactory serviceScopeFactory)
{
    public int GetResult()
    {
        var scope = serviceScopeFactory.CreateScope();
        var scopedService = myScopedService.Resolve(scope);
        return scopedService.GetValue();
    }
}

public class MyScopedService
{
    public int GetValue()
    {
        return 3;
    }
}