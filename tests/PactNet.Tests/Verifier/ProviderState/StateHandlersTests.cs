using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using PactNet.Exceptions;
using PactNet.Verifier.ProviderState;
using Provider.Tests;
using Xunit;

namespace PactNet.Tests.Verifier.ProviderState
{
    public class StateHandlersTests
    {
        private List<StateHandler> expectedStateHandlers => new List<StateHandler>()
        {
            new StateHandler("state1", () => { }),
            new StateHandler("state2", () => { }, (IDictionary<string, string> args) => { }),
            new StateHandler("state3", () => { }, StateAction.Teardown),
            new StateHandler("state4", () => { }, (IDictionary<string, string> args) => { }, StateAction.Setup)
        };

        public StateHandlersTests()
        {
            //The static class StateHandlers is intended to be used as a in memory storage for scenario invoking.
            // - therefore, to make it thread-safe in a unit testing context, a ClearScenario is to be called before each test.
            StateHandlers.ClearStateHandlers();
        }

        [Fact]
        public void Should_Be_Able_To_Add_A_Scenario()
        {
            var expectedScenario = expectedStateHandlers.FirstOrDefault();

            StateHandlers.AddStateHandler(expectedScenario);

            StateHandlers.NumberOfStates.Should().Be(1);
        }

        [Fact]
        public void Should_Fail_To_Add_A_Scenario_If_Invalid_Scenario()
        {
            Action actual = () => StateHandlers.AddStateHandler(null);

            actual.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Should_Fail_To_Add_A_Scenario_If_Scenario_Already_Added()
        {
            var stateHandler = expectedStateHandlers.FirstOrDefault();
            var expectedDescription = stateHandler.Description;

            StateHandlers.AddStateHandler(stateHandler);

            Action actual = () => StateHandlers.AddStateHandler(stateHandler);

            actual.Should().Throw<StateHandlerConfigurationException>();
        }

        [Fact]
        public void Should_Be_Able_To_Add_Multiple_StateHandlers()
        {
            var expectedNumberOfStateHandlers = expectedStateHandlers.Count;

            StateHandlers.AddStateHandlers(expectedStateHandlers);

            StateHandlers.NumberOfStates.Should().Be(expectedNumberOfStateHandlers);
        }

        [Fact]
        public void Should_Fail_To_Add_Multiple_StateHandlers_If_Null_StateHandler_List()
        {
            Action actual = () => StateHandlers.AddStateHandlers(null);

            actual.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Should_Fail_To_Add_Multiple_StateHandlers_If_Empty_StateHandler_List()
        {
            Action actual = () => StateHandlers.AddStateHandlers(new List<StateHandler>());

            actual.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Should_Fail_To_Add_Multiple_Scenario_If_Scenario_Already_Added()
        {
            var expectedScenario = expectedStateHandlers.FirstOrDefault();

            StateHandlers.AddStateHandlers(expectedStateHandlers);

            Action actual = () => StateHandlers.AddStateHandlers(expectedStateHandlers);
            actual.Should().Throw<StateHandlerConfigurationException>();
        }

        [Theory]
        [InlineData("state4", StateAction.Setup)]
        [InlineData("state1", StateAction.Setup)]
        [InlineData("state3", StateAction.Teardown)]
        public void Should_Be_Able_To_Get_Single_Scenario_At_Setup(string description, StateAction stateAction)
        {
            StateHandlers.AddStateHandlers(expectedStateHandlers);

            var actualScenario = StateHandlers.GetByDescriptionAndAction(description, stateAction);

            actualScenario.Should().NotBeNull();
        }

        [Fact]
        public void Should_Be_Able_To_Clear_The_StateHandlers()
        {
            StateHandlers.AddStateHandlers(expectedStateHandlers);

            StateHandlers.ClearStateHandlers();

            StateHandlers.NumberOfStates.Should().Be(0);
        }
    }
}
