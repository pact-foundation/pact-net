using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Net.Http;
using NSubstitute;
using PactNet.Mocks.MockHttpService.Models;
using PactNet.Mocks.MockHttpService.Validators;
using PactNet.Models;
using Xunit;

namespace PactNet.Tests
{
    public class PactVerifierTests
    {
        public IPactVerifier GetSubject()
        {
            return new PactVerifier();
        }

        [Fact]
        public void ProviderStates_WhenProviderStatesForHasNotBeenCalled_ReturnsNull()
        {
            var pactVerifier = (PactVerifier)GetSubject();

            var providerStates = pactVerifier.ProviderStates;

            Assert.Null(providerStates);
        }

        [Fact]
        public void ProviderStatesFor_WhenCalledWithNullConsumerName_ThrowsArgumentException()
        {
            var pactVerifier = GetSubject();

            Assert.Throws<ArgumentException>(() => pactVerifier.ProviderStatesFor(null));
        }

        [Fact]
        public void ProviderStatesFor_WhenCalledWithEmptyConsumerName_ThrowsArgumentException()
        {
            var pactVerifier = GetSubject();

            Assert.Throws<ArgumentException>(() => pactVerifier.ProviderStatesFor(String.Empty));
        }

        [Fact]
        public void ProviderStatesFor_WhenCalledWithConsumerName_SetsConsumerName()
        {
            const string consumerName = "My Client";
            var pactVerifier = GetSubject();

            pactVerifier.ProviderStatesFor(consumerName);

            Assert.Equal(consumerName, ((PactVerifier)pactVerifier).ConsumerName);
        }

        [Fact]
        public void ProviderStatesFor_WhenConsumerNameHasBeenSetAndSupplyingADifferentConsumerName_ThrowsArgumentException()
        {
            var pactVerifier = GetSubject();

            pactVerifier.HonoursPactWith("My Client");

            Assert.Throws<ArgumentException>(() => pactVerifier.ProviderStatesFor("My Client 2"));
        }

        [Fact]
        public void ProviderState_WhenCalledBeforeProviderStatesFor_ThrowsInvalidOperationException()
        {
            const string providerState = "There is an event with id 1234 in the database";

            var pactVerifier = (PactVerifier)GetSubject();

            Assert.Throws<InvalidOperationException>(() => pactVerifier.ProviderState(providerState));
        }

        [Fact]
        public void ProviderState_WhenCalledWithSetUpAndTearDown_SetsProviderStateWithSetUpAndTearDownActions()
        {
            const string providerState = "There is an event with id 1234 in the database";
            Action providerStateSetUpAction = () => { };
            Action providerStateTearDownAction = () => { };

            var pactVerifier = (PactVerifier)GetSubject();

            pactVerifier.ProviderStatesFor("My Client")
                .ProviderState(providerState, providerStateSetUpAction, providerStateTearDownAction);

            var providerStateItem = pactVerifier.ProviderStates.Find(providerState);

            Assert.Equal(providerStateSetUpAction, providerStateItem.SetUp);
            Assert.Equal(providerStateTearDownAction, providerStateItem.TearDown);
        }

        [Fact]
        public void ProviderState_WhenCalledWithNullProviderState_ThrowsArgumentException()
        {
            var pactVerifier = GetSubject();

            Assert.Throws<ArgumentException>(() => 
                pactVerifier.ProviderStatesFor("My Client")
                .ProviderState(null));
        }

        [Fact]
        public void ProviderState_WhenCalledWithEmptyProviderState_ThrowsArgumentException()
        {
            var pactVerifier = GetSubject();

            Assert.Throws<ArgumentException>(() =>
                pactVerifier.ProviderStatesFor("My Client")
                .ProviderState(String.Empty));
        }

        [Fact]
        public void ServiceProvider_WhenCalledWithNullProviderName_ThrowsArgumentException()
        {
            var pactVerifier = GetSubject();

            Assert.Throws<ArgumentException>(() => pactVerifier.ServiceProvider(null, new HttpClient()));
        }

        [Fact]
        public void ServiceProvider_WhenCalledWithEmptyProviderName_ThrowsArgumentException()
        {
            var pactVerifier = GetSubject();

            Assert.Throws<ArgumentException>(() => pactVerifier.ServiceProvider(String.Empty, new HttpClient()));
        }

        [Fact]
        public void ServiceProvider_WhenCalledWithNullHttpClient_ThrowsArgumentException()
        {
            var pactVerifier = GetSubject();

            Assert.Throws<ArgumentException>(() => pactVerifier.ServiceProvider("Event API", null));
        }

