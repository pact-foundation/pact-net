using System.Collections.Generic;
using FluentAssertions;
using PactNet.Exceptions;
using PactNet.Verifier.ProviderState;
using Xunit;

namespace PactNet.Tests.Verifier.ProviderState
{
    public class StateHandlerTests
    {
        private string actualValue;
        private bool handlerCalled;

        public StateHandlerTests()
        {
            actualValue = "initial value";
            handlerCalled = false;
        }

        [Fact]
        public void Should_have_a_description_and_a_default_action()
        {
            string expectedDescription = "a state";
            StateAction expectedAction = StateAction.Setup;

            StateHandler handler = new StateHandler(expectedDescription, this.CommandHandler);

            handler.Description.Should().Be(expectedDescription);
            handler.Action.Should().Be(expectedAction);
        }

        [Fact]
        public void Should_invoke_state_without_arguments()
        {
            StateHandler handler = new StateHandler("a state", this.CommandHandler);

            handler.Execute();

            CheckCommandHasBeenCalled();
        }

        [Fact]
        public void Should_fail_to_invoke_state_without_arguments_if_action_not_well_configured()
        {
            StateHandler handler = new StateHandler("a state", this.CommandHandlerWithArgs);

            handler
                .Invoking(x => x.Execute())
                .Should().Throw<StateHandlerExecutionException>();
        }

        [Fact]
        public void Should_invoke_state_with_arguments()
        {
            string expectedValue = "good value";

            //Arrange
            StateHandler handler = new StateHandler("a state", this.CommandHandlerWithArgs);

            //Act
            handler.Execute(new Dictionary<string, string>{{"paramToChange", expectedValue}});

            //Arrange
            CheckValueChangedFromCommandWithArgs(expectedValue);
        }

        [Fact]
        public void Should_fail_to_invoke_state_with_arguments_if_action_not_well_configured()
        {
            StateHandler handler = new StateHandler("a state", this.CommandHandler);

            handler
                .Invoking(x => x.Execute(new Dictionary<string, string>{{"param", "newValue"}}))
                .Should().Throw<StateHandlerExecutionException>();
        }

        [Fact]
        public void Should_be_able_to_invoke_both_invoker_with_and_without_arguments()
        {
            string expectedValue = "good value";

            //Arrange
            StateHandler handler = new StateHandler("a state", this.CommandHandler, this.CommandHandlerWithArgs);

            //Act
            handler.Execute(new Dictionary<string, string> {{ "paramToChange", expectedValue }});
            handler.Execute();

            //Assert
            CheckCommandHasBeenCalled();
            CheckValueChangedFromCommandWithArgs(expectedValue);
        }

        private void CheckCommandHasBeenCalled()
        {
            this.handlerCalled.Should().BeTrue();
        }

        private void CheckValueChangedFromCommandWithArgs(string expectedValue)
        {
            this.actualValue.Should().Be(expectedValue);
        }

        private void CommandHandler()
        {
            handlerCalled = true;
        }

        private void CommandHandlerWithArgs(IDictionary<string, string> args)
        {
            actualValue = args["paramToChange"];
        }
    }
}
