using System;
using PactNet.Models;
using Xunit;

namespace PactNet.Tests.Models
{
    public class ProviderStatesTests
    {
        [Fact]
        public void Ctor_WithSetUpAction_SetsSetUpAction()
        {
            Action setUp = () => { };
            var providerStates = new ProviderStates(setUp: setUp);

            Assert.Equal(setUp, providerStates.SetUp);
        }

        [Fact]
        public void Ctor_WithTearDownAction_SetsTearDownAction()
        {
            Action tearDown = () => { };
            var providerStates = new ProviderStates(tearDown: tearDown);

            Assert.Equal(tearDown, providerStates.TearDown);
        }

        [Fact]
        public void Add_WithProviderState_AddsProviderState()
        {
            const string providerStateDescription = "my provider state";
            var providerState = new ProviderState(providerStateDescription);
            var providerStates = new ProviderStates();

            providerStates.Add(providerState);

            Assert.Equal(providerState, providerStates.Find(providerStateDescription));
        }

        [Fact]
        public void Add_WithAndAlreadyAddedProviderState_ThrowsArgumentException()
        {
            const string providerStateDescription = "my provider state";
            var providerState1 = new ProviderState(providerStateDescription);
            var providerState2 = new ProviderState(providerStateDescription);
            var providerStates = new ProviderStates();
            providerStates.Add(providerState1);

            Assert.Throws<ArgumentException>(() => providerStates.Add(providerState2));
        }

        [Fact]
        public void Find_WithProviderStateThatHasBeenAdded_ReturnsProviderState()
        {
            const string providerStateDescription = "my provider state 2";
            var providerState1 = new ProviderState("my provider state");
            var providerState2 = new ProviderState(providerStateDescription);
            var providerStates = new ProviderStates();
            providerStates.Add(providerState1);
            providerStates.Add(providerState2);

            var actualProviderState = providerStates.Find(providerStateDescription);

            Assert.Equal(providerState2, actualProviderState);
        }

        [Fact]
        public void Find_WithNoAddedProviderStates_ReturnsNull()
        {
            var providerStates = new ProviderStates();

            var actualProviderState = providerStates.Find("my provider state");

            Assert.Null(actualProviderState);
        }

        [Fact]
        public void Find_WithProviderStateThatDoesNotMatchProviderStates_ReturnsNull()
        {
            const string providerStateDescription = "my provider state 2";
            var providerState1 = new ProviderState("my provider state");
            var providerState2 = new ProviderState(providerStateDescription);
            var providerStates = new ProviderStates();
            providerStates.Add(providerState1);
            providerStates.Add(providerState2);

            var actualProviderState = providerStates.Find("something else");

            Assert.Null(actualProviderState);
        }
    }
}