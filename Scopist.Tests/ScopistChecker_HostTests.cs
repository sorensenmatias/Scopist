using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Scopist.Tests;

public class ScopistChecker_HostTests
{
    [Fact]
    public async Task HostWithScopist_AllowsValidRegistration()
    {
        var hostBuilder = Host.CreateDefaultBuilder();
        using var host = hostBuilder
            .ConfigureServices(services =>
            {
                services.AddScopist();

                services.AddSingleton<MyService>();
                services.AddScoped<ScopedService>();
                services.AddSingleton<SingletonService>();
            })
            .Build();


        var act = async () =>
        {
            await host.StartAsync(TestContext.Current.CancellationToken);
            await host.StopAsync();
        };
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task HostWithScopist_DoesNotAllowInvalidRegistration()
    {
        var hostBuilder = Host.CreateDefaultBuilder();
        using var host = hostBuilder
            .ConfigureServices(services =>
            {
                services.AddScopist();

                services.AddSingleton<ServiceWithInvalidRegistration>();
                services.AddScoped<ScopedService>();
                services.AddSingleton<SingletonService>();
            })
            .Build();


        var act = async () =>
        {
            await host.StartAsync(TestContext.Current.CancellationToken);
            await host.StopAsync();
        };
        
        await act.Should()
            .ThrowExactlyAsync<ScopistValidationException>()
            .WithMessage(
            """
            Scopist validation failed:
            Service Scopist.Tests.ScopistChecker_HostTests+SingletonService must be registered as Scoped for use with IScopedResolver<SingletonService>. Found: Singleton.
            """
            );
    }

    public class MyService(IScopedResolver<ScopedService> myScopedService);

    public class ServiceWithInvalidRegistration(IScopedResolver<SingletonService> myScopedService);

    public class SingletonService { }

    public class ScopedService { }
}