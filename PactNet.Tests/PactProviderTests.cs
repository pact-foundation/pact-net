using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Net.Http;
using NSubstitute;
using PactNet.Mocks.MockHttpService.Models;
using PactNet.Mocks.MockHttpService.Validators;
using Xunit;

namespace PactNet.Tests
{
    public class PactProviderTests
    {
        public IPactProvider GetSubject()
        {
            return new Pact();
        }

        [Fact]
        public void ProviderStates_WhenProviderStatesForHasNotBeenCalled_ReturnsNull()
        {
            var pact = GetSubject();

            var providerStates = pact.ProviderStates;

            Assert.Null(providerStates);
        }

        [Fact]
        public void ProviderStatesFor_WhenCalledWithNullConsumerName_ThrowsArgumentException()
        {
            var pact = GetSubject();

            Assert.Throws<ArgumentException>(() => pact.ProviderStatesFor(
                null, 
                new Dictionary<string, Action>
                {
                    { "There is an event with id 1234 in the database", () => { } }
                }));
        }

        [Fact]
        public void ProviderStatesFor_WhenCalledWithEmptyConsumerName_ThrowsArgumentException()
        {
            var pact = GetSubject();

            Assert.Throws<ArgumentException>(() => pact.ProviderStatesFor(
                String.Empty,
                new Dictionary<string, Action>
                {
                    { "There is an event with id 1234 in the database", () => { } }
                }));
        }

        [Fact]
        public void ProviderStatesFor_WhenCalledWithNullProviderStates_ThrowsArgumentException()
        {
            var pact = GetSubject();

            Assert.Throws<ArgumentException>(() => pact.ProviderStatesFor("My Client", null));
        }

        [Fact]
        public void ProviderStatesFor_WhenCalledWithEmptyProviderStates_ThrowsArgumentException()
        {
            var pact = GetSubject();

            Assert.Throws<ArgumentException>(() => pact.ProviderStatesFor("My Client", new Dictionary<string, Action>()));
        }

        [Fact]
        public void ProviderStatesFor_WhenCalledWithConsumerName_SetsConsumerName()
        {
            const string consumerName = "My Client";
            var pact = GetSubject();

            pact.ProviderStatesFor(
                consumerName,
                new Dictionary<string, Action>
                {
                    { "There is an event with id 1234 in the database", () => { } }
                });

            Assert.Equal(consumerName, pact.ConsumerName);
        }

        [Fact]
        public void ProviderStatesFor_WhenCalledWithProviderStates_SetsProviderStates()
        {
            var providerStates = new Dictionary<string, Action>
            {
                {"There is an event with id 1234 in the database", () => { }}
            };

            var pact = GetSubject();

            pact.ProviderStatesFor("My Client", providerStates);

            Assert.Equal(providerStates, pact.ProviderStates);
        }

        [Fact]
        public void ProviderStatesFor_WhenConsumerNameHasBeenSetAndSupplyingADifferentConsumerName_ThrowsArgumentException()
        {
            var pact = GetSubject();

            pact.HonoursPactWith("My Client");

            Assert.Throws<ArgumentException>(() => pact.ProviderStatesFor(
                "My Client 2",
                new Dictionary<string, Action>
                {
                    { "There is an event with id 1234 in the database", () => { } }
                }));
        }

        [Fact]
        public void ServiceProvider_WhenCalledWithNullProviderName_ThrowsArgumentException()
        {
            var pact = GetSubject();

            Assert.Throws<ArgumentException>(() => pact.ServiceProvider(null, new HttpClient()));
        }

        [Fact]
        public void ServiceProvider_WhenCalledWithEmptyProviderName_ThrowsArgumentException()
        {
            var pact = GetSubject();

            Assert.Throws<ArgumentException>(() => pact.ServiceProvider(String.Empty, new HttpClient()));
        }

        [Fact]
        public void ServiceProvider_WhenCalledWithNullHttpClient_ThrowsArgumentException()
        {
            var pact = GetSubject();

            Assert.Throws<ArgumentException>(() => pact.ServiceProvider("Event API", null));
        }

        [Fact]
        public void ServiceProvider_WhenCalledWithProviderName_SetsProviderName()
        {
            const string providerName = "Event API";
            var pact = GetSubject();

            pact.ServiceProvider(providerName, new HttpClient());

            Assert.Equal(providerName, pact.ProviderName);
        }

