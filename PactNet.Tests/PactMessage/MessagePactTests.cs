using System;
using System.Net;
using NSubstitute;
using PactNet.Mocks.MockHttpService.Models;
using PactNet.PactMessage;
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
				.With(new Message { Contents = new { Test = "Test" } });

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
				.With(new Message { Contents = new { Test = "Test" } });

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
		public void With_WithoutDescriptionSet_ThrowsArgumentException()
		{
			//Arrange 
			var messagePact = new MessagePact();

			//Act + Assert
			Assert.Throws<ArgumentException>(() => messagePact.With(new Message()
			{
				Contents = new
				{
					Test = "Test"
				}
			}));
		}

		[Fact]
		public void With_WithMessage_CreatesNewInteraction()
		{
			//Arrange 
			var messagePact = new MessagePact();
			const string testDescription = "Test description";

			var expectedMessageInteraction = new MessageInteraction
			{
				Contents = new
				{
					Test = "Test"
				},
				Description = testDescription,
			};

			//Act
			messagePact
				.ExpectedToReceive(testDescription)
				.With(new Message
				{
					Contents = new
					{
						Test = "Test"
					}
				});

			//Assert
			Assert.True(messagePact.MessageInteractions.Count == 1);
			Assert.Equal(messagePact.MessageInteractions[0], expectedMessageInteraction);
		}
	}
}
