namespace DependencyInjectionContainerLibrary.model;

public class DependencyInfo
{
    public readonly Type ClassType;

    public readonly DependencyLifeTime DependencyLifeTime;

    public DependencyInfo(DependencyLifeTime dependencyLifeTime, Type classType)
    {
        ClassType = classType;
        DependencyLifeTime = dependencyLifeTime;
    }
}