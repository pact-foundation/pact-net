using System.IO.Abstractions;
using NSubstitute;
using PactNet.Mocks.MockHttpService.Nancy;

namespace PactNet.Tests.IntegrationTests
{
    public class IntegrationTestingMockProviderNancyBootstrapper : MockProviderNancyBootstrapper
    {
        protected override void ApplicationStartup(Nancy.TinyIoc.TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            container.Register(typeof(IFileSystem), Substitute.For<IFileSystem>());
        }
    }
}