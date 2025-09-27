using Microsoft.Extensions.DependencyInjection;

namespace Scopist;

public class ScopedResolver<T> 
    where T : notnull
{
    /// <summary>
    /// Encapsulates a scoped service of type <see cref="T"/> so it can be safely resolved outside a scoped context.
    /// The service of type <see cref="T"/> should be injected via AddScoped.
    /// </summary>
    /// <param name="serviceScope">The scope used to resolve the service</param>
    /// <returns></returns>
    public T Resolve(IServiceScope serviceScope)
    {
        return serviceScope.ServiceProvider.GetRequiredService<T>();
    }
}