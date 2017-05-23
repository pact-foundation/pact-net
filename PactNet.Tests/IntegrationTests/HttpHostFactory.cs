using PactNet.Mocks.MockHttpService;
using System;

#if USE_KESTREL

using NSubstitute;
using PactNet.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

#endif

namespace PactNet.Tests.IntegrationTests
{
    public static class HttpHostFactory
    {
        internal static HttpHost Create(Uri baseUri, PactConfig pactConfig)
        {
#if USE_NANCY
            return new HttpHost(baseUri, "MyApi", pactConfig, new IntegrationTestingMockProviderNancyBootstrapper(pactConfig));
#endif
#if USE_KESTREL
            return new HttpHost(baseUri, "MyApi", pactConfig, services => services.AddSingleton(Substitute.For<IFileSystem>()));
#endif
        }
    }
}