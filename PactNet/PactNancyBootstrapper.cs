using System.Collections.Generic;
using Nancy;
using Nancy.Bootstrapper;

namespace PactNet
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
    }
}