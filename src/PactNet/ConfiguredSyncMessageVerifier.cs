using System;
using System.Text.Json;
using System.Threading.Tasks;
using PactNet.Drivers;
using PactNet.Exceptions;
using PactNet.Interop;
using PactNet.Models;

namespace PactNet
{
    /// <summary>
    /// Verifies a configured synchronous (request/response) message interaction
    /// </summary>
    internal class ConfiguredSyncMessageVerifier : IConfiguredSyncMessageVerifier
    {
        // the native message returned from the FFI always uses camel case property
        // names, but the inner content may use different settings supplied by the user
        private static readonly JsonSerializerOptions NativeMessageSettings = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private readonly ISyncMessageInteractionDriver driver;
        private readonly PactConfig config;
        private readonly PactSpecification version;

        /// <summary>
        /// Initialises a new instance of the <see cref="ConfiguredSyncMessageVerifier"/>
        /// </summary>
        /// <param name="driver">Pact driver</param>
        /// <param name="config">Pact configuration</param>
        /// <param name="version">Pact specification version</param>
        internal ConfiguredSyncMessageVerifier(ISyncMessageInteractionDriver driver, PactConfig config, PactSpecification version)
        {
            this.driver = driver;
            this.config = config;
            this.version = version;
        }

        /// <summary>
        /// Verify a request message is read and handled correctly and write the message pact
        /// </summary>
        /// <param name="handler">The method handling the request message and producing a response</param>
        public void Verify<TRequest, TResponse>(Func<TRequest, TResponse> handler)
        {
            try
            {
                TRequest requestReified = this.RequestReified<TRequest>();

                handler(requestReified);

                this.driver.WritePactFile(this.config.PactDir);
            }
            catch (Exception e)
            {
                throw new PactMessageConsumerVerificationException($"The message could not be verified by the consumer handler", e);
            }
        }

        /// <summary>
        /// Verify a request message is read and handled correctly and write the message pact
        /// </summary>
        /// <param name="handler">The method handling the request message and producing a response</param>
        public async Task VerifyAsync<TRequest, TResponse>(Func<TRequest, Task<TResponse>> handler)
        {
            try
            {
                TRequest requestReified = this.RequestReified<TRequest>();

                await handler(requestReified);

                this.driver.WritePactFile(this.config.PactDir);
            }
            catch (Exception e)
            {
                throw new PactMessageConsumerVerificationException($"The message could not be verified by the consumer handler", e);
            }
        }

        /// <summary>
        /// Try to read the reified request message
        /// </summary>
        /// <typeparam name="TRequest">the type of the request message</typeparam>
        /// <returns>the request message</returns>
        private TRequest RequestReified<TRequest>()
        {
            string reified = this.driver.Reify();
            NativeSyncMessage content = JsonSerializer.Deserialize<NativeSyncMessage>(reified, NativeMessageSettings);

            // Synchronous messages only exist in the V4 Pact format, so the reified contents
            // always use the V4 body envelope (a `content` field)
            string contentString = ((JsonElement)content.Request.Contents).GetProperty("content").GetRawText();

            TRequest requestReified = JsonSerializer.Deserialize<TRequest>(contentString, this.config.DefaultJsonSettings);

            return requestReified;
        }
    }
}
