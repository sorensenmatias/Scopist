using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Scopist;

internal class ScopistChecker(ScopistServicesCollection scopistServicesCollection)
{
    public void Validate()
    {
        var resolverTargetTypes = FindResolverTargetTypes(scopistServicesCollection.ServiceCollection);

        if (resolverTargetTypes.Count == 0)
        {
            return;
        }

        var errors = new List<string>();

        foreach (var targetType in resolverTargetTypes.Distinct())
        {
            var descriptor = FindRegistrationFor(scopistServicesCollection.ServiceCollection, targetType);
            if (descriptor == null)
            {
                errors.Add($"No registration found for {targetType}. IScopedResolver<{targetType.Name}> requires {targetType.Name} to be registered as Scoped.");
                continue;
            }

            if (descriptor.Lifetime != ServiceLifetime.Scoped)
            {
                errors.Add($"Service {targetType} must be registered as Scoped for use with IScopedResolver<{targetType.Name}>. Found: {descriptor.Lifetime}.");
            }
        }

        if (errors.Count > 0)
        {
            var message = "Scopist validation failed:\n" + string.Join("\n", errors);
            throw new InvalidOperationException(message);
        }
    }

    private static List<Type> FindResolverTargetTypes(IEnumerable<ServiceDescriptor> descriptors)
    {
        var targets = new List<Type>();

        foreach (var d in descriptors)
        {
            var implementationType = d.ImplementationType;
            if (implementationType == null)
            {
                implementationType = d.ServiceType.IsClass ? d.ServiceType : null;
            }

            if (implementationType == null)
            {
                continue;
            }

            var ctor = ChooseConstructor(implementationType);
            if (ctor == null)
            {
                continue;
            }

            foreach (var p in ctor.GetParameters())
            {
                var pt = p.ParameterType;
                if (pt.IsGenericType && pt.GetGenericTypeDefinition() == typeof(IScopedResolver<>))
                {
                    var target = pt.GetGenericArguments()[0];
                    targets.Add(target);
                }
            }
        }

        return targets;
    }

    private static ConstructorInfo? ChooseConstructor(Type implementationType)
    {
        return implementationType
            .GetConstructors(BindingFlags.Public | BindingFlags.Instance)
            .OrderByDescending(c => c.GetParameters().Length)
            .FirstOrDefault();
    }

    private static ServiceDescriptor? FindRegistrationFor(IEnumerable<ServiceDescriptor> descriptors, Type target)
    {
        return descriptors.FirstOrDefault(d => d.ServiceType == target);
    }
}


