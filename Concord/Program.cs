using System;
using System.Collections.Generic;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Diagnostics;
using Nancy.Hosting.Self;
using Nancy.Testing;
using Nancy.TinyIoc;

namespace Concord
{
    /*public class CustomBootstrapper : NancyBootstrapperWithRequestContainerBase<TinyIoCContainer>
    {
        private readonly List<NancyModule> _modules;

        public CustomBootstrapper()
        {
            _modules = new List<NancyModule>();
        }

        public void AddModule(NancyModule module)
        {
            _modules.Add(module);
        }

        protected override IDiagnostics GetDiagnostics()
        {
            return this.ApplicationContainer.Resolve<IDiagnostics>();
        }

        protected override IEnumerable<IApplicationStartup> GetApplicationStartupTasks()
        {
            return this.ApplicationContainer.ResolveAll<IApplicationStartup>(false);
        }

        protected override IEnumerable<IRequestStartup> RegisterAndGetRequestStartupTasks(TinyIoCContainer container, Type[] requestStartupTypes)
        {
            container.RegisterMultiple(typeof(IRequestStartup), requestStartupTypes);

            return container.ResolveAll<IRequestStartup>(false);
        }

        protected override IEnumerable<IRegistrations> GetRegistrationTasks()
        {
            return this.ApplicationContainer.ResolveAll<IRegistrations>(false);
        }

        protected override INancyEngine GetEngineInternal()
        {
            return this.ApplicationContainer.Resolve<INancyEngine>();
        }

        protected override TinyIoCContainer GetApplicationContainer()
        {
            return new TinyIoCContainer();
        }

        protected override void RegisterBootstrapperTypes(TinyIoCContainer applicationContainer)
        {
            applicationContainer.Register<INancyModuleCatalog>(this);
        }

        protected override void RegisterTypes(TinyIoCContainer container, IEnumerable<TypeRegistration> typeRegistrations)
        {
            foreach (var typeRegistration in typeRegistrations)
            {
                switch (typeRegistration.Lifetime)
                {
                    case Lifetime.Transient:
                        container.Register(typeRegistration.RegistrationType, typeRegistration.ImplementationType).AsMultiInstance();
                        break;
                    case Lifetime.Singleton:
                        container.Register(typeRegistration.RegistrationType, typeRegistration.ImplementationType).AsSingleton();
                        break;
                    case Lifetime.PerRequest:
                        throw new InvalidOperationException("Unable to directly register a per request lifetime.");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        protected override void RegisterCollectionTypes(TinyIoCContainer container, IEnumerable<CollectionTypeRegistration> collectionTypeRegistrationsn)
        {
            foreach (var collectionTypeRegistration in collectionTypeRegistrationsn)
            {
                switch (collectionTypeRegistration.Lifetime)
                {
                    case Lifetime.Transient:
                        container.RegisterMultiple(collectionTypeRegistration.RegistrationType, collectionTypeRegistration.ImplementationTypes).AsMultiInstance();
                        break;
                    case Lifetime.Singleton:
                        container.RegisterMultiple(collectionTypeRegistration.RegistrationType, collectionTypeRegistration.ImplementationTypes).AsSingleton();
                        break;
                    case Lifetime.PerRequest:
                        throw new InvalidOperationException("Unable to directly register a per request lifetime.");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        protected override void RegisterInstances(TinyIoCContainer container, IEnumerable<InstanceRegistration> instanceRegistrations)
        {
            foreach (var instanceRegistration in instanceRegistrations)
            {
                container.Register(
                    instanceRegistration.RegistrationType,
                    instanceRegistration.Implementation);
            }
        }

        protected override TinyIoCContainer CreateRequestContainer()
        {
            return this.ApplicationContainer.GetChildContainer();
        }

        protected override void RegisterRequestContainerModules(TinyIoCContainer container, IEnumerable<ModuleRegistration> moduleRegistrationTypes)
        {
            foreach (var module in _modules)
            {
                container.Register(typeof(INancyModule), module.GetType(), module).AsSingleton();
            }

            foreach (var moduleRegistrationType in moduleRegistrationTypes)
            {
                if (moduleRegistrationType.ModuleType == typeof (PactServiceNancyModule))
                {
                    continue;
                }

                container.Register(
                    typeof(INancyModule),
                    moduleRegistrationType.ModuleType,
                    moduleRegistrationType.ModuleType.FullName).
                    AsSingleton();
            }
        }

        protected override IEnumerable<INancyModule> GetAllModules(TinyIoCContainer container)
        {
            var nancyModules = container.ResolveAll<INancyModule>(false);
            return nancyModules;
        }

        protected override INancyModule GetModule(TinyIoCContainer container, Type moduleType)
        {
            container.Register(typeof(INancyModule), moduleType);

            return container.Resolve<INancyModule>();
        }
    }*/

    internal class Program
    {
        private static void Main()
        {
            //var temp = new PactService(1234);

            //temp.Start();

            //var test = new PactServiceNancyModule(HttpVerb.Get, "/events", new PactServiceRequest(), new PactServiceResponse { Status = 200, Body = "hello" });

            PactServiceNancyModule.Set(HttpVerb.Get, "/events", new PactServiceRequest(), new PactServiceResponse { Status = 200, Body = "hello" });

            //var test2 = new ModuleRegistration(typeof(PactServiceNancyModule));
            //var b = new Browser(with => with.Module(test));

            //b.Get("/", with)

            //Console.ReadKey();
            var hostConfig = new HostConfiguration
            {
                UrlReservations = { CreateAutomatically = true }
            };
            //var bootstrapper = new CustomBootstrapper();

            /*var bootstrapper = new ConfigurableBootstrapper(with =>
            {
                //with.Module(test);
            });*/

            //var bootstrapper = new CustomBootstrapper();
            //bootstrapper.AddModule(test);

            //ConfigurableBootstrapper.ConfigurableBoostrapperConfigurator

            //using (var host = new NancyHost(bootstrapper, hostConfig, new Uri("http://localhost:1234")))
            using (var host = new NancyHost(hostConfig, new Uri("http://localhost:1234")))
            {
                host.Start();
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
            }
        }
    }
}
