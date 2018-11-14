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
		private const string PactDir = @"C:\TestDirectory";

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
			_fileWrapper.Exists(PactDir + @"\Test_Consumer_Test_Provider").Returns(x => false);

			//Act
			pactMerger.DeleteUnexpectedInteractions(null, "TestConsumer", "TestProvider");

			//Assert
			_fileWrapper.DidNotReceive().Delete(Arg.Any<string>());
		}

		[Fact]
		public void DeleteUnexpectedInteractions_NullInteractionsFileExists_DeletesFile()
		{
			//Arrange
			var pactMerger = GetSubject();
			_fileWrapper.Exists(PactDir + @"\Test_Consumer_Test_Provider").Returns(x => true);

			//Act
			pactMerger.DeleteUnexpectedInteractions(null, "Test Consumer", "Test Provider");

			//Assert
			_fileWrapper.Received().Delete(PactDir + @"\Test_Consumer_Test_Provider");
		}

		[Fact]
		public void DeleteUnexpectedInteractions_EmptyInteractionsFileExists_DeletesFile()
		{
			//Arrange
			var pactMerger = GetSubject();
			_fileWrapper.Exists(PactDir + @"\Test_Consumer_Test_Provider").Returns(x => true);

			//Act
			pactMerger.DeleteUnexpectedInteractions(new Interaction[0], "Test Consumer", "Test Provider");

			//Assert
			_fileWrapper.Received().Delete(PactDir + @"\Test_Consumer_Test_Provider");
		}

		[Fact]
		public void DeleteUnexpectedInteractions_NoUnexpectedInteractionsInFile_DoesNotReomveThemFromFile()
		{
			//Arrange
			var pactMerger = GetSubject();
			_fileWrapper.Exists(PactDir + @"\Test_Consumer_Test_Provider").Returns(x => true);

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
			pactMerger.DeleteUnexpectedInteractions(new Interaction[0], "Test Consumer", "Test Provider");

			//Assert
			_fileWrapper.Received().Delete(PactDir + @"\Test_Consumer_Test_Provider");
		}
	}
}
