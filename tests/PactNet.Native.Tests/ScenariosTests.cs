using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using PactNet.Models;
using Xunit;

namespace PactNet.Native.Tests
{
    public class ScenariosTests
    {
        private readonly Scenarios scenarios;

        private List<Scenario> expectedScenarios => new List<Scenario>()
        {
            new Scenario("scenario1", () => "scenario1_object"),
            new Scenario("scenario2", () => "scenario2_object"),
            new Scenario("scenario3", () => "scenario3_object"),
            new Scenario("scenario4", () => "scenario4_object")
        };

        public ScenariosTests()
        {
            this.scenarios = new Scenarios();
        }

        [Fact]
        public void Should_Be_Able_To_Add_A_Scenario()
        {
            var expectedScenario = expectedScenarios.FirstOrDefault();
            var expectedDescription = expectedScenario.Description;

            scenarios.AddScenario(expectedScenario);

            scenarios.AllScenarios.Should().HaveCount(1);
        }

        [Fact]
        public void Should_Fail_To_Add_A_Scenario_If_Invalid_Scenario()
        {
            scenarios
                .Invoking(x => x.AddScenario(null))
                .Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Should_Fail_To_Add_A_Scenario_If_Scenario_Already_Added()
        {
            var expectedScenario = expectedScenarios.FirstOrDefault();
            var expectedDescription = expectedScenario.Description;

            scenarios.AddScenario(expectedScenario);

            scenarios
                .Invoking(x => x.AddScenario(expectedScenario))
                .Should().Throw<InvalidOperationException>()
                .WithMessage($"Scenario \"{expectedDescription}\" already added");
        }

        [Fact]
        public void Should_Be_Able_To_Add_Multiple_Scenarios()
        {
            var expectedNumberOfScenarios = expectedScenarios.Count;

            scenarios.AddScenarios(expectedScenarios);

            scenarios.AllScenarios.Should().HaveCount(expectedNumberOfScenarios);
        }

        [Fact]
        public void Should_Fail_To_Add_Multiple_Scenarios_If_Null_Scenario_List()
        {
            scenarios
                .Invoking(x => x.AddScenarios(null))
                .Should().Throw<ArgumentException>()
                .WithMessage("scenarios cannot be null or empty");
        }

        [Fact]
        public void Should_Fail_To_Add_Multiple_Scenarios_If_Empty_Scenario_List()
        {
            scenarios
                .Invoking(x => x.AddScenarios(new List<Scenario>()))
                .Should().Throw<ArgumentException>()
                .WithMessage("scenarios cannot be null or empty");
        }

        [Fact]
        public void Should_Fail_To_Add_Multiple_Scenario_If_Scenario_Already_Added()
        {
            var expectedScenario = expectedScenarios.FirstOrDefault();
            var expectedDescription = expectedScenario.Description;

            scenarios.AddScenarios(expectedScenarios);

            scenarios
                .Invoking(x => x.AddScenarios(expectedScenarios))
                .Should().Throw<InvalidOperationException>()
                .WithMessage($"A scenario has already been added");
        }

        [Fact]
        public void Should_Be_Able_To_Get_Single_Scenario()
        {
            var expectedScenario = expectedScenarios.FirstOrDefault();
            var expectedDescription = expectedScenario.Description;

            scenarios.AddScenario(expectedScenario);

            var actualScenario = scenarios.GetByDescription(expectedDescription);

            actualScenario.Should().BeEquivalentTo(expectedScenario);
        }

        [Fact]
        public void Should_Be_Able_To_Tell_If_Scenario_Exists()
        {
            var expectedScenario = expectedScenarios.FirstOrDefault();
            var expectedDescription = expectedScenario.Description;

            scenarios.AddScenario(expectedScenario);

            scenarios.Exist(expectedDescription).Should().Be(true);
        }

        [Fact]
        public void Should_Be_Able_To_Execute_Scenario()
        {
            var expectedScenario = expectedScenarios.FirstOrDefault();
            var expectedDescription = expectedScenario.Description;
            var expectedReturnedObject = expectedScenario.InvokeScenario();

            scenarios.AddScenarios(expectedScenarios);

            var actual = scenarios.InvokeScenario(expectedDescription);

            Assert.Equal(expectedReturnedObject, actual);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void Should_Fail_To_Execute_Scenario_When_Invalid_Description(string description)
        {
            scenarios.AddScenarios(expectedScenarios);

            scenarios
                .Invoking(x => x.InvokeScenario(description))
                .Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Should_Fail_To_Execute_Scenario_If_No_Scenario_Added()
        {
            scenarios
                .Invoking(x => x.InvokeScenario("description"))
                .Should().Throw<InvalidOperationException>()
                .WithMessage("Scenario \"description\" not found. You need to add the scenario first");
        }

        [Fact]
        public void Should_Fail_To_Execute_Scenario_If_Scenario_Not_Found()
        {
            scenarios.AddScenarios(expectedScenarios);

            scenarios
                .Invoking(x => x.InvokeScenario("description"))
                .Should().Throw<InvalidOperationException>()
                .WithMessage("Scenario \"description\" not found. You need to add the scenario first");
        }

        [Fact]
        public void Should_Be_Able_To_Clear_The_Scenarios()
        {
            scenarios.AddScenarios(expectedScenarios);

            scenarios.ClearScenarios();

            scenarios.AllScenarios.Should().BeEmpty();
        }
    }
}
