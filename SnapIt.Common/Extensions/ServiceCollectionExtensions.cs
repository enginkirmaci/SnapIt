using Prism.Ioc;

namespace SnapIt.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IContainerRegistry AddTransientFromNamespace(
        this IContainerRegistry services,
        string namespaceName,
        params Assembly[] assemblies
    )
    {
        foreach (Assembly assembly in assemblies)
        {
            IEnumerable<Type> types = assembly.GetTypes()
                .Where(x =>
                    x.IsClass &&
                    x.Namespace!.StartsWith(namespaceName, StringComparison.InvariantCultureIgnoreCase)
                );

            foreach (Type? type in types)
            {
                if (!services.IsRegistered(type))
                {
                    services.Register(type);
                }
            }
        }

        return services;
    }
}