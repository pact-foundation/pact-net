using System;
using NSubstitute;
using PactNet.Configuration.Json;
using PactNet.Core;
using PactNet.Infrastructure.Outputters;
using PactNet.Matchers;
using PactNet.PactMessage;
using PactNet.PactMessage.Host.Commands;
using PactNet.PactMessage.Models;
using Xunit;

namespace PactNet.Tests.PactMessage
{
    public class MessagePactTests
    {
        [Fact]
        public void Given_WithProviderState_SetsProviderState()
        {
            //Arrange
            var messagePact = new MessagePact();
            var providerStates = new[]
            {
                new ProviderState
                {
                    Name = "Provider state test "
                }
            };

            //Act
            messagePact
                .Given(providerStates)
                .ExpectedToReceive("My description")
                .With(new Message { Contents = new { Test = "Test" } })
                .VerifyConsumer<MyMessage>(SuccessMessageHandler);

            //Assert
            Assert.Equal(providerStates, messagePact.MessageInteractions[0].ProviderStates);
        }

        [Fact]
        public void Given_WithNullProviderState_ThrowsArgumentException()
        {
            //Arrange
            var messagePact = new MessagePact();

            //Assert
            Assert.Throws<ArgumentException>(() => messagePact.Given(null));
        }

        [Fact]
        public void Given_WithEmptyProviderState_ThrowsArgumentException()
        {
            //Arrange
            var messagePact = new MessagePact();

            //Assert
            Assert.Throws<ArgumentException>(() => messagePact.Given(new ProviderState[0]));
        }


        [Fact]
        public void ExpectsToRecieve_WithDescription_SetsDescription()
        {
            //Arrange
            var messagePact = new MessagePact();
            const string description = "Test description";

            //Act
            messagePact
                .ExpectedToReceive(description)
                .With(new Message { Contents = new { Test = "Test" } })
                .VerifyConsumer<MyMessage>(SuccessMessageHandler);

            //Assert
            Assert.Equal(description, messagePact.MessageInteractions[0].Description);
        }

        [Fact]
        public void ExpectsToRecieve_WithNullDescription_ThrowsArgumentException()
        {
            //Arrange
            var messagePact = new MessagePact();

            //Act + Assert
            Assert.Throws<ArgumentException>(() => messagePact.ExpectedToReceive(null));
        }

        [Fact]
        public void ExpectsToRecieve_WithEmptyDescription_ThrowsArgumentException()
        {
            //Arrange
            var messagePact = new MessagePact();

            //Act + Assert
            Assert.Throws<ArgumentException>(() => messagePact.ExpectedToReceive(string.Empty));
        }

        [Fact]
        public void With_NullMessage_ThrowsArgumentException()
        {
            //Arrange 
            var messagePact = new MessagePact();

            //Act + Assert
            Assert.Throws<ArgumentException>(() => messagePact.With(null));
        }


        [Fact]
        public void With_WithoutDescriptionSet_ThrowsInvalidOperationException()
        {
            //Arrange 
            var messagePact = new MessagePact();

            //Act + Assert
            Assert.Throws<InvalidOperationException>(() => messagePact.With(new Message
            {
                Contents = new
                {
                    Test = "Test"
                }
            }));
        }

        [Fact]
        public void With_WithMessage_SetsMessage()
        {
            //Arrange 
            var messagePact = new MessagePact();
            const string testDescription = "Test description";
            var providerStates = new[]
            {
                new ProviderState
                {
                    Name = "Test state"
                }
            };
            var expectedContent = new
            {
                Test = "Test"
            };
            var expectedMetdata = new
            {
                ContentType = "application/json;"
            };

            //Act
            messagePact
                .Given(providerStates)
                .ExpectedToReceive(testDescription)
                .With(new Message
                {
                    Contents = expectedContent,
                    Metadata = expectedMetdata
                })
                .VerifyConsumer<MyMessage>(SuccessMessageHandler);
                
            //Assert
            Assert.True(messagePact.MessageInteractions.Count == 1);
            Assert.Equal(testDescription, messagePact.MessageInteractions[0].Description);
            Assert.Equal(expectedContent, messagePact.MessageInteractions[0].Contents);
            Assert.Equal(expectedMetdata, messagePact.MessageInteractions[0].Metadata);
            Assert.Equal(providerStates, messagePact.MessageInteractions[0].ProviderStates);
        }

