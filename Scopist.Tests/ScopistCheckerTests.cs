using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Scopist.Tests;

public class ScopistCheckerTests
{
    [Fact]
    public void AddScopistChecker_Allows_Scoped_Targets()
    {
        var services = new ServiceCollection();
        services.AddScopist();

        services.AddSingleton<MyService>();
        services.AddScoped<ScopedService>();
        services.AddSingleton<SingletonService>();

        var serviceProvider = services.BuildAndValidateServiceProvider();

        var act = () => serviceProvider.ValidateScopist();
        act.Should().NotThrow();
    }

    [Fact]
    public void AddScopistChecker_Throws_For_NonScoped_Targets()
    {
        var services = new ServiceCollection();
        services.AddScopist();

        services.AddSingleton<MyService2>();
        services.AddScoped<ScopedService>();
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
    public void ValidateScopist_Manual_Call_Throws_For_NonScoped_Targets()
    {
        var services = new ServiceCollection();
        services.AddScopist();

        services.AddSingleton<MyService2>();
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

        services.AddSingleton<MyService>();

        var serviceProvider = services.BuildAndValidateServiceProvider();

        var act = () => serviceProvider.ValidateScopist();
        act.Should()
            .ThrowExactly<ScopistValidationException>()
            .WithMessage("""
                         Scopist validation failed:
                         No registration found for Scopist.Tests.ScopistCheckerTests+ScopedService. IScopedResolver<ScopedService> requires ScopedService to be registered as Scoped.
                         """);


    }

    public class MyService(IScopedResolver<ScopedService> myScopedService);

    public class MyService2(IScopedResolver<SingletonService> myScopedService);

    public class SingletonService { }

    public class ScopedService { }
}

