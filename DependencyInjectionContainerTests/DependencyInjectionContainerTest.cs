using System.Collections.Generic;
using DependencyInjectionContainerLibrary.model;
using DependencyInjectionContainerLibrary.service;
using NUnit.Framework;

namespace DependencyInjectionContainerTests
{
    [TestFixture]
    public class DependencyInjectionContainerTest
    {
        [Test]
        public void SimpleTest()
        {
            var configuration = new DependencyConfiguration();
            
            configuration.Register<SecondInterface, SecondInterfaceImpl>(DependencyLifeTime.Singleton);
            
            var provider = new DependenciesProvider(configuration);

            var dependency = (SecondInterfaceImpl) provider.Resolve<SecondInterface>();
            
            Assert.IsNotNull(dependency);
        }

        [Test]
        public void DefaultAndSingletonTest()
        {
            var configuration = new DependencyConfiguration();
            configuration.Register<SecondInterface, SecondInterfaceImpl>(DependencyLifeTime.Singleton);
            configuration.Register<IService, FirstIServiceImpl>();
            var provider = new DependenciesProvider(configuration);

            var dependency1 = (SecondInterfaceImpl) provider.Resolve<SecondInterface>();
            var dependency2 = (SecondInterfaceImpl) provider.Resolve<SecondInterface>();
            
            Assert.AreEqual(dependency1, dependency2);
            
            var s1 = provider.Resolve<IService>();
            var s2 = provider.Resolve<IService>();
            
            Assert.AreNotEqual(s1, s2);
        }

        [Test]
        public void SeveralImplementationsTest()
        {
            var configuration = new DependencyConfiguration();
            configuration.Register<IService, FirstIServiceImpl>();
            configuration.Register<IService, SecondIServiceImpl>();
            var provider = new DependenciesProvider(configuration);

            var impls = provider.Resolve<IEnumerable<IService>>();
            
            Assert.IsNotNull(impls);
            Assert.AreEqual(2, ((List<IService>) impls).Count);
        }

        [Test]
        public void InnerDependencyTest()
        {
            var configuration = new DependencyConfiguration();
            configuration.Register<SecondInterface, SecondInterfaceImpl>();
            configuration.Register<IService, FirstIServiceImpl>();
            configuration.Register<IService, SecondIServiceImpl>();
            configuration.Register<ThirdInterface, SecondIThirdInterfaceImpl>();
            var provider = new DependenciesProvider(configuration);

            var dependency1 = (FirstIServiceImpl) provider.Resolve<IService>();
            Assert.IsNotNull(dependency1.SecondInterface);

            var dependency2 = (SecondIThirdInterfaceImpl) provider.Resolve<ThirdInterface>();
            
            Assert.IsNotNull(dependency2.Serv);
            Assert.AreEqual(2, ((List<IService>) dependency2.Serv).Count);
        }

        [Test]
        public void RecursionTest()
        {
            var configuration = new DependencyConfiguration();
            configuration.Register<ThirdInterface, ThirdInterfaceImpl>();
            configuration.Register<FirstInterface, FirstInterfaceImpl>();
            var provider = new DependenciesProvider(configuration);

            var client = (ThirdInterfaceImpl) provider.Resolve<ThirdInterface>();
            
            Assert.IsNull(((FirstInterfaceImpl) client.FirstInterface).Cl);
        }

        [Test]
        public void OneClassTest()
        {
            var configuration = new DependencyConfiguration();
            configuration.Register<BooImpl, BooImpl>();
            var provider = new DependenciesProvider(configuration);
            var humanImpl = provider.Resolve<BooImpl>();
            
            Assert.IsNotNull(humanImpl);
        }
    }
}