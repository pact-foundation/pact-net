using System;
using System.Collections.Generic;
using System.IO;
using PactNet;
using PactNet.Exceptions;
using PactNet.Infrastructure.Outputters;
using Xunit;
using PactNet.Output.Xunit;
using PactNet.Verifier;
using Xunit.Abstractions;

namespace GrpcGreeter.Tests
{
    public class GrpcGreeterTests(ITestOutputHelper output, ServerFixture serverFixture) : IClassFixture<ServerFixture>, IDisposable
    {
        private readonly PactVerifier verifier = new("Grpc Greeter Api", new PactVerifierConfig
        {
            LogLevel = PactLogLevel.Debug,
            Outputters = new List<IOutput>
            {
                new XunitOutput(output)
            }
        });

        private readonly string pactPath = Path.Combine("..", "..", "..", "..", "..", "Grpc", "pacts",
            "grpc-greeter-client-grpc-greeter.json");

        [Fact]
        public void VerificationThrowsExceptionWhenNoRunningProvider()
        {
            Assert.Throws<PactVerificationFailedException>(() => verifier
                .WithHttpEndpoint(new Uri("http://localhost:5060"))
                .WithFileSource(new FileInfo(pactPath))
                .Verify());
        }

        [Fact]
        public void VerificationSuccessForRunningProvider()
        {
            verifier.WithHttpEndpoint(serverFixture.ProviderUri)
                .WithFileSource(new FileInfo(pactPath))
                .Verify();
        }

        public void Dispose()
        {
            this.verifier?.Dispose();
        }
    }
}
