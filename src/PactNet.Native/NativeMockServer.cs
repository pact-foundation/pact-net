using System;
using System.Runtime.InteropServices;
using PactNet.Native.Interop;

namespace PactNet.Native
{
    /// <summary>
    /// Native mock server
    /// </summary>
    internal class NativeMockServer : IMockServer
    {
        public int CreateMockServerForPact(PactHandle pact, string addrStr, bool tls)
        {
            int result = NativeInterop.CreateMockServerForPact(pact, addrStr, tls);

            if (result > 0)
            {
                return result;
            }

            throw result switch
            {
                -1 => new InvalidOperationException("Invalid handle when starting mock server"),
                -3 => new InvalidOperationException("Unable to start mock server"),
                -4 => new InvalidOperationException("The pact reference library panicked"),
                -5 => new InvalidOperationException("The IPAddress is invalid"),
                -6 => new InvalidOperationException("Could not create the TLS configuration with the self-signed certificate"),
                _ => new InvalidOperationException($"Unknown mock server error: {result}")
            };
        }

        public string MockServerMismatches(int mockServerPort)
        {
            IntPtr matchesPtr = NativeInterop.MockServerMismatches(mockServerPort);

            return matchesPtr == IntPtr.Zero
                       ? string.Empty
                       : Marshal.PtrToStringAnsi(matchesPtr);
        }

        public string MockServerLogs(int mockServerPort)
        {
            IntPtr logsPtr = NativeInterop.MockServerLogs(mockServerPort);

            return logsPtr == IntPtr.Zero
                       ? string.Empty
                       : Marshal.PtrToStringAnsi(logsPtr);
        }

        public bool CleanupMockServer(int mockServerPort)
            => NativeInterop.CleanupMockServer(mockServerPort);

        public void WritePactFile(int mockServerPort, string directory, bool overwrite)
        {
            int result = NativeInterop.WritePactFile(mockServerPort, directory, overwrite);

            if (result == 0)
            {
                return;
            }

            throw result switch
            {
                1 => new InvalidOperationException("The pact reference library panicked"),
                2 => new InvalidOperationException("The pact file could not be written"),
                3 => new InvalidOperationException("A mock server with the provided port was not found"),
                _ => new InvalidOperationException($"Unknown error from backend: {result}")
            };
        }

        public PactHandle NewPact(string consumerName, string providerName)
            => NativeInterop.NewPact(consumerName, providerName);

        public InteractionHandle NewInteraction(PactHandle pact, string description)
            => NativeInterop.NewInteraction(pact, description);

        public bool Given(InteractionHandle interaction, string description)
            => NativeInterop.Given(interaction, description);

        public bool GivenWithParam(InteractionHandle interaction, string description, string name, string value)
            => NativeInterop.GivenWithParam(interaction, description, name, value);

        public bool WithRequest(InteractionHandle interaction, string method, string path)
            => NativeInterop.WithRequest(interaction, method, path);

        public bool WithQueryParameter(InteractionHandle interaction, string name, string value, uint index)
            => NativeInterop.WithQueryParameter(interaction, name, new UIntPtr(index), value);

        public bool WithSpecification(PactHandle pact, PactSpecification version)
            => NativeInterop.WithSpecification(pact, version);

        public bool WithRequestHeader(InteractionHandle interaction, string name, string value, uint index)
            => NativeInterop.WithHeader(interaction, InteractionPart.Request, name, new UIntPtr(index), value);

        public bool WithResponseHeader(InteractionHandle interaction, string name, string value, uint index)
            => NativeInterop.WithHeader(interaction, InteractionPart.Response, name, new UIntPtr(index), value);

        public bool ResponseStatus(InteractionHandle interaction, ushort status)
            => NativeInterop.ResponseStatus(interaction, status);

        public bool WithRequestBody(InteractionHandle interaction, string contentType, string body)
            => NativeInterop.WithBody(interaction, InteractionPart.Request, contentType, body);

        public bool WithResponseBody(InteractionHandle interaction, string contentType, string body)
            => NativeInterop.WithBody(interaction, InteractionPart.Response, contentType, body);
    }
}
