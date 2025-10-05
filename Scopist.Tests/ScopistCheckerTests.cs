using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Scopist.Tests;

public class ScopistCheckerTests
{
    [Fact]
    public void SingletonServiceWithScopedDependency_Works()
    {
        var services = new ServiceCollection();
        services.AddScopist();

        services.AddSingleton<SingletonServiceWithScopedDependency>();
        services.AddScoped<ScopedService>();

        var serviceProvider = services.BuildAndValidateServiceProvider();

        var act = () => serviceProvider.ValidateScopist();
        act.Should().NotThrow();
    }

    [Fact]
    public void ServiceWithWrongLifetime_ThrowsValidationError()
    {
        var services = new ServiceCollection();
        services.AddScopist();

        services.AddSingleton<SingletonServiceWithSingletonDependency>();
        services.AddSingleton<SingletonService>();

        var serviceProvider = services.BuildAndValidateServiceProvider();

        var act = () => serviceProvider.ValidateScopist();
        act.Should()
            .ThrowExactly<ScopistValidationException>()
            .WithMessage("""
                         Scopist validation failed:
                         Service Scopist.Tests.ScopistCheckerTests+SingletonService must be registered as Scoped for use with IScopedResolver<SingletonService>. Found: Singleton.
                         """);
    }

    [Fact]
    public void ServiceWithUnregisteredDependency_ThrowsValidationError()
    {
        var services = new ServiceCollection();
        services.AddScopist();

        services.AddSingleton<SingletonServiceWithScopedDependency>();

        var serviceProvider = services.BuildAndValidateServiceProvider();

        var act = () => serviceProvider.ValidateScopist();
        act.Should()
            .ThrowExactly<ScopistValidationException>()
            .WithMessage("""
                         Scopist validation failed:
                         No registration found for Scopist.Tests.ScopistCheckerTests+ScopedService. IScopedResolver<ScopedService> requires ScopedService to be registered as Scoped.
                         """);
    }

    public class SingletonServiceWithScopedDependency(IScopedResolver<ScopedService> myScopedService);

    public class SingletonServiceWithSingletonDependency(IScopedResolver<SingletonService> myScopedService);

    public class SingletonService { }

    public class ScopedService { }
}