        [Fact]
        public void ServiceProvider_WhenCalledWithProviderName_SetsProviderName()
        {
            const string providerName = "Event API";
            var pactVerifier = GetSubject();

            pactVerifier.ServiceProvider(providerName, new HttpClient());

            Assert.Equal(providerName, ((PactVerifier)pactVerifier).ProviderName);
        }

        [Fact]
        public void ServiceProvider_WhenCalledWithHttpClient_SetsHttpClient()
        {
            var httpClient = new HttpClient();
            var pactVerifier = GetSubject();

            pactVerifier.ServiceProvider("Event API", httpClient);

            Assert.Equal(httpClient, ((PactVerifier)pactVerifier).HttpClient);
        }

        [Fact]
        public void HonoursPactWith_WhenCalledWithNullConsumerName_ThrowsArgumentException()
        {
            var pactVerifier = GetSubject();

            Assert.Throws<ArgumentException>(() => pactVerifier.HonoursPactWith(null));
        }

        [Fact]
        public void HonoursPactWith_WhenCalledWithEmptyConsumerName_ThrowsArgumentException()
        {
            var pactVerifier = GetSubject();

            Assert.Throws<ArgumentException>(() => pactVerifier.HonoursPactWith(String.Empty));
        }

        [Fact]
        public void HonoursPactWith_WhenCalledWithConsumerName_SetsConsumerName()
        {
            const string consumerName = "My Client";
            var pactVerifier = GetSubject();

            pactVerifier.HonoursPactWith(consumerName);

            Assert.Equal(consumerName, ((PactVerifier)pactVerifier).ConsumerName);
        }

        [Fact]
        public void HonoursPactWith_WhenConsumerNameHasBeenSetAndSupplyingADifferentConsumerName_ThrowsArgumentException()
        {
            var pactVerifier = GetSubject();

            pactVerifier.ProviderStatesFor("My Client")
                .ProviderState("There is an event with id 1234 in the database");

            Assert.Throws<ArgumentException>(() => pactVerifier.HonoursPactWith("My Client 2"));
        }

        [Fact]
        public void PactUri_WhenCalledWithNullUri_ThrowsArgumentException()
        {
            var pactVerifier = GetSubject();

            Assert.Throws<ArgumentException>(() => pactVerifier.PactUri(null));
        }

        [Fact]
        public void PactUri_WhenCalledWithEmptyUri_ThrowsArgumentException()
        {
            var pactVerifier = GetSubject();

            Assert.Throws<ArgumentException>(() => pactVerifier.PactUri(String.Empty));
        }

        [Fact]
        public void PactUri_WhenCalledWithUri_SetsPactFileUri()
        {
            const string pactFileUri = "../../../Consumer.Tests/pacts/my_client-event_api.json";
            var pactVerifier = GetSubject();

            pactVerifier.PactUri(pactFileUri);

            Assert.Equal(pactFileUri, ((PactVerifier)pactVerifier).PactFileUri);
        }

        [Fact]
        public void Verify_WhenHttpClientIsNull_ThrowsInvalidOperationException()
        {
            var pactVerifier = GetSubject();
            pactVerifier.PactUri("../../../Consumer.Tests/pacts/my_client-event_api.json");

            Assert.Throws<InvalidOperationException>(() => pactVerifier.Verify());
        }

        [Fact]
        public void Verify_WhenPactFileUriIsNull_ThrowsInvalidOperationException()
        {
            var pactVerifier = GetSubject();
            pactVerifier.ServiceProvider("Event API", new HttpClient());

            Assert.Throws<InvalidOperationException>(() => pactVerifier.Verify());
        }

        [Fact]
        public void Verify_WhenFileDoesNotExistOnFileSystem_ThrowsInvalidOperationException()
        {
            var serviceProvider = "Event API";
            var serviceConsumer = "My client";
            var pactUri = "../../../Consumer.Tests/pacts/my_client-event_api.json";

            var mockFileSystem = Substitute.For<IFileSystem>();
            mockFileSystem.File.ReadAllText(pactUri).Returns(x => { throw new FileNotFoundException(); });

            var pactVerifier = new PactVerifier(mockFileSystem, null)
                .ServiceProvider(serviceProvider, new HttpClient())
                .HonoursPactWith(serviceConsumer)
                .PactUri(pactUri);

            Assert.Throws<InvalidOperationException>(() => pactVerifier.Verify());

            mockFileSystem.File.Received(1).ReadAllText(pactUri);
        }

