using System.Collections.Generic;
using System.IO;
using NSubstitute;
using PactNet.Models;
using PactNet.PactMessage.Models;
using PactNet.Wrappers;
using Xunit;

namespace PactNet.Tests
{
	public class PactMergerTests
	{
		private IFileWrapper _fileWrapper;
		private const string PactDir = @"..\PactExamples";

		private PactMerger GetSubject()
		{
			_fileWrapper = Substitute.For<IFileWrapper>();
			var pactMerger = new PactMerger(PactDir, _fileWrapper);
			return pactMerger;
		}

		[Fact]
		public void DeleteUnexpectedInteractions_NoInteractionsFileDoesNotExist_DoesNotTryToDeleteFile()
		{
			//Arrange
			var pactMerger = GetSubject();
			_fileWrapper.Exists(PactDir + @"\Test_consumer_Test_provider_V1.txt").Returns(x => false);

			//Act
			pactMerger.DeleteUnexpectedInteractions(null, "Test consumer", "Test provider V1");

			//Assert
			_fileWrapper.DidNotReceive().Delete(Arg.Any<string>());
		}

		[Fact]
		public void DeleteUnexpectedInteractions_NullInteractionsFileExists_DeletesFile()
		{
			//Arrange
			var pactMerger = GetSubject();
			_fileWrapper.Exists(PactDir + @"\Test_consumer_Test_provider_V1.txt").Returns(x => true);

			//Act
			pactMerger.DeleteUnexpectedInteractions(null, "Test consumer", "Test provider V1");

			//Assert
			_fileWrapper.Received().Delete(PactDir + @"\Test_consumer_Test_provider_V1.txt");
		}

		[Fact]
		public void DeleteUnexpectedInteractions_EmptyInteractionsFileExists_DeletesFile()
		{
			//Arrange
			var pactMerger = GetSubject();
			_fileWrapper.Exists(PactDir + @"\Test_consumer_Test_provider_V1.txt").Returns(x => true);

			//Act
			pactMerger.DeleteUnexpectedInteractions(new Interaction[0], "Test consumer", "Test provider V1");

			//Assert
			_fileWrapper.Received().Delete(PactDir + @"\Test_consumer_Test_provider_V1.txt");
		}

		[Fact]
		public void DeleteUnexpectedInteractions_NoUnexpectedInteractionsInFileV1_DoesNotReomveThemFromFile()
		{
			//Arrange
			var pactMerger = GetSubject();
			_fileWrapper.Exists(PactDir + @"\Test_consumer_Test_provider_V1.txt").Returns(x => true);
			_fileWrapper.ReadAllText(PactDir + @"\Test_consumer_Test_provider_V1.txt").Returns(x => File.ReadAllText(PactDir + @"\Test_consumer_Test_provider_V1.txt"));

			var expectedInterations = new List<MessageInteraction>
			{
				new MessageInteraction
				{
					Description = "Test interaction",
					ProviderState = "Test state",
					Contents = new
					{
						Test = "Test"
					}
				},
				new MessageInteraction
				{
					Description = "Test interaction",
					ProviderStates = new[]
					{
						new ProviderState {Name = "Test 2"},
						new ProviderState {Name = "Test 3"}
					},
					Contents = new
					{
						Test = "Test"
					}
				},
			};

			//Act
			pactMerger.DeleteUnexpectedInteractions(expectedInterations, "Test consumer", "Test provider V1");

			//Assert
			_fileWrapper.DidNotReceive().Delete(PactDir + @"\Test_consumer_Test_provider_V1.txt");
			_fileWrapper.DidNotReceive().WriteAllText(PactDir + @"\Test_consumer_Test_provider_V1.txt", Arg.Any<string>());
		}

		[Fact]
		public void DeleteUnexpectedInteractions_NoUnexpectedInteractionsInFileV2_DoesNotReomveThemFromFile()
		{
			//Arrange
			var pactMerger = GetSubject();
			_fileWrapper.Exists(PactDir + @"\Test_consumer_Test_provider_V2.txt").Returns(x => true);
			_fileWrapper.ReadAllText(PactDir + @"\Test_consumer_Test_provider_V2.txt").Returns(x => File.ReadAllText(PactDir + @"\Test_consumer_Test_provider_V2.txt"));

			var expectedInterations = new List<MessageInteraction>
			{
				new MessageInteraction
				{
					Description = "Test interaction",
					ProviderState = "Test state",
					Contents = new
					{
						Test = "Test"
					}
				},
				new MessageInteraction
				{
					Description = "Test interaction",
					ProviderStates = new[]
					{
						new ProviderState {Name = "Test 2"},
						new ProviderState {Name = "Test 3"}
					},
					Contents = new
					{
						Test = "Test"
					}
				},
			};

			//Act
			pactMerger.DeleteUnexpectedInteractions(expectedInterations, "Test consumer", "Test provider V2");

			//Assert
			_fileWrapper.DidNotReceive().Delete(PactDir + @"\Test_consumer_Test_provider_V2.txt");
			_fileWrapper.DidNotReceive().WriteAllText(PactDir + @"\Test_consumer_Test_provider_V2.txt", Arg.Any<string>());
		}

