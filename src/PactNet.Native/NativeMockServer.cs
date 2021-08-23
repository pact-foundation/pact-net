using System;
using System.Runtime.InteropServices;
using PactNet.Native.Interop;

namespace PactNet.Native
{
    /// <summary>
    /// Native mock server
    /// </summary>
    internal class NativeMockServer : IMockServer, IMessageMockServer
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
                       ? "ERROR: Unable to retrieve mock server logs"
                       : Marshal.PtrToStringAnsi(logsPtr);
        }

        public bool CleanupMockServer(int mockServerPort)
            => NativeInterop.CleanupMockServer(mockServerPort);

        public void WritePactFile(int mockServerPort, string directory, bool overwrite)
            => VerifyResult(() => NativeInterop.WritePactFile(mockServerPort, directory, overwrite));

        #region Http Interaction Model Support
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

        #endregion Http Interaction Model Support

        #region Message Interaction Model Support

        public void WriteMessagePactFile(MessagePactHandle pact, string directory, bool overwrite)
            => VerifyResult(() => NativeInterop.WriteMessagePactFile(pact, directory, overwrite));

        public bool WithMessagePactMetadata(MessagePactHandle pact, string @namespace, string name, string value)
            => NativeInterop.WithMessagePactMetadata(pact, @namespace, name, value);

        public MessagePactHandle NewMessagePact(string consumerName, string providerName)
            => NativeInterop.NewMessagePact(consumerName, providerName);

        public MessageHandle NewMessage(MessagePactHandle pact, string description)
            => NativeInterop.NewMessage(pact, description);

        public bool ExpectsToReceive(MessageHandle message, string description)
            => NativeInterop.MessageExpectsToReceive(message, description);
        public bool Given(MessageHandle message, string description)
            => NativeInterop.MessageGiven(message, description);

        public bool GivenWithParam(MessageHandle message, string description, string name, string value)
            => NativeInterop.MessageGivenWithParam(message, description, name, value);

        public bool WithMetadata(MessageHandle message, string key, string value)
            => NativeInterop.MessageWithMetadata(message, key, value);

        public bool WithContents(MessageHandle message, string contentType, string body, uint size)
            => NativeInterop.MessageWithContents(message, contentType, body, new UIntPtr(size));

        public string Reify(MessageHandle message)
        {
            var allocatedString = Marshal.PtrToStringAnsi(NativeInterop.MessageReify(message));
            return allocatedString;
        }


        #endregion Message Interaction Model Support

        private void VerifyResult(Func<int> func)
        {
            var result = func.Invoke();

            if (result != 0)
            {
                throw result switch
                {
                    1 => new InvalidOperationException("The pact reference library panicked"),
                    2 => new InvalidOperationException("The pact file could not be written"),
                    3 => new InvalidOperationException("A mock server with the provided port was not found"),
                    _ => new InvalidOperationException($"Unknown error from backend: {result}")
                    };
            }
        }
    }
}
