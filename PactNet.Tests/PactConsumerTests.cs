using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using NSubstitute;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Models;
using Xunit;

namespace PactNet.Tests
{
    public class PactConsumerTests
    {
        public IPactConsumer GetSubject()
        {
            return new Pact();
        }

        [Fact]
        public void ServiceConsumer_WithConsumerName_SetsConsumerName()
        {
            const string consumerName = "My Service Consumer";
            var pact = GetSubject();

            pact.ServiceConsumer(consumerName);

            Assert.Equal(consumerName, pact.ConsumerName);
        }

        [Fact]
        public void ServiceConsumer_WithNullConsumerName_ThrowsArgumentException()
        {
            var pact = GetSubject();

            Assert.Throws<ArgumentException>(() => pact.ServiceConsumer(null));
        }

        [Fact]
        public void ServiceConsumer_WithEmptyConsumerName_ThrowsArgumentException()
        {
            var pact = GetSubject();

            Assert.Throws<ArgumentException>(() => pact.ServiceConsumer(String.Empty));
        }

        [Fact]
        public void HasPactWith_WithProviderName_SetsProviderName()
        {
            const string providerName = "My Service Provider";
            var pact = GetSubject();

            pact.HasPactWith(providerName);

            Assert.Equal(providerName, pact.ProviderName);
        }

        [Fact]
        public void HasPactWith_WithNullProviderName_ThrowsArgumentException()
        {
            var pact = GetSubject();

            Assert.Throws<ArgumentException>(() => pact.HasPactWith(null));
        }

        [Fact]
        public void HasPactWith_WithEmptyProviderName_ThrowsArgumentException()
        {
            var pact = GetSubject();

            Assert.Throws<ArgumentException>(() => pact.HasPactWith(String.Empty));
        }

        [Fact]
        public void PactFileUri_WhenCalledBeforeConsumerAndProviderNamesHaveBeenSet_ReturnsFileSystemPathWithNoConsumerAndProviderNameAndDoesNotThrow()
        {
            var pact = GetSubject();
            var uri = pact.PactFileUri;

            Assert.Equal("../../pacts/-.json", uri);
        }

        [Fact]
        public void PactFileUri_WhenConsumerAndProviderNamesHaveBeenSet_ReturnsFileSystemPathWithCorrectNamesLowercaseAndWithSpacedReplacedWithUnderscores()
        {
            var pact = GetSubject()
                .ServiceConsumer("My Client")
                .HasPactWith("My Service");

            var uri = pact.PactFileUri;

            Assert.Equal("../../pacts/my_client-my_service.json", uri);
        }

        [Fact]
        public void MockService_WhenCalled_StartIsCalledAndMockProviderServiceIsReturned()
        {
            var mockMockProviderService = Substitute.For<IMockProviderService>();

            IPactConsumer pact = new Pact(port => mockMockProviderService, null);

            var mockProviderService = pact.MockService(1234);

            mockMockProviderService.Received(1).Start();
            Assert.Equal(mockMockProviderService, mockProviderService);
        }

        [Fact]
        public void MockService_WhenCalledTwice_StopIsCalledTheSecondTime()
        {
            var mockMockProviderService = Substitute.For<IMockProviderService>();

            IPactConsumer pact = new Pact(port => mockMockProviderService, null);

            pact.MockService(1234);
            mockMockProviderService.Received(0).Stop();

            pact.MockService(1234);
            mockMockProviderService.Received(1).Stop();
        }

        [Fact]
        public void Dispose_WhenCalledWithoutConsumerNameSet_ThrowsInvalidOperationException()
        {
            IPactConsumer pact = new Pact(null, null)
                .HasPactWith("Event API");

            Assert.Throws<InvalidOperationException>(() => pact.Dispose());
        }