        [Fact]
        public void ServiceProvider_WhenCalledWithHttpClient_SetsHttpClient()
        {
            var httpClient = new HttpClient();
            var pact = GetSubject();

            pact.ServiceProvider("Event API", httpClient);

            Assert.Equal(httpClient, pact.HttpClient);
        }

        [Fact]
        public void HonoursPactWith_WhenCalledWithNullConsumerName_ThrowsArgumentException()
        {
            var pact = GetSubject();

            Assert.Throws<ArgumentException>(() => pact.HonoursPactWith(null));
        }

        [Fact]
        public void HonoursPactWith_WhenCalledWithEmptyConsumerName_ThrowsArgumentException()
        {
            var pact = GetSubject();

            Assert.Throws<ArgumentException>(() => pact.HonoursPactWith(String.Empty));
        }

        [Fact]
        public void HonoursPactWith_WhenCalledWithConsumerName_SetsConsumerName()
        {
            const string consumerName = "My Client";
            var pact = GetSubject();

            pact.HonoursPactWith(consumerName);

            Assert.Equal(consumerName, pact.ConsumerName);
        }

        [Fact]
        public void HonoursPactWith_WhenConsumerNameHasBeenSetAndSupplyingADifferentConsumerName_ThrowsArgumentException()
        {
            var pact = GetSubject();

            pact.ProviderStatesFor(
                "My Client",
                new Dictionary<string, Action>
                {
                    {"There is an event with id 1234 in the database", () => { }}
                });

            Assert.Throws<ArgumentException>(() => pact.HonoursPactWith("My Client 2"));
        }

        [Fact]
        public void PactUri_WhenCalledWithNullUri_ThrowsArgumentException()
        {
            var pact = GetSubject();

            Assert.Throws<ArgumentException>(() => pact.PactUri(null));
        }

        [Fact]
        public void PactUri_WhenCalledWithEmptyUri_ThrowsArgumentException()
        {
            var pact = GetSubject();

            Assert.Throws<ArgumentException>(() => pact.PactUri(String.Empty));
        }

        [Fact]
        public void PactUri_WhenCalledWithUri_SetsPactFileUri()
        {
            const string pactFileUri = "../../../Consumer.Tests/pacts/my_client-event_api.json";
            var pact = GetSubject();

            pact.PactUri(pactFileUri);

            Assert.Equal(pactFileUri, pact.PactFileUri);
        }

        [Fact]
        public void VerifyProviderService_WhenHttpClientIsNull_ThrowsInvalidOperationException()
        {
            var pact = GetSubject();
            pact.PactUri("../../../Consumer.Tests/pacts/my_client-event_api.json");

            Assert.Throws<InvalidOperationException>(() => pact.VerifyProviderService());
        }

        [Fact]
        public void VerifyProviderService_WhenPactFileUriIsNull_ThrowsInvalidOperationException()
        {
            var pact = GetSubject();
            pact.ServiceProvider("Event API", new HttpClient());

            Assert.Throws<InvalidOperationException>(() => pact.VerifyProviderService());
        }

        [Fact]
        public void VerifyProviderService_WhenFileDoesNotExistOnFileSystem_ThrowsPactAssertException()
        {
            var serviceProvider = "Event API";
            var serviceConsumer = "My client";
            var pactUri = "../../../Consumer.Tests/pacts/my_client-event_api.json";

            var mockFileSystem = Substitute.For<IFileSystem>();
            mockFileSystem.File.ReadAllText(pactUri).Returns(x => { throw new FileNotFoundException(); });

            var pact = new Pact(null, mockFileSystem, null)
                .ServiceProvider(serviceProvider, new HttpClient())
                .HonoursPactWith(serviceConsumer)
                .PactUri(pactUri);

            Assert.Throws<ComparisonFailedException>(() => pact.VerifyProviderService());

            mockFileSystem.File.Received(1).ReadAllText(pactUri);
        }

