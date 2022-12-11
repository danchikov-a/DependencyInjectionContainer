namespace DependencyInjectionContainerLibrary.model;

public class DependencyInformation
{
    public readonly Type ClassType;

    public readonly DependencyLifeTime DependencyLifeTime;

    public DependencyInformation(DependencyLifeTime dependencyLifeTime, Type classType)
    {
        ClassType = classType;
        DependencyLifeTime = dependencyLifeTime;
    }
}