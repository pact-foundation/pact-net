using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using NSubstitute;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Models;
using Xunit;

namespace PactNet.Tests
{
    public class PactBuilderTests
    {
        public IPactBuilder GetSubject()
        {
            return new PactBuilder();
        }

        [Fact]
        public void ServiceConsumer_WithConsumerName_SetsConsumerName()
        {
            const string consumerName = "My Service Consumer";
            var pactBuilder = GetSubject();

            pactBuilder.ServiceConsumer(consumerName);

            Assert.Equal(consumerName, ((PactBuilder)pactBuilder).ConsumerName);
        }

        [Fact]
        public void ServiceConsumer_WithNullConsumerName_ThrowsArgumentException()
        {
            var pactBuilder = GetSubject();

            Assert.Throws<ArgumentException>(() => pactBuilder.ServiceConsumer(null));
        }

        [Fact]
        public void ServiceConsumer_WithEmptyConsumerName_ThrowsArgumentException()
        {
            var pactBuilder = GetSubject();

            Assert.Throws<ArgumentException>(() => pactBuilder.ServiceConsumer(String.Empty));
        }

        [Fact]
        public void HasPactWith_WithProviderName_SetsProviderName()
        {
            const string providerName = "My Service Provider";
            var pact = GetSubject();

            pact.HasPactWith(providerName);

            Assert.Equal(providerName, ((PactBuilder)pact).ProviderName);
        }

        [Fact]
        public void HasPactWith_WithNullProviderName_ThrowsArgumentException()
        {
            var pactBuilder = GetSubject();

            Assert.Throws<ArgumentException>(() => pactBuilder.HasPactWith(null));
        }

        [Fact]
        public void HasPactWith_WithEmptyProviderName_ThrowsArgumentException()
        {
            var pactBuilder = GetSubject();

            Assert.Throws<ArgumentException>(() => pactBuilder.HasPactWith(String.Empty));
        }

        [Fact]
        public void PactFileUri_WhenCalledBeforeConsumerAndProviderNamesHaveBeenSet_ReturnsFileSystemPathWithNoConsumerAndProviderNameAndDoesNotThrow()
        {
            var pactBuilder = GetSubject();
            var uri = ((PactBuilder)pactBuilder).PactFileUri;

            Assert.Equal("../../pacts/-.json", uri);
        }

        [Fact]
        public void PactFileUri_WhenConsumerAndProviderNamesHaveBeenSet_ReturnsFileSystemPathWithCorrectNamesLowercaseAndWithSpacedReplacedWithUnderscores()
        {
            var pactBuilder = GetSubject()
                .ServiceConsumer("My Client")
                .HasPactWith("My Service");

            var uri = ((PactBuilder)pactBuilder).PactFileUri;

            Assert.Equal("../../pacts/my_client-my_service.json", uri);
        }

        [Fact]
        public void MockService_WhenCalled_StartIsCalledAndMockProviderServiceIsReturned()
        {
            var mockMockProviderService = Substitute.For<IMockProviderService>();

            IPactBuilder pactBuilder = new PactBuilder((port, enableSsl) => mockMockProviderService, null);

            var mockProviderService = pactBuilder.MockService(1234);

            mockMockProviderService.Received(1).Start();
            Assert.Equal(mockMockProviderService, mockProviderService);
        }

        [Fact]
        public void MockService_WhenCalledTwice_StopIsCalledTheSecondTime()
        {
            var mockMockProviderService = Substitute.For<IMockProviderService>();

            IPactBuilder pactBuilder = new PactBuilder((port, enableSsl) => mockMockProviderService, null);

            pactBuilder.MockService(1234);
            mockMockProviderService.Received(0).Stop();

            pactBuilder.MockService(1234);
            mockMockProviderService.Received(1).Stop();
        }

        [Fact]
        public void MockService_WhenCalled_MockProviderServiceFactoryIsInvokedWithSslNotEnabled()
        {
            var calledWithSslEnabled = false;
            var mockMockProviderService = Substitute.For<IMockProviderService>();

            IPactBuilder pactBuilder = new PactBuilder((port, enableSsl) =>
            {
                calledWithSslEnabled = enableSsl;
                return mockMockProviderService;
            }, null);

            pactBuilder.MockService(1234);

            Assert.False(calledWithSslEnabled);
        }

        [Fact]
        public void MockService_WhenCalledWithEnableSslFalse_MockProviderServiceFactoryIsInvokedWithSslNotEnabled()
        {
            var calledWithSslEnabled = false;
            var mockMockProviderService = Substitute.For<IMockProviderService>();

            IPactBuilder pactBuilder = new PactBuilder((port, enableSsl) =>
            {
                calledWithSslEnabled = enableSsl;
                return mockMockProviderService;
            }, null);

            pactBuilder.MockService(1234, false);

            Assert.False(calledWithSslEnabled);
        }

