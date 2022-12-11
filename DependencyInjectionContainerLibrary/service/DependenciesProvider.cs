using System.Collections.Concurrent;
using DependencyInjectionContainerLibrary.model;

namespace DependencyInjectionContainerLibrary.service;

public class DependenciesProvider
{
    private readonly DependencyConfiguration _configuration;

    private readonly ConcurrentDictionary<Type, object> _singletonImplementations = new();

    private readonly Stack<Type> _recursionStackResolver = new();

    public DependenciesProvider(DependencyConfiguration configuration)
    {
        _configuration = configuration;
    }

    public TDependency Resolve<TDependency>()
    {
        return (TDependency) Resolve(typeof(TDependency));
    }

    private object Resolve(Type type)
    {
        var dependenciesInformations = GetDependenciesInformations(type);

        if (dependenciesInformations == null && type.GetGenericTypeDefinition() != typeof(IEnumerable<>))
        {
            throw new Exception();
        }

        if (_recursionStackResolver.Contains(type))
        {
            return null;
        }

        _recursionStackResolver.Push(type);

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        {
            type = type.GetGenericArguments()[0];
            dependenciesInformations = GetDependenciesInformations(type);

            if (dependenciesInformations == null)
            {
                throw new Exception();
            }

            return ConvertToIEnumerable(dependenciesInformations
                .Select(info => GetImplementation(info, type))
                .ToList(), type);
        }

        var obj = GetImplementation(dependenciesInformations[0], type);
        _recursionStackResolver.Pop();

        return obj;
    }

    private List<DependencyInformation> GetDependenciesInformations(Type dependencyType)
    {
        if (_configuration.RegisteredDependencies.ContainsKey(dependencyType))
        {
            return _configuration.RegisteredDependencies[dependencyType];
        }

        if (!dependencyType.IsGenericType)
        {
            return null;
        }

        var definition = dependencyType.GetGenericTypeDefinition();

        return _configuration.RegisteredDependencies.ContainsKey(definition)
            ? _configuration.RegisteredDependencies[definition]
            : null;
    }

    private object GetImplementation(DependencyInformation implInformation, Type resolvingDependency)
    {
        Type innerTypeForOpenGeneric = null;

        if (implInformation.ClassType.IsGenericType
            && implInformation.ClassType.IsGenericTypeDefinition
            && implInformation.ClassType.GetGenericArguments()[0].FullName == null
           )
        {
            innerTypeForOpenGeneric = resolvingDependency.GetGenericArguments().FirstOrDefault();
        }

        if (implInformation.DependencyLifeTime == DependencyLifeTime.Singleton)
        {
            if (!_singletonImplementations.ContainsKey(implInformation.ClassType))
            {
                _singletonImplementations.TryAdd(
                    implInformation.ClassType,
                    CreateInstanceForDependency(implInformation.ClassType, innerTypeForOpenGeneric)
                );
            }

            return _singletonImplementations[implInformation.ClassType];
        }

        return CreateInstanceForDependency(implInformation.ClassType, innerTypeForOpenGeneric);
    }

    private object CreateInstanceForDependency(Type implClassType, Type innerTypeForOpenGeneric)
    {
        var constructors = implClassType
            .GetConstructors()
            .OrderByDescending(x => x.GetParameters().Length)
            .ToArray();
        
        object implementationInstance = null;

        foreach (var constructor in constructors)
        {
            var paramsValues = new List<object>();

            foreach (var parameter in constructor.GetParameters())
            {
                if (IsDependency(parameter.ParameterType))
                {
                    var obj = Resolve(parameter.ParameterType);
                    paramsValues.Add(obj);
                }
                else
                {
                    object obj = null;

                    try
                    {
                        obj = Activator.CreateInstance(parameter.ParameterType, null);
                    }
                    catch
                    {
                        // ignored
                    }

                    paramsValues.Add(obj);
                }
            }

            try
            {
                if (innerTypeForOpenGeneric != null)
                {
                    implClassType = implClassType.MakeGenericType(innerTypeForOpenGeneric);
                }

                implementationInstance = Activator.CreateInstance(implClassType, paramsValues.ToArray());
                
                break;
            }
            catch
            {
                // ignored
            }
        }

        return implementationInstance;
    }

    private object ConvertToIEnumerable(List<object> implementations, Type type)
    {
        var enumerableType = typeof(Enumerable);
        var castMethod = enumerableType.GetMethod(nameof(Enumerable.Cast))?.MakeGenericMethod(type);
        var toListMethod = enumerableType.GetMethod(nameof(Enumerable.ToList))?.MakeGenericMethod(type);

        IEnumerable<object> itemsToCast = implementations;

        var castedItems = castMethod?.Invoke(null, new[]
        {
            itemsToCast
        });

        return toListMethod?.Invoke(null, new[] {castedItems});
    }

    private bool IsDependency(Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>)
            ? IsDependency(type.GetGenericArguments()[0])
            : _configuration.RegisteredDependencies.ContainsKey(type);
    }
}