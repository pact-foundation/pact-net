using System;
using System.Collections.Generic;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Diagnostics;
using Nancy.TinyIoc;
using PactNet.Mocks.MockHttpService.Comparers;
using PactNet.Mocks.MockHttpService.Mappers;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService
{
    public class MockProviderNancyBootstrapper : DefaultNancyBootstrapper
    {
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
                   // c.ContextFactory = typeof (PactAwareContextFactory);
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
            container.Register(typeof(IPactProviderServiceRequestComparer), typeof(PactProviderServiceRequestComparer));
            container.Register(typeof(IPactProviderServiceRequestMapper), typeof(PactProviderServiceRequestMapper));
            container.Register(typeof(INancyResponseMapper), typeof(NancyResponseMapper));
            
        }
    }

    public class PactAwareContextFactory : INancyContextFactory
    {
        public NancyContext Create(Request request)
        {
            return new NancyContext()
            {
                Request = request,
            };
        }
    }
}