        [Fact]
        public void VerifyProviderService_WhenPactFileWithNoInteractionsExistOnFileSystem_CallsPactProviderValidator()
        {
            var serviceProvider = "Event API";
            var serviceConsumer = "My client";
            var pactUri = "../../../Consumer.Tests/pacts/my_client-event_api.json";
            var pactFileJson = "{ \"provider\": { \"name\": \"" + serviceProvider + "\" }, \"consumer\": { \"name\": \"" + serviceConsumer + "\" }, \"metadata\": { \"pactSpecificationVersion\": \"1.0.0\" } }";
            var httpClient = new HttpClient();

            var mockFileSystem = Substitute.For<IFileSystem>();
            var mockPactProviderServiceValidator = Substitute.For<IProviderServiceValidator>();
            mockFileSystem.File.ReadAllText(pactUri).Returns(pactFileJson);

            var pact = new Pact(null, mockFileSystem, client => mockPactProviderServiceValidator)
                .ServiceProvider(serviceProvider, httpClient)
                .HonoursPactWith(serviceConsumer)
                .PactUri(pactUri);

            pact.VerifyProviderService();

            mockFileSystem.File.Received(1).ReadAllText(pactUri);
            mockPactProviderServiceValidator.Received(1).Validate(Arg.Any<ServicePactFile>());
        }

        [Fact]
        public void VerifyProviderService_WhenPactFileWithAnInteractionThatHasAProviderState_ProviderStateIsInvoked()
        {
            var serviceProvider = "Event API";
            var serviceConsumer = "My client";
            var actionInvoked = false;
            var providerStates = new Dictionary<string, Action>
            {
                { "My Provider State", () => { actionInvoked = true; } }
            };
            var pactUri = "../../../Consumer.Tests/pacts/my_client-event_api.json";
            var pactFileJson = "{ \"provider\": { \"name\": \"" + serviceProvider + "\" }, \"consumer\": { \"name\": \"" + serviceConsumer + "\" }, \"interactions\": [{ \"description\": \"My Description\", \"providerState\": \"My Provider State\" }], \"metadata\": { \"pactSpecificationVersion\": \"1.0.0\" } }";
            var httpClient = new HttpClient();

            var mockFileSystem = Substitute.For<IFileSystem>();
            var mockPactProviderServiceValidator = Substitute.For<IProviderServiceValidator>();
            mockFileSystem.File.ReadAllText(pactUri).Returns(pactFileJson);

            var pact = new Pact(null, mockFileSystem, client => mockPactProviderServiceValidator)
                .ProviderStatesFor(serviceConsumer, providerStates)
                .ServiceProvider(serviceProvider, httpClient)
                .HonoursPactWith(serviceConsumer)
                .PactUri(pactUri);

            pact.VerifyProviderService();

            mockFileSystem.File.Received(1).ReadAllText(pactUri);
            mockPactProviderServiceValidator.Received(1).Validate(Arg.Any<ServicePactFile>());
            Assert.True(actionInvoked);
        }

        [Fact]
        public void VerifyProviderService_WhenPactFileWithAnInteractionThatHasAProviderStateButNoProviderStateIsSupplied_ThrowsInvalidOperationException()
        {
            var serviceProvider = "Event API";
            var serviceConsumer = "My client";
            var pactUri = "../../../Consumer.Tests/pacts/my_client-event_api.json";
            var pactFileJson = "{ \"provider\": { \"name\": \"" + serviceProvider + "\" }, \"consumer\": { \"name\": \"" + serviceConsumer + "\" }, \"interactions\": [{ \"description\": \"My Description\", \"providerState\": \"My Provider State\" }], \"metadata\": { \"pactSpecificationVersion\": \"1.0.0\" } }";
            var httpClient = new HttpClient();

            var mockFileSystem = Substitute.For<IFileSystem>();
            var mockPactProviderServiceValidator = Substitute.For<IProviderServiceValidator>();
            mockFileSystem.File.ReadAllText(pactUri).Returns(pactFileJson);

            var pact = new Pact(null, mockFileSystem, client => mockPactProviderServiceValidator)
                .ServiceProvider(serviceProvider, httpClient)
                .HonoursPactWith(serviceConsumer)
                .PactUri(pactUri);

            Assert.Throws<InvalidOperationException>(() => pact.VerifyProviderService());

            mockFileSystem.File.Received(1).ReadAllText(pactUri);
        }
    }
}
