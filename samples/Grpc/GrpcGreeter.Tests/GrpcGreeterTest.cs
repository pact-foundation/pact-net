using System.Threading.Tasks;
using System.Threading;
using FluentAssertions;
using PactNet;
using Xunit;
using PactNet.Interop;

namespace GrpcGreeter.Tests
{
    public class GrpcGreeterTests
    {
        public GrpcGreeterTests()
        {
            PactLogLevel.Information.InitialiseLogging();
        }

        [Fact]
        public void ReturnsVerificationFailureWhenNoRunningProvider()
        {
            var verifier = NativeInterop.VerifierNewForApplication("pact-dotnet","0.0.0");
            NativeInterop.VerifierSetProviderInfo(verifier,"grpc-greeter",null,null,0,null);
            NativeInterop.AddProviderTransport(verifier, "grpc",5060,"/","http");
            NativeInterop.VerifierAddFileSource(verifier,"../../../../pacts/grpc-greeter-client-grpc-greeter.json");
            var VerifierExecuteResult = NativeInterop.VerifierExecute(verifier);
            VerifierExecuteResult.Should().Be(1);
        }
        [Fact]
        public async Task ReturnsVerificationSuccessRunningProviderAsync()
        {
            var verifier = NativeInterop.VerifierNewForApplication("pact-dotnet", "0.0.0");
            NativeInterop.VerifierSetProviderInfo(verifier, "grpc-greeter", null, null, 0, null);
            NativeInterop.AddProviderTransport(verifier, "grpc", 5000, "/", "https");
            NativeInterop.VerifierAddFileSource(verifier, "../../../../pacts/grpc-greeter-client-grpc-greeter.json");

            // Arrange
            // Setup our app to run before our verifier executes
            // Setup a cancellation token so we can shutdown the app after
            var cts = new CancellationTokenSource();
            var token = cts.Token;
            var runAppTask = Task.Run(async () =>
            {
                await GrpcGreeterService.RunApp([], token);
            }, token);
            await Task.Delay(2000);

            // Act
            var VerifierExecuteResult = NativeInterop.VerifierExecute(verifier);
            VerifierExecuteResult.Should().Be(0);
            NativeInterop.VerifierShutdown(verifier);
            // After test execution, signal the task to terminate
            cts.Cancel();
        }
    }
}