		[Fact]	
		public void DeleteUnexpectedInteractions_NoUnexpectedInteractionsInFileV3_DoesNotReomveThemFromFile()
		{
			//Arrange
			var pactMerger = GetSubject();
			_fileWrapper.Exists(PactDir + @"\Test_consumer_Test_provider_V3_Multiple_states.txt").Returns(x => true);
			_fileWrapper.ReadAllText(PactDir + @"\Test_consumer_Test_provider_V3_Multiple_states.txt").Returns(x => File.ReadAllText(PactDir + @"\Test_consumer_Test_provider_V3_Multiple_states.txt"));

			var expectedInterations = new List<MessageInteraction>
			{
				new MessageInteraction
				{
					Description = "Test interaction",
					ProviderState = "Test state",
					Contents = new
					{
						Test = "Test"
					}
				},
				new MessageInteraction
				{
					Description = "Test interaction",
					ProviderStates = new[]
					{
						new ProviderState {Name = "Test 2"},
						new ProviderState {Name = "Test 3"}
					},
					Contents = new
					{
						Test = "Test"
					}
				},
			};

			//Act
			pactMerger.DeleteUnexpectedInteractions(expectedInterations, "Test consumer", "Test provider V3 Multiple states");

			//Assert
			_fileWrapper.DidNotReceive().Delete(PactDir + @"\Test_consumer_Test_provider_V3_Multiple_states.txt");
			_fileWrapper.DidNotReceive().WriteAllText(PactDir + @"\Test_consumer_Test_provider_V3_Multiple_states.txt", Arg.Any<string>());
		}

		[Fact]
		public void DeleteUnexpectedInteractions_NoUnexpectedInteractionsInFileV3WithMessages_DoesNotReomveThemFromFile()
		{
			//Arrange
			var pactMerger = GetSubject();
			_fileWrapper.Exists(PactDir + @"\Test_consumer_Test_provider_V3_Messages.txt").Returns(x => true);
			_fileWrapper.ReadAllText(PactDir + @"\Test_consumer_Test_provider_V3_Messages.txt").Returns(x => File.ReadAllText(PactDir + @"\Test_consumer_Test_provider_V3_Messages.txt"));

			var expectedInterations = new List<MessageInteraction>
			{
				new MessageInteraction
				{
					Description = "Test interaction",
					ProviderState = "Test state",
					Contents = new
					{
						Test = "Test"
					}
				},
				new MessageInteraction
				{
					Description = "Test interaction",
					ProviderStates = new[]
					{
						new ProviderState {Name = "Test 2"},
						new ProviderState {Name = "Test 3"}
					},
					Contents = new
					{
						Test = "Test"
					}
				},
			};

			//Act
			pactMerger.DeleteUnexpectedInteractions(expectedInterations, "Test consumer", "Test provider V3 Messages");

			//Assert
			_fileWrapper.DidNotReceive().Delete(PactDir + @"\Test_consumer_Test_provider_V3_Messages.txt");
			_fileWrapper.DidNotReceive().WriteAllText(PactDir + @"\Test_consumer_Test_provider_V3_Messages.txt", Arg.Any<string>());
		}

		[Fact]
		public void DeleteUnexpectedInteractions_UnexpectedInteractionsInFileV1_ReomvesThemFromFile()
		{
			//Arrange
			var pactMerger = GetSubject();
			_fileWrapper.Exists(PactDir + @"\Test_consumer_Test_provider_V1.txt").Returns(x => true);
			_fileWrapper.ReadAllText(PactDir + @"\Test_consumer_Test_provider_V1.txt").Returns(x => File.ReadAllText(PactDir + @"\Test_consumer_Test_provider_V1.txt"));

			var expectedInterations = new List<MessageInteraction>
			{
				new MessageInteraction
				{
					Description = "Test interaction",
					ProviderState = "Test state",
					Contents = new
					{
						Test = "Test"
					}
				},
				new MessageInteraction
				{
					Description = "Test interaction",
					ProviderStates = new[]
					{
						new ProviderState {Name = "Test 2"},
						new ProviderState {Name = "Test 3"}
					},
					Contents = new
					{
						Test = "Test"
					}
				},
			};

			//Act
			pactMerger.DeleteUnexpectedInteractions(expectedInterations, "Test consumer", "Test provider V1");

			//Assert
			_fileWrapper.DidNotReceive().Delete(PactDir + @"\Test_consumer_Test_provider_V1.txt");
			_fileWrapper.Received().WriteAllText(PactDir + @"\Test_consumer_Test_provider_V1.txt", File.ReadAllText(PactDir + @"\Test_consumer_Test_provider_V1_After_Merge.txt"));
		}
	}
}
