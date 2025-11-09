using Microsoft.Extensions.DependencyInjection;

namespace SnapIt.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTransientFromNamespace(
        this IServiceCollection services,
        string namespaceName,
        params Assembly[] assemblies
    )
    {
        foreach (Assembly assembly in assemblies)
        {
            IEnumerable<Type> types = assembly.GetTypes()
                .Where(x =>
                    x.IsClass &&
                    x.Namespace != null &&
                    x.Namespace!.StartsWith(namespaceName, StringComparison.InvariantCultureIgnoreCase)
                );

            foreach (Type? type in types)
            {
                if (!services.Any(sd => sd.ImplementationType == type))
                {
                    services.AddSingleton(type);
                }
            }
        }

        return services;
    }
}