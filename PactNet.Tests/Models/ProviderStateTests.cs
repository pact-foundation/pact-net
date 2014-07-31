using System;
using PactNet.Models;
using Xunit;

namespace PactNet.Tests.Models
{
    public class ProviderStateTests
    {
        [Fact]
        public void Ctor_WithProviderState_SetsProviderStateDescription()
        {
            const string provideStateDescription = "my provider state";
            var providerState = new ProviderState(provideStateDescription);

            Assert.Equal(provideStateDescription, providerState.ProviderStateDescription);
        }

        [Fact]
        public void Ctor_WithSetUpAction_SetsSetUpAction()
        {
            Action setUp = () => { };
            var providerState = new ProviderState("my provider state", setUp: setUp);

            Assert.Equal(setUp, providerState.SetUp);
        }

        [Fact]
        public void Ctor_WithTearDownAction_SetsTearDownAction()
        {
            Action tearDown = () => { };
            var providerState = new ProviderState("my provider state", tearDown: tearDown);

            Assert.Equal(tearDown, providerState.TearDown);
        }
    }
}