        [Fact]
        public void VerifyConsumer_WithoutMessageSet_ThrowsInvalidOperationException()
        {
            //Arrange
            var outputBuilder = Substitute.For<IOutputBuilder>();
            var coreHost = Substitute.For<IPactCoreHost>();
            var reifyCommand = Substitute.For<IReifyCommand>();

            var pactMessage = new MessagePact((interaction, builder, coreHostFactory) => reifyCommand, outputBuilder, JsonConfig.ApiSerializerSettings, config => coreHost);

            //Act + Assert
            Assert.Throws<InvalidOperationException>(() => pactMessage.VerifyConsumer<MyMessage>(SuccessMessageHandler));
        }

        [Fact]
        public void VerifyConsumer_SubscriberCannotHandleMessage_PactFailureExceptionIsThrown()
        {
            //Arrange
            var outputBuilder = Substitute.For<IOutputBuilder>();
            var coreHost = Substitute.For<IPactCoreHost>();
            var reifyCommand = Substitute.For<IReifyCommand>();

            reifyCommand.When(x => x.Execute()).Do(x => outputBuilder.ToString().Returns("{\"Test\": \"Test\"}"));

            var pactMessage = new MessagePact((interaction, builder, coreHostFactory) => reifyCommand, outputBuilder, JsonConfig.ApiSerializerSettings, config => coreHost);

            //Act + Assert
            Assert.Throws<PactFailureException>(() => pactMessage.ExpectedToReceive("Test message")
                .With(new Message
                {
                    Contents = new
                    {
                        Test = Match.Type("Test")
                    }
                }).VerifyConsumer<MyMessage>(FailureMessageHandler));
        }

        [Fact]
        public void VerifyConsumer_SubscriberCanHandleMessage_ClearsOutputAfterInteraction()
        {
            //Arrange
            var outputBuilder = Substitute.For<IOutputBuilder>();
            var coreHost = Substitute.For<IPactCoreHost>();
            var reifyCommand = Substitute.For<IReifyCommand>();

            reifyCommand.When(x => x.Execute()).Do(x => outputBuilder.ToString().Returns("{\"Test\": \"Test\"}"));

            var pactMessage = new MessagePact((interaction, builder, coreHostFactory) => reifyCommand, outputBuilder, JsonConfig.ApiSerializerSettings, config => coreHost);

            //Act
            pactMessage.ExpectedToReceive("Test message")
                .With(new Message
                {
                    Contents = new
                    {
                        Test = Match.Type("Test")
                    }
                })
                .VerifyConsumer<MyMessage>(SuccessMessageHandler);

            //Assert
            outputBuilder.Received(1).Clear();
        }

        [Fact]
        public void VerifyConsumer_MultipleInteractions_VerifiesOnlyTheLastInteractionEachTime()
        {
            //Arrange
            var outputBuilder = Substitute.For<IOutputBuilder>();
            var coreHost = Substitute.For<IPactCoreHost>();
            var reifyCommand = Substitute.For<IReifyCommand>();

            var pactMessage = new MessagePact((interaction, builder, coreHostFactory) => reifyCommand, outputBuilder, JsonConfig.ApiSerializerSettings, config => coreHost);

            reifyCommand.When(x => x.Execute()).Do(x => outputBuilder.ToString().Returns("{\"Test\": \"Test 1\"}"));

            var firstMessage = new Message
            {
                Contents = new
                {
                    Test = Match.Type("Test 1")
                }
            };

            pactMessage.ExpectedToReceive("First Test message")
                .With(firstMessage)
                .VerifyConsumer<MyMessage>(SuccessMessageHandler);

            reifyCommand.When(x => x.Execute()).Do(x => outputBuilder.ToString().Returns("{\"Test\": \"Test 2\"}"));
            var secondMessage = new Message
            {
                Contents = new
                {
                    Test = Match.Type("Test 2")
                }
            };

            //Act 
            pactMessage.ExpectedToReceive("Second Test message")
                .With(secondMessage)
                .VerifyConsumer<MyMessage>(SuccessMessageHandler);

            //Assert
            outputBuilder.Received(2).Clear();
            reifyCommand.Received(2).Execute();
        }

        private static void SuccessMessageHandler(MyMessage test)
        {
        }

        private static void FailureMessageHandler(MyMessage test)
        {
            throw new NullReferenceException($"{test}");
        }

        private class MyMessage
        {
            public string Description { get; set; }
        }
    }
}
