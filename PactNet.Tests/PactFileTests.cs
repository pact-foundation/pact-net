using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PactNet.Tests
{
    public class PactFileTests
    {
        [Fact]
        public void Interactions_WithNoInteractions_ReturnsCountOfZero()
        {
            var pactFile = new PactFile();

            Assert.Equal(0, pactFile.Interactions.Count());
        }

        [Fact]
        public void AddInteraction_WithNoExistingInteractions_AddsTheInteraction()
        {
            var pactFile = new PactFile();
            var interaction = new PactInteraction
            {
                Description = "A description of the interaction",
                ProviderState = "Name of the provider state",
                Request = new PactProviderRequest(),
                Response = new PactProviderResponse()
            };

            pactFile.AddInteraction(interaction);

            Assert.Equal(1, pactFile.Interactions.Count());
            Assert.Equal(interaction, pactFile.Interactions.First());
        }

        [Fact]
        public void AddInteraction_WithExistingInteractionWhenAddingANewInteraction_TheInteractionIsAdded()
        {
            var pactFile = new PactFile();

            var interaction1 = new PactInteraction
            {
                Description = "My interaction",
                Request = new PactProviderRequest(),
                Response = new PactProviderResponse()
            };

            pactFile.AddInteraction(interaction1);

            var interaction2 = new PactInteraction
            {
                Description = "A description of the interaction",
                ProviderState = "Name of the provider state",
                Request = new PactProviderRequest(),
                Response = new PactProviderResponse()
            };

            pactFile.AddInteraction(interaction2);

            Assert.Equal(2, pactFile.Interactions.Count());
            Assert.Equal(interaction2, pactFile.Interactions.Last());
        }

        [Fact]
        public void AddInteraction_WhenAddingANullInteraction_TheInteractionIsNotAdded()
        {
            var pactFile = new PactFile();

            pactFile.AddInteraction(null);

            Assert.Equal(0, pactFile.Interactions.Count());
        }

        [Fact]
        public void AddInteractions_WhenTwoInteractions_TheInteractionsAreAdded()
        {
            var pactFile = new PactFile();
            var interactions = new List<PactInteraction>
            {
                new PactInteraction
                {
                    Description = "A description of the interaction",
                    Request = new PactProviderRequest(),
                    Response = new PactProviderResponse()
                },
                new PactInteraction
                {
                    Description = "A description of the interaction 2",
                    Request = new PactProviderRequest(),
                    Response = new PactProviderResponse()
                }
            };

            pactFile.AddInteractions(interactions);

            Assert.Equal(2, pactFile.Interactions.Count());
        }

        [Fact]
        public void AddInteractions_WhenAddingNullInteractions_TheInteractionsAreNotAdded()
        {
            var pactFile = new PactFile();

            pactFile.AddInteractions(null);

            Assert.Equal(0, pactFile.Interactions.Count());
        }

        [Fact]
        public void AddInteractions_WhenAddingEmptyInteractions_TheInteractionsAreNotAdded()
        {
            var pactFile = new PactFile();

            pactFile.AddInteractions(new List<PactInteraction>());

            Assert.Equal(0, pactFile.Interactions.Count());
        }
    }
}
