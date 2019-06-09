using NSubstitute;
using PactNet.Core;
using PactNet.Infrastructure.Outputters;
using PactNet.PactMessage.Host;
using PactNet.PactMessage.Host.Commands;
using PactNet.PactMessage.Models;
using System;
using Xunit;

namespace PactNet.Tests.PactMessage.Host.Commands
{
    public class ReifyCommandTests
    {
        [Fact]
        public void Should_Set_Correct_Arguments()
        {
            // Arrange
            var messageInteraction = new MessageInteraction { Contents = new { key = "value" } };

            var coreHostFactory = Substitute.For<Func<PactMessageHostConfig, IPactCoreHost>>();
            PactMessageHostConfig pactMessageHostConfig = null;
            coreHostFactory.Invoke(Arg.Do<PactMessageHostConfig>(x => pactMessageHostConfig = x));

            var reifyCommand = new ReifyCommand(messageInteraction, Substitute.For<IOutputBuilder>(), coreHostFactory, null);

            // Act
            reifyCommand.Execute();

            // Assert
            Assert.Equal("reify \"{\\\"key\\\":\\\"value\\\"}\"", pactMessageHostConfig.Arguments);
        }
    }
}
