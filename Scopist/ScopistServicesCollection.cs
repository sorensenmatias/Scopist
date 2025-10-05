using Microsoft.Extensions.DependencyInjection;

namespace Scopist;

internal class ScopistServicesCollection(IServiceCollection serviceCollection)
{
    public IServiceCollection ServiceCollection { get; } = serviceCollection;
}