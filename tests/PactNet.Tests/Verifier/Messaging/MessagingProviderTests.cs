using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PactNet.Output.Xunit;
using PactNet.Verifier;
using PactNet.Verifier.Messaging;
using Xunit;
using Xunit.Abstractions;

namespace PactNet.Tests.Verifier.Messaging
{
    public class MessagingProviderTests : IDisposable
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore
        };

        private readonly MessagingProvider provider;
        private readonly HttpClient client;

        private readonly Mock<IMessageScenarios> mockScenarios;

        public MessagingProviderTests(ITestOutputHelper output)
        {
            this.mockScenarios = new Mock<IMessageScenarios>();

            this.provider = new MessagingProvider(new PactVerifierConfig
            {
                Outputters = new[]
                {
                    new XunitOutput(output)
                },
                LogLevel = PactLogLevel.Debug
            }, this.mockScenarios.Object);

            Uri uri = this.provider.Start(Settings);
            this.client = new HttpClient { BaseAddress = uri };
        }

        public void Dispose()
        {
            this.provider.Dispose();
        }

        [Fact]
        public async Task HandleMessage_InvalidHttpMethod_ReturnsMethodNotAllowed()
        {
            HttpResponseMessage response = await this.client.DeleteAsync(string.Empty);

            response.StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);
        }

        [Fact]
        public async Task HandleMessage_InvalidInteractionBody_ReturnsError()
        {
            StringContent content = new StringContent("foo");

            HttpResponseMessage response = await this.client.PostAsync(string.Empty, content);

            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async Task HandleMessage_NoDescription_ReturnsBadRequest()
        {
            StringContent content = new StringContent("{}");

            HttpResponseMessage response = await this.client.PostAsync(string.Empty, content);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task HandleMessage_UnknownDescription_ReturnsNotFound()
        {
            StringContent content = new StringContent(@"{""description"":""a message""}");
            this.mockScenarios
                .Setup(s => s.Scenarios)
                .Returns(new ReadOnlyDictionary<string, Scenario>(new Dictionary<string, Scenario>()));

            HttpResponseMessage response = await this.client.PostAsync(string.Empty, content);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task HandleMessage_ValidInteraction_ReturnsMetadataHeader()
        {
            StringContent content = new StringContent(@"{""description"":""a message""}");
            Func<dynamic> factory = () => new { Foo = "bar" };
            dynamic metadata = new { Key = 42 };

            this.mockScenarios
                .Setup(s => s.Scenarios)
                .Returns(new ReadOnlyDictionary<string, Scenario>(new Dictionary<string, Scenario>
                 {
                     ["a message"] = new Scenario("a message",
                                                  factory,
                                                  metadata,
                                                  new JsonSerializerSettings
                                                  {
                                                      ContractResolver = new CamelCasePropertyNamesContractResolver()
                                                  })
                 }));

            HttpResponseMessage response = await this.client.PostAsync(string.Empty, content);

            var headers = response.Headers.GetValues("Pact-Message-Metadata");

            headers.Should().Equal("eyJrZXkiOjQyfQ=="); // this is {"key":42} in base64
        }

        [Fact]
        public async Task HandleMessage_ValidInteraction_ReturnsResponseBody()
        {
            StringContent content = new StringContent(@"{""description"":""a message""}");
            Func<dynamic> factory = () => new { Foo = "bar", Baz = 42 };

            this.mockScenarios
                .Setup(s => s.Scenarios)
                .Returns(new ReadOnlyDictionary<string, Scenario>(new Dictionary<string, Scenario>
                 {
                     ["a message"] = new Scenario("a message",
                                                  factory,
                                                  null,
                                                  new JsonSerializerSettings
                                                  {
                                                      ContractResolver = new CamelCasePropertyNamesContractResolver()
                                                  })
                 }));

            HttpResponseMessage response = await this.client.PostAsync(string.Empty, content);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            string body = await response.Content.ReadAsStringAsync();
            body.Should().Be(@"{""foo"":""bar"",""baz"":42}");
        }
    }
}
