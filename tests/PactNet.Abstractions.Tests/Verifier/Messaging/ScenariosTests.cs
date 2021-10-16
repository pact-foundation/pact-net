using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using PactNet.Verifier.Messaging;
using Xunit;

namespace PactNet.Abstractions.Tests.Verifier.Messaging
{
    public class ScenariosTests
    {
        private List<Scenario> expectedScenarios => new List<Scenario>()
        {
            new Scenario("scenario1", () => "scenario1_object"),
            new Scenario("scenario2", () => "scenario2_object"),
            new Scenario("scenario3", () => "scenario3_object"),
            new Scenario("scenario4", () => "scenario4_object")
        };

        public ScenariosTests()
        {
            //The static class Scenarios is intended to be used as a in memory storage for scenario invoking.
            // - therefore, to make it thread-safe in a unit testing context, a ClearScenario is to be called before each test.
            Scenarios.ClearScenarios();
        }

        [Fact]
        public void Should_Be_Able_To_Add_A_Scenario()
        {
            var expectedScenario = expectedScenarios.FirstOrDefault();

            Scenarios.AddScenario(expectedScenario);

            Scenarios.NumberOfScenarios.Should().Be(1);
        }

        [Fact]
        public void Should_Fail_To_Add_A_Scenario_If_Invalid_Scenario()
        {
            Action actual = () => Scenarios.AddScenario(null);

            actual.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Should_Fail_To_Add_A_Scenario_If_Scenario_Already_Added()
        {
            var expectedScenario = expectedScenarios.FirstOrDefault();
            var expectedDescription = expectedScenario.Description;

            Scenarios.AddScenario(expectedScenario);

            Action actual = () => Scenarios.AddScenario(expectedScenario);

            actual.Should().Throw<InvalidOperationException>()
                .WithMessage($"Scenario \"{expectedDescription}\" already added");
        }

        [Fact]
        public void Should_Be_Able_To_Add_Multiple_Scenarios()
        {
            var expectedNumberOfScenarios = expectedScenarios.Count;

            Scenarios.AddScenarios(expectedScenarios);

            Scenarios.NumberOfScenarios.Should().Be(expectedNumberOfScenarios);
        }

        [Fact]
        public void Should_Fail_To_Add_Multiple_Scenarios_If_Null_Scenario_List()
        {
            Action actual = () => Scenarios.AddScenarios(null);

            actual.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Should_Fail_To_Add_Multiple_Scenarios_If_Empty_Scenario_List()
        {
            Action actual = () => Scenarios.AddScenarios(new List<Scenario>());

            actual.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Should_Fail_To_Add_Multiple_Scenario_If_Scenario_Already_Added()
        {
            var expectedScenario = expectedScenarios.FirstOrDefault();

            Scenarios.AddScenarios(expectedScenarios);

            Action actual = () => Scenarios.AddScenarios(expectedScenarios);
            actual.Should().Throw<InvalidOperationException>()
                .WithMessage($"A scenario has already been added");
        }

        [Fact]
        public void Should_Be_Able_To_Get_Single_Scenario()
        {
            var expectedScenario = expectedScenarios.FirstOrDefault();
            var expectedDescription = expectedScenario.Description;

            Scenarios.AddScenario(expectedScenario);

            var actualScenario = Scenarios.GetByDescription(expectedDescription);

            actualScenario.Should().BeEquivalentTo(expectedScenario);
        }

        [Fact]
        public void Should_Be_Able_To_Tell_If_Scenario_Exists()
        {
            var expectedScenario = expectedScenarios.FirstOrDefault();
            var expectedDescription = expectedScenario.Description;

            Scenarios.AddScenario(expectedScenario);

            Scenarios.Exist(expectedDescription).Should().Be(true);
        }

        [Fact]
        public void Should_Be_Able_To_Execute_Scenario()
        {
            var expectedScenario = expectedScenarios.FirstOrDefault();
            var expectedDescription = expectedScenario.Description;
            var expectedReturnedObject = expectedScenario.InvokeScenario();

            Scenarios.AddScenarios(expectedScenarios);

            var actual = Scenarios.InvokeScenario(expectedDescription);

            Assert.Equal(expectedReturnedObject, actual);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void Should_Fail_To_Execute_Scenario_When_Invalid_Description(string description)
        {
            Scenarios.AddScenarios(expectedScenarios);

            Action actual = () => Scenarios.InvokeScenario(description);

            actual.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Should_Fail_To_Execute_Scenario_If_No_Scenario_Added()
        {
            Action actual = () => Scenarios.InvokeScenario("description");

            actual.Should().Throw<InvalidOperationException>()
                .WithMessage("Scenario \"description\" not found. You need to add the scenario first");
        }

        [Fact]
        public void Should_Fail_To_Execute_Scenario_If_Scenario_Not_Found()
        {
            Scenarios.AddScenarios(expectedScenarios);

            Action actual = () => Scenarios.InvokeScenario("description");

            actual.Should().Throw<InvalidOperationException>()
                .WithMessage("Scenario \"description\" not found. You need to add the scenario first");
        }

        [Fact]
        public void Should_Be_Able_To_Clear_The_Scenarios()
        {
            Scenarios.AddScenarios(expectedScenarios);

            Scenarios.ClearScenarios();

            Scenarios.NumberOfScenarios.Should().Be(0);
        }
    }
}