        [Fact]
        public void Dispose_WhenCalledWithoutProviderNameSet_ThrowsInvalidOperationException()
        {
            IPactConsumer pact = new Pact(null, null)
                .ServiceConsumer("Event Client");

            Assert.Throws<InvalidOperationException>(() => pact.Dispose());
        }

        [Fact]
        public void Dispose_WhenCalledWithNoMockProviderService_NewPactFileIsSavedWithNoInteractions()
        {
            var mockFileSystem = Substitute.For<IFileSystem>();

            IPactConsumer pact = new Pact(null, mockFileSystem)
                .ServiceConsumer("Event Client")
                .HasPactWith("Event API");

            var pactFilePath = pact.PactFileUri;

            pact.Dispose();

            mockFileSystem.File.Received(1).WriteAllText(pactFilePath, Arg.Any<string>());
        }

        [Fact]
        public void Dispose_WhenCalledWithAnInteractionOnTheMockProviderService_NewPactFileIsSavedWithTheInteraction()
        {
            var mockMockProviderService = Substitute.For<IMockProviderService>();
            var mockFileSystem = Substitute.For<IFileSystem>();

            IPactConsumer pact = new Pact(port => mockMockProviderService, mockFileSystem)
                .ServiceConsumer("Event Client")
                .HasPactWith("Event API");

            pact.MockService(1234);

            var pactFilePath = pact.PactFileUri;

            mockFileSystem.File.ReadAllText(pactFilePath).Returns(x => { throw new System.IO.FileNotFoundException(); });
            mockMockProviderService.Interactions.Returns(new List<PactServiceInteraction>
            {
                new PactServiceInteraction
                {
                    Description = "My interaction",
                    Request = new PactProviderServiceRequest(),
                    Response = new PactProviderServiceResponse()
                }
            });

            pact.Dispose();

            var pactInteractions = mockMockProviderService.Received(1).Interactions;
            mockFileSystem.File.Received(1).WriteAllText(pactFilePath, Arg.Any<string>());
        }

        [Fact]
        public void Dispose_WhenCalledAndDirectoryDoesNotExist_DirectoryIsCreatedThenFileIsCreated()
        {
            var mockFileSystem = Substitute.For<IFileSystem>();

            IPactConsumer pact = new Pact(null, mockFileSystem)
                .ServiceConsumer("Event Client")
                .HasPactWith("Event API");
            var pactFilePath = pact.PactFileUri;

            var callCount = 0;
            mockFileSystem.File
                .When(x => x.WriteAllText(pactFilePath, Arg.Any<string>()))
                .Do(x =>
                {
                    if (callCount++ < 1)
                    {
                        throw new System.IO.DirectoryNotFoundException();
                    }
                });

            pact.Dispose();

            mockFileSystem.File.Received(2).WriteAllText(pactFilePath, Arg.Any<string>());
            mockFileSystem.Directory.Received(1).CreateDirectory(Arg.Any<string>());
        }

        [Fact]
        public void Dispose_WhenCalledWithAnInitialisedMockProviderService_StopIsCallOnTheMockServiceProvider()
        {
            var mockMockProviderService = Substitute.For<IMockProviderService>();
            var mockFileSystem = Substitute.For<IFileSystem>();

            IPactConsumer pact = new Pact(port => mockMockProviderService, mockFileSystem)
                .ServiceConsumer("Event Client")
                .HasPactWith("Event API");
            var pactFilePath = pact.PactFileUri;

            pact.MockService(1234);

            var callCount = 0;
            mockFileSystem.File
                .When(x => x.WriteAllText(pactFilePath, Arg.Any<string>()))
                .Do(x =>
                {
                    if (callCount++ < 1)
                    {
                        throw new System.IO.DirectoryNotFoundException();
                    }
                });

            pact.Dispose();

            mockMockProviderService.Received(1).Stop();

            mockFileSystem.File.Received(2).WriteAllText(pactFilePath, Arg.Any<string>());
            mockFileSystem.Directory.Received(1).CreateDirectory(Arg.Any<string>());
        }
    }
}