        [Fact]
        public void Verify_WhenPactFileWithNoInteractionsExistOnFileSystem_CallsPactProviderValidator()
        {
            var serviceProvider = "Event API";
            var serviceConsumer = "My client";
            var pactUri = "../../../Consumer.Tests/pacts/my_client-event_api.json";
            var pactFileJson = "{ \"provider\": { \"name\": \"" + serviceProvider + "\" }, \"consumer\": { \"name\": \"" + serviceConsumer + "\" }, \"metadata\": { \"pactSpecificationVersion\": \"1.0.0\" } }";
            var httpClient = new HttpClient();

            var mockFileSystem = Substitute.For<IFileSystem>();
            var mockPactProviderServiceValidator = Substitute.For<IProviderServiceValidator>();
            mockFileSystem.File.ReadAllText(pactUri).Returns(pactFileJson);

            var pactVerifier = new PactVerifier(mockFileSystem, client => mockPactProviderServiceValidator)
                .ServiceProvider(serviceProvider, httpClient)
                .HonoursPactWith(serviceConsumer)
                .PactUri(pactUri);

            pactVerifier.Verify();

            mockFileSystem.File.Received(1).ReadAllText(pactUri);
            mockPactProviderServiceValidator.Received(1).Validate(Arg.Any<ProviderServicePactFile>(), Arg.Any<ProviderStates>());
        }

        [Fact]
        public void Verify_WithNoProviderDescriptionOrProviderStateSupplied_CallsProviderServiceValidatorWithAll3Interactions()
        {
            var pactUri = "../../../Consumer.Tests/pacts/my_client-event_api.json";
            var pactFileJson = "{ \"provider\": { \"name\": \"Event API\" }, \"consumer\": { \"name\": \"My client\" }, \"interactions\": [{ \"description\": \"My Description\", \"provider_state\": \"My Provider State\" }, { \"description\": \"My Description 2\", \"provider_state\": \"My Provider State\" }, { \"description\": \"My Description\", \"provider_state\": \"My Provider State 2\" }], \"metadata\": { \"pactSpecificationVersion\": \"1.0.0\" } }";
            var httpClient = new HttpClient();

            var mockFileSystem = Substitute.For<IFileSystem>();
            var mockPactProviderServiceValidator = Substitute.For<IProviderServiceValidator>();
            mockFileSystem.File.ReadAllText(pactUri).Returns(pactFileJson);

            var pactVerifier = new PactVerifier(mockFileSystem, client => mockPactProviderServiceValidator);

            pactVerifier.ProviderStatesFor("My client")
                .ProviderState("My Provider State")
                .ProviderState("My Provider State 2");

            pactVerifier.ServiceProvider("Event API", httpClient)
                .HonoursPactWith("My client")
                .PactUri(pactUri);

            pactVerifier.Verify();

            mockPactProviderServiceValidator.Received(1).Validate(
                Arg.Is<ProviderServicePactFile>(x => x.Interactions.Count() == 3),
                Arg.Any<ProviderStates>());
        }

        [Fact]
        public void Verify_WithProviderDescription_CallsProviderServiceValidatorWith2FilteredInteractions()
        {
            var description = "My Description";
            var pactUri = "../../../Consumer.Tests/pacts/my_client-event_api.json";
            var pactFileJson = "{ \"provider\": { \"name\": \"Event API\" }, \"consumer\": { \"name\": \"My client\" }, \"interactions\": [{ \"description\": \"My Description\", \"provider_state\": \"My Provider State\" }, { \"description\": \"My Description 2\", \"provider_state\": \"My Provider State\" }, { \"description\": \"My Description\", \"provider_state\": \"My Provider State 2\" }], \"metadata\": { \"pactSpecificationVersion\": \"1.0.0\" } }";
            var httpClient = new HttpClient();

            var mockFileSystem = Substitute.For<IFileSystem>();
            var mockPactProviderServiceValidator = Substitute.For<IProviderServiceValidator>();
            mockFileSystem.File.ReadAllText(pactUri).Returns(pactFileJson);

            var pactVerifier = new PactVerifier(mockFileSystem, client => mockPactProviderServiceValidator);

            pactVerifier.ProviderStatesFor("My client")
                .ProviderState("My Provider State")
                .ProviderState("My Provider State 2");

            pactVerifier.ServiceProvider("Event API", httpClient)
                .HonoursPactWith("My client")
                .PactUri(pactUri);

            pactVerifier.Verify(providerDescription: description);

            mockPactProviderServiceValidator.Received(1).Validate(
                Arg.Is<ProviderServicePactFile>(x => x.Interactions.Count() == 2 && x.Interactions.All(i => i.Description.Equals(description))),
                Arg.Any<ProviderStates>());
        }

