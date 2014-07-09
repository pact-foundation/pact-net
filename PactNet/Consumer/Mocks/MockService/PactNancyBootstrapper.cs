using System.Collections.Generic;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Diagnostics;
using Nancy.TinyIoc;
using PactNet.Comparers;

namespace PactNet.Consumer.Mocks.MockService
{
    public class PactNancyBootstrapper : DefaultNancyBootstrapper
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
                    c.RequestDispatcher = typeof(PactNancyRequestDispatcher);
                });
            }
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            container.Register(typeof (IPactProviderServiceRequestComparer), typeof (PactProviderServiceRequestComparer));

            DiagnosticsHook.Disable(pipelines);
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
        }
    }
}