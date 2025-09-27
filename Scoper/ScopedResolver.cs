using Microsoft.Extensions.DependencyInjection;

namespace Scoper;

public class ScopedResolver<T> 
    where T : notnull
{
    public T Resolve(IServiceScope serviceScope)
    {
        return serviceScope.ServiceProvider.GetRequiredService<T>();
    }
}