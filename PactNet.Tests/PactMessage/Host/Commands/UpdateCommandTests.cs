using NSubstitute;
using PactNet.Core;
using PactNet.PactMessage.Host;
using PactNet.PactMessage.Host.Commands;
using PactNet.PactMessage.Models;
using System;
using System.Collections.Generic;
using Xunit;

namespace PactNet.Tests.PactMessage.Host.Commands
{
    public class UpdateCommandTests
    {
        [Fact]
        public void Should_Set_Correct_Arguments()
        {
            // Arrange
            var pactConfig = new PactConfig
            {
                PactDir = "dir\\pacts",
                SpecificationVersion = "3.0.0"
            };

            var messageInteraction = new MessageInteraction
            {
                Contents = new { key = "value" },
                ProviderStates = new List<ProviderState> { new ProviderState { Name = "provider-state" } }
            };

            var coreHostFactory = Substitute.For<Func<PactMessageHostConfig, IPactCoreHost>>();
            PactMessageHostConfig pactMessageHostConfig = null;
            coreHostFactory.Invoke(Arg.Do<PactMessageHostConfig>(x => pactMessageHostConfig = x));

            var updateCommand = new UpdateCommand("consumer", "provider", pactConfig, messageInteraction, coreHostFactory, null);

            // Act
            updateCommand.Execute();

            // Assert
            var expectedArguments = "update \"{\\\"providerStates\\\":[{\\\"name\\\":\\\"provider-state\\\"}]," +
                "\\\"contents\\\":{\\\"key\\\":\\\"value\\\"},\\\"metadata\\\":null}\" " +
                "--consumer=\"consumer\" --provider=\"provider\" --pact-dir=dir/pacts/ --pact-specification-version=3.0.0";
            Assert.Equal(expectedArguments, pactMessageHostConfig.Arguments);
        }
    }
}
