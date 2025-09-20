using System;
using System.Text.Json;
using System.Threading.Tasks;
using PactNet.Drivers.Message;
using PactNet.Exceptions;
using PactNet.Interop;
using PactNet.Models;

namespace PactNet
{
    /// <summary>
    /// Verifies a configured message interaction
    /// </summary>
    internal class ConfiguredMessageVerifier : IConfiguredMessageVerifier
    {
        // the native message returned from the FFI always uses camel case property
        // names, but the inner content may use different settings supplied by the user
        private static readonly JsonSerializerOptions NativeMessageSettings = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private readonly IMessageInteractionDriver driver;
        private readonly PactConfig config;
        private readonly PactSpecification version;

        /// <summary>
        /// Initialises a new instance of the <see cref="ConfiguredMessageVerifier"/>
        /// </summary>
        /// <param name="driver">Pact driver</param>
        /// <param name="config">Pact configuration</param>
        /// <param name="version">Pact specification version</param>
        internal ConfiguredMessageVerifier(IMessageInteractionDriver driver, PactConfig config, PactSpecification version)
        {
            this.driver = driver;
            this.config = config;
            this.version = version;
        }

        /// <summary>
        /// Verify a message is read and handled correctly and write the message pact
        /// </summary>
        /// <param name="handler">The method using the message</param>
        public void Verify<T>(Action<T> handler)
        {
            try
            {
                var messageReified = this.MessageReified<T>();

                handler(messageReified);

                this.driver.WritePactFile(this.config.PactDir);
            }
            catch (Exception e)
            {
                throw new PactMessageConsumerVerificationException($"The message could not be verified by the consumer handler", e);
            }
        }

        /// <summary>
        /// Verify a message is read and handled correctly and write the message pact
        /// </summary>
        /// <param name="handler">The method using the message</param>
        public async Task VerifyAsync<T>(Func<T, Task> handler)
        {
            try
            {
                var messageReified = this.MessageReified<T>();

                await handler(messageReified);

                this.driver.WritePactFile(this.config.PactDir);
            }
            catch (Exception e)
            {
                throw new PactMessageConsumerVerificationException($"The message could not be verified by the consumer handler", e);
            }
        }

        /// <summary>
        /// Try to read the reified message
        /// </summary>
        /// <typeparam name="T">the type of message</typeparam>
        /// <returns>the message</returns>
        private T MessageReified<T>()
        {
            string reified = this.driver.Reify();
            NativeMessage content = JsonSerializer.Deserialize<NativeMessage>(reified, NativeMessageSettings);

            string contentString = this.version switch
            {
                PactSpecification.V3 => ((JsonElement)content.Contents).GetRawText(),
                PactSpecification.V4 => ((JsonElement)content.Contents).GetProperty("content").GetRawText(),
                _ => throw new ArgumentOutOfRangeException(nameof(version), this.version, "Unsupported specification version")
            };

            T messageReified = JsonSerializer.Deserialize<T>(contentString, this.config.DefaultJsonSettings);

            return messageReified;
        }
    }
}
