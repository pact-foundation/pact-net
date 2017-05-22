using Nancy;
using Nancy.Bootstrapper;
using Nancy.Diagnostics;
using Nancy.TinyIoc;
using PactNet.Infrastructure;
using PactNet.Logging;
using PactNet.Mocks.MockHttpService.Comparers;
using PactNet.Mocks.MockHttpService.Mappers;
using System;
using System.Collections.Generic;

namespace PactNet.Mocks.MockHttpService.Nancy
{
    public class MockProviderNancyBootstrapper : DefaultNancyBootstrapper
    {
        private readonly PactConfig _config;

        public MockProviderNancyBootstrapper(PactConfig config)
        {
            _config = config;
        }

        protected override IEnumerable<ModuleRegistration> Modules => new List<ModuleRegistration>();

#if USE_NANCY_HOST
        protected override NancyInternalConfiguration InternalConfiguration
#elif USE_KESTREL_HOST
        protected override Func<ITypeCatalog, NancyInternalConfiguration> InternalConfiguration
#endif
        {
            get
            {
                return NancyInternalConfiguration.WithOverrides(c =>
                {
                    c.RequestDispatcher = typeof(MockProviderNancyRequestDispatcher);
                    c.StatusCodeHandlers.Clear();
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
            container.Register(typeof(PactConfig), (c, o) => _config);
            container.Register<IProviderServiceRequestMapper, ProviderServiceRequestMapper>().AsMultiInstance();
            container.Register<IProviderServiceRequestComparer, ProviderServiceRequestComparer>().AsMultiInstance();
            container.Register<INancyResponseMapper, NancyResponseMapper>().AsMultiInstance();
            container.Register<IMockProviderRequestHandler, MockProviderRequestHandler>().AsMultiInstance();
            container.Register<IMockProviderAdminRequestHandler, MockProviderAdminRequestHandler>().AsMultiInstance();
            container.Register<IMockProviderRepository, MockProviderRepository>().AsSingleton();
            container.Register<IFileSystem, FileSystem>().AsMultiInstance();
            container.Register(typeof(ILog), (c, o) => LogProvider.GetLogger(_config.LoggerName));
        }
    }
}