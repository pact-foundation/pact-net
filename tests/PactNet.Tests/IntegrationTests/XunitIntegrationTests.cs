using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using PactNet.Matchers;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Models;
using Xunit;

namespace PactNet.Tests.IntegrationTests
{
    public class XunitIntegrationTests : PactTests<CalculatorApiPact>
    {
        public XunitIntegrationTests(CalculatorApiPact pact)
            : base(pact)
        {
        }

        [Fact]
        public async Task Add_Numbers()
        {
            // Arrange
            var request = new ProviderServiceRequest()
            {
                Method = HttpVerb.Post,
                Path = "/calculator/add",
                Headers = new Dictionary<string, object>()
                {
                    ["Content-Type"] = "application/json; charset=utf-8",
                },
                Body = new
                {
                    left = 1,
                    right = 1,
                },
            };

            var response = new ProviderServiceResponse()
            {
                Status = 200,
                Headers = new Dictionary<string, object>()
                {
                    ["Content-Type"] = "application/json",
                },
                Body = new
                {
                    result = Match.Type(2),
                },
            };

            Pact.PactProvider
                .Given("I want to add two integers numbers")
                .UponReceiving("An HTTP POST request to add two numbers")
                .With(request)
                .WillRespondWith(response);

            // Act
            using (var httpClient = CreateHttpClient())
            {
                using (var httpContent = new StringContent(@"{""left"":1,""right"":1}", Encoding.UTF8, "application/json"))
                {
                    using (var httpResponse = await httpClient.PostAsync("/calculator/add", httpContent))
                    {
                        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);

                        string json = await httpResponse.Content.ReadAsStringAsync();

                        Assert.Equal(@"{""result"":2}", json);
                    }
                }
            }

            // Assert
            Pact.PactProvider.VerifyInteractions();
        }

        [Fact]
        public async Task Subtract_Numbers()
        {
            // Arrange
            var request = new ProviderServiceRequest()
            {
                Method = HttpVerb.Post,
                Path = "/calculator/subtract",
                Headers = new Dictionary<string, object>()
                {
                    ["Content-Type"] = "application/json; charset=utf-8",
                },
                Body = new
                {
                    left = 2,
                    right = 3,
                },
            };

            var response = new ProviderServiceResponse()
            {
                Status = 200,
                Headers = new Dictionary<string, object>()
                {
                    ["Content-Type"] = "application/json",
                },
                Body = new
                {
                    result = Match.Type(-1),
                },
            };

            Pact.PactProvider
                .Given("I want to subtract two integers numbers")
                .UponReceiving("An HTTP POST request to subtract two numbers")
                .With(request)
                .WillRespondWith(response);

            // Act
            using (var httpClient = CreateHttpClient())
            {
                using (var httpContent = new StringContent(@"{""left"":2,""right"":3}", Encoding.UTF8, "application/json"))
                {
                    using (var httpResponse = await httpClient.PostAsync("/calculator/subtract", httpContent))
                    {
                        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);

                        string json = await httpResponse.Content.ReadAsStringAsync();

                        Assert.Equal(@"{""result"":-1}", json);
                    }
                }
            }

            // Assert
            Pact.PactProvider.VerifyInteractions();
        }
    }

    public abstract class PactTests<T> : IClassFixture<T>
        where T : ApiPact
    {
        protected PactTests(T pact)
        {
            Pact = pact;
        }

        protected T Pact { get; }

        public HttpClient CreateHttpClient()
        {
            return new HttpClient()
            {
                BaseAddress = Pact.PactBrokerUri,
            };
        }
    }

    public sealed class CalculatorApiPact : ApiPact
    {
        public CalculatorApiPact()
            : base("Calculator")
        {
        }
    }

    public abstract class ApiPact : IAsyncLifetime, IDisposable
    {
        private bool _disposed;
        private IMockProviderService _provider;

        protected ApiPact(string providerName)
        {
            PactBuilder = new PactBuilder(
                new PactConfig()
                {
                    LogDir = "logs",
                    PactDir = "pacts",
                    SpecificationVersion = "2.0.0",
                });

            PactBuilder
                .ServiceConsumer("Calculator")
                .HasPactWith(providerName);

            PactBrokerUri = new UriBuilder()
            {
                Host = "localhost",
                Port = 4322,
                Scheme = "http",
            }.Uri;
        }

        ~ApiPact()
        {
            Dispose(false);
        }

        public Uri PactBrokerUri { get; }

        public IMockProviderService PactProvider
        {
            get
            {
                if (_provider == null)
                {
                    _provider = PactBuilder.MockService(PactBrokerUri.Port);
                }

                return _provider;
            }
        }

        private IPactBuilder PactBuilder { get; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public Task DisposeAsync()
        {
            Dispose();
            return Task.CompletedTask;
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_provider != null)
                    {
                        PactBuilder?.Build();
                    }
                }

                _disposed = true;
            }
        }
    }
}
