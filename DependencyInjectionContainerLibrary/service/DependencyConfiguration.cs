using DependencyInjectionContainerLibrary.model;

namespace DependencyInjectionContainerLibrary.service;

public class DependencyConfiguration
{
    public readonly Dictionary<Type, List<DependencyInformation>> RegisteredDependencies;

    public DependencyConfiguration()
    {
        RegisteredDependencies = new Dictionary<Type, List<DependencyInformation>>();
    }

    public void Register<TDependency, TImplementation>(DependencyLifeTime dependencyLifeTime
        = DependencyLifeTime.InstancePerDependency)
    {
        var interfaceType = typeof(TDependency);
        var classType = typeof(TImplementation);

        if (!interfaceType.IsInterface && interfaceType != classType
            || classType.IsAbstract
            || !interfaceType.IsAssignableFrom(classType) && !interfaceType.IsGenericTypeDefinition
           )
        {
            throw new Exception();
        }

        if (RegisteredDependencies.ContainsKey(interfaceType))
        {
            RegisteredDependencies[interfaceType].Add(new DependencyInformation(dependencyLifeTime, classType));
        }
        else
        {
            RegisteredDependencies.Add(interfaceType, new List<DependencyInformation>
            {
                new(dependencyLifeTime, classType)
            });
        }
    }
}