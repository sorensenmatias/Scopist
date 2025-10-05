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
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*must be registered as Scoped*");
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
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*must be registered as Scoped*");
    }

    public class MyService(IScopedResolver<ScopedService> myScopedService);

    public class MyService2(IScopedResolver<SingletonService> myScopedService);

    public class SingletonService { }

    public class ScopedService { }
}

