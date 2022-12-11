using DependencyInjectionContainerLibrary.model;

namespace DependencyInjectionContainerLibrary.service;

public class DependencyConfiguration
{
    public readonly Dictionary<Type, List<DependencyInfo>> RegisteredDependencies;

    public DependencyConfiguration()
    {
        RegisteredDependencies = new Dictionary<Type, List<DependencyInfo>>();
    }

    public void Register<TDependency, TImplementation>(DependencyLifeTime dependencyLifeTime
        = DependencyLifeTime.InstancePerDependency)
    {
        Register(typeof(TDependency), typeof(TImplementation), dependencyLifeTime);
    }

    private void Register(Type interfaceType, Type classType, DependencyLifeTime dependencyLifeTime
        = DependencyLifeTime.InstancePerDependency)
    {
        if (!interfaceType.IsInterface && interfaceType != classType
            || classType.IsAbstract
            || !interfaceType.IsAssignableFrom(classType) && !interfaceType.IsGenericTypeDefinition
           )
        {
            throw new Exception();
        }

        if (RegisteredDependencies.ContainsKey(interfaceType))
        {
            RegisteredDependencies[interfaceType].Add(new DependencyInfo(dependencyLifeTime, classType));
        }
        else
        {
            RegisteredDependencies.Add(interfaceType, new List<DependencyInfo>
            {
                new(dependencyLifeTime, classType)
            });
        }
    }
}