        [Fact]
        public void MockService_WhenCalledWithEnableSslTrue_MockProviderServiceFactoryIsInvokedWithSslEnabled()
        {
            var calledWithSslEnabled = false;
            var mockMockProviderService = Substitute.For<IMockProviderService>();

            IPactBuilder pactBuilder = new PactBuilder((port, enableSsl) =>
            {
                calledWithSslEnabled = enableSsl;
                return mockMockProviderService;
            }, null);

            pactBuilder.MockService(1234, true);

            Assert.True(calledWithSslEnabled);
        }

        [Fact]
        public void Build_WhenCalledWithoutConsumerNameSet_ThrowsInvalidOperationException()
        {
            IPactBuilder pactBuilder = new PactBuilder(null, null)
                .HasPactWith("Event API");

            Assert.Throws<InvalidOperationException>(() => pactBuilder.Build());
        }

        [Fact]
        public void Build_WhenCalledWithoutProviderNameSet_ThrowsInvalidOperationException()
        {
            IPactBuilder pact = new PactBuilder(null, null)
                .ServiceConsumer("Event Client");

            Assert.Throws<InvalidOperationException>(() => pact.Build());
        }

        [Fact]
        public void Build_WhenCalledWithNoMockProviderService_NewPactFileIsSavedWithNoInteractions()
        {
            var mockFileSystem = Substitute.For<IFileSystem>();

            IPactBuilder pactBuilder = new PactBuilder(null, mockFileSystem)
                .ServiceConsumer("Event Client")
                .HasPactWith("Event API");

            var pactFilePath = ((PactBuilder)pactBuilder).PactFileUri;

            pactBuilder.Build();

            mockFileSystem.File.Received(1).WriteAllText(pactFilePath, Arg.Any<string>());
        }

        [Fact]
        public void Build_WhenCalledWithAnInteractionOnTheMockProviderService_NewPactFileIsSavedWithTheInteraction()
        {
            var mockMockProviderService = Substitute.For<IMockProviderService>();
            var mockFileSystem = Substitute.For<IFileSystem>();

            IPactBuilder pactBuilder = new PactBuilder((port, enableSsl) => mockMockProviderService, mockFileSystem)
                .ServiceConsumer("Event Client")
                .HasPactWith("Event API");

            pactBuilder.MockService(1234);

            var pactFilePath = ((PactBuilder)pactBuilder).PactFileUri;

            mockFileSystem.File.ReadAllText(pactFilePath).Returns(x => { throw new System.IO.FileNotFoundException(); });
            mockMockProviderService.Interactions.Returns(new List<ProviderServiceInteraction>
            {
                new ProviderServiceInteraction
                {
                    Description = "My interaction",
                    Request = new ProviderServiceRequest(),
                    Response = new ProviderServiceResponse()
                }
            });

            pactBuilder.Build();

            var pactInteractions = mockMockProviderService.Received(1).Interactions;
            mockFileSystem.File.Received(1).WriteAllText(pactFilePath, Arg.Any<string>());
        }

        [Fact]
        public void Build_WhenCalledAndDirectoryDoesNotExist_DirectoryIsCreatedThenFileIsCreated()
        {
            var mockFileSystem = Substitute.For<IFileSystem>();

            IPactBuilder pactBuilder = new PactBuilder(null, mockFileSystem)
                .ServiceConsumer("Event Client")
                .HasPactWith("Event API");

            var pactFilePath = ((PactBuilder)pactBuilder).PactFileUri;

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

            pactBuilder.Build();

            mockFileSystem.File.Received(2).WriteAllText(pactFilePath, Arg.Any<string>());
            mockFileSystem.Directory.Received(1).CreateDirectory(Arg.Any<string>());
        }

        [Fact]
        public void Build_WhenCalledWithAnInitialisedMockProviderService_StopIsCallOnTheMockServiceProvider()
        {
            var mockMockProviderService = Substitute.For<IMockProviderService>();
            var mockFileSystem = Substitute.For<IFileSystem>();

            IPactBuilder pactBuilder = new PactBuilder((port, enableSsl) => mockMockProviderService, mockFileSystem)
                .ServiceConsumer("Event Client")
                .HasPactWith("Event API");

            var pactFilePath = ((PactBuilder)pactBuilder).PactFileUri;

            pactBuilder.MockService(1234);

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

            pactBuilder.Build();

            mockMockProviderService.Received(1).Stop();

            mockFileSystem.File.Received(2).WriteAllText(pactFilePath, Arg.Any<string>());
            mockFileSystem.Directory.Received(1).CreateDirectory(Arg.Any<string>());
        }
    }
}
