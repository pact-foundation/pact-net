using System.Collections.Generic;
using System.IO.Abstractions;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Diagnostics;
using Nancy.TinyIoc;
using PactNet.Logging;
using PactNet.Mocks.MockHttpService.Comparers;
using PactNet.Mocks.MockHttpService.Mappers;
using PactNet.Models;

namespace PactNet.Mocks.MockHttpService.Nancy
{
    public class MockProviderNancyBootstrapper : DefaultNancyBootstrapper
    {
        private readonly string _pactFileDirectory;

        public MockProviderNancyBootstrapper(string pactFileDirectory)
        {
            _pactFileDirectory = pactFileDirectory;
        }

        protected override IEnumerable<ModuleRegistration> Modules
        {
            get { return new List<ModuleRegistration>(); }
        }

        protected override NancyInternalConfiguration InternalConfiguration
        {
            get
            {
                return NancyInternalConfiguration.WithOverrides(c =>
                {
                    c.RequestDispatcher = typeof(MockProviderNancyRequestDispatcher);
                });
            }
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            RegisterDependenciesWithNancyContainer(container);

            DiagnosticsHook.Disable(pipelines);
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
        }

        private void RegisterDependenciesWithNancyContainer(TinyIoCContainer container)
        {
            container.Register(typeof(PactFileInfo), (c, o) => new PactFileInfo(_pactFileDirectory));
            container.Register<IProviderServiceRequestMapper, ProviderServiceRequestMapper>().AsMultiInstance();
            container.Register<IProviderServiceRequestComparer, ProviderServiceRequestComparer>().AsMultiInstance();
            container.Register<INancyResponseMapper, NancyResponseMapper>().AsMultiInstance();
            container.Register<IMockProviderRequestHandler, MockProviderRequestHandler>().AsMultiInstance();
            container.Register<IMockProviderAdminRequestHandler, MockProviderAdminRequestHandler>().AsMultiInstance();
            container.Register<IMockProviderRepository, MockProviderRepository>().AsSingleton();
            container.Register<IFileSystem, FileSystem>().AsMultiInstance();
            container.Register(typeof(ILog), (c, o) => LogProvider.GetLogger(typeof(MockProviderNancyRequestDispatcher)));
        }
    }
}