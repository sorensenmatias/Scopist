using Microsoft.Extensions.DependencyInjection;

namespace Scopist;

/// <summary>
/// Provides a safe way to resolve a scoped service <typeparamref name="T"/> 
/// from an <see cref="IServiceScope"/> without directly depending on the container.
/// </summary>
/// <typeparam name="T">The type of the scoped service.</typeparam>
public interface IScopedResolver<out T> where T : notnull
{
    /// <summary>
    /// Resolves the scoped service from the provided <see cref="IServiceScope"/>.
    /// </summary>
    /// <param name="scope">The <see cref="IServiceScope"/> to resolve from.</param>
    /// <returns>An instance of <typeparamref name="T"/>.</returns>
    T Resolve(IServiceScope scope);
}