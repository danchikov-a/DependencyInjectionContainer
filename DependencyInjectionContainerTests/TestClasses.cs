using System.Collections.Generic;

namespace DependencyInjectionContainerTests
{
    interface FirstInterface
    {
    }

    class FirstInterfaceImpl : FirstInterface
    {
        public readonly ThirdInterface Cl;

        public FirstInterfaceImpl(ThirdInterface cl)
        {
            Cl = cl;
        }
    }

    interface SecondInterface
    {
    }

    class SecondInterfaceImpl : SecondInterface
    {
    }

    interface IService
    {
    }

    class FirstIServiceImpl : IService
    {
        public readonly SecondInterface SecondInterface;

        public FirstIServiceImpl()
        {
        }

        public FirstIServiceImpl(SecondInterface secondInterface)
        {
            SecondInterface = secondInterface;
        }
    }

    class SecondIServiceImpl : IService
    {
    }

    interface ThirdInterface
    {
    }

    class ThirdInterfaceImpl : ThirdInterface
    {
        public readonly FirstInterface FirstInterface;

        public ThirdInterfaceImpl(FirstInterface firstInterface)
        {
            FirstInterface = firstInterface;
        }
    }

    class SecondIThirdInterfaceImpl : ThirdInterface
    {
        public readonly IEnumerable<IService> Serv;

        public SecondIThirdInterfaceImpl(IEnumerable<IService> serv)
        {
            Serv = serv;
        }
    }
    
    
    interface IAnother<T>
        where T : SecondInterface
    {
    }

    class First<T> : IAnother<T>
        where T : SecondInterface
    {
    }

    interface IFoo<T>
        where T : IService
    {
    }

    class Second<T> : IFoo<T>
        where T : IService
    {
    }

    public interface IBoo
    {
    }

    public class BooImpl : IBoo
    {
    }
}