        [Fact]
        public void Verify_WithProviderState_CallsProviderServiceValidatorWith2FilteredInteractions()
        {
            var providerState = "My Provider State";
            var pactUri = "../../../Consumer.Tests/pacts/my_client-event_api.json";
            var pactFileJson = "{ \"provider\": { \"name\": \"Event API\" }, \"consumer\": { \"name\": \"My client\" }, \"interactions\": [{ \"description\": \"My Description\", \"provider_state\": \"My Provider State\" }, { \"description\": \"My Description 2\", \"provider_state\": \"My Provider State\" }, { \"description\": \"My Description\", \"provider_state\": \"My Provider State 2\" }], \"metadata\": { \"pactSpecificationVersion\": \"1.0.0\" } }";
            var httpClient = new HttpClient();

            var mockFileSystem = Substitute.For<IFileSystem>();
            var mockPactProviderServiceValidator = Substitute.For<IProviderServiceValidator>();
            mockFileSystem.File.ReadAllText(pactUri).Returns(pactFileJson);

            var pactVerifier = new PactVerifier(mockFileSystem, client => mockPactProviderServiceValidator);

            pactVerifier.ProviderStatesFor("My client")
                .ProviderState("My Provider State")
                .ProviderState("My Provider State 2");

            pactVerifier.ServiceProvider("Event API", httpClient)
                .HonoursPactWith("My client")
                .PactUri(pactUri);

            pactVerifier.Verify(providerState: providerState);

            mockPactProviderServiceValidator.Received(1).Validate(
                Arg.Is<ProviderServicePactFile>(x => x.Interactions.Count() == 2 && x.Interactions.All(i => i.ProviderState.Equals(providerState))), 
                Arg.Any<ProviderStates>());
        }

        [Fact]
        public void Verify_WithDescriptionAndProviderState_CallsProviderServiceValidatorWith1FilteredInteractions()
        {
            var description = "My Description";
            var providerState = "My Provider State";
            var pactUri = "../../../Consumer.Tests/pacts/my_client-event_api.json";
            var pactFileJson = "{ \"provider\": { \"name\": \"Event API\" }, \"consumer\": { \"name\": \"My client\" }, \"interactions\": [{ \"description\": \"My Description\", \"provider_state\": \"My Provider State\" }, { \"description\": \"My Description 2\", \"provider_state\": \"My Provider State\" }, { \"description\": \"My Description\", \"provider_state\": \"My Provider State 2\" }], \"metadata\": { \"pactSpecificationVersion\": \"1.0.0\" } }";
            var httpClient = new HttpClient();

            var mockFileSystem = Substitute.For<IFileSystem>();
            var mockPactProviderServiceValidator = Substitute.For<IProviderServiceValidator>();
            mockFileSystem.File.ReadAllText(pactUri).Returns(pactFileJson);

            var pactVerifier = new PactVerifier(mockFileSystem, client => mockPactProviderServiceValidator);

            pactVerifier.ProviderStatesFor("My client")
                .ProviderState("My Provider State")
                .ProviderState("My Provider State 2");

            pactVerifier.ServiceProvider("Event API", httpClient)
                .HonoursPactWith("My client")
                .PactUri(pactUri);

            pactVerifier.Verify(providerDescription: description, providerState: providerState);

            mockPactProviderServiceValidator.Received(1).Validate(
                Arg.Is<ProviderServicePactFile>(x => x.Interactions.Count() == 1 && x.Interactions.All(i => i.ProviderState.Equals(providerState) && i.Description.Equals(description))), 
                Arg.Any<ProviderStates>());
        }

        [Fact]
        public void Verify_WithProviderStateSet_CallsProviderServiceValidatorWithProviderState()
        {
            var httpClient = new HttpClient();

            var mockFileSystem = Substitute.For<IFileSystem>();
            var mockPactProviderServiceValidator = Substitute.For<IProviderServiceValidator>();
            mockFileSystem.File.ReadAllText(Arg.Any<string>()).Returns("{}");

            var pactVerifier = new PactVerifier(mockFileSystem, client => mockPactProviderServiceValidator);

            pactVerifier.ProviderStatesFor("My client")
                .ProviderState("My Provider State")
                .ProviderState("My Provider State 2");

            pactVerifier.ServiceProvider("Event API", httpClient)
                .HonoursPactWith("My client")
                .PactUri("/test.json");

            pactVerifier.Verify();

            mockPactProviderServiceValidator.Received(1).Validate(
                Arg.Any<ProviderServicePactFile>(),
                pactVerifier.ProviderStates);
        }
    }
}
