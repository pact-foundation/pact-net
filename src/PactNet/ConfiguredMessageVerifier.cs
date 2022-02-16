using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PactNet.Exceptions;
using PactNet.Interop;
using PactNet.Models;

namespace PactNet
{
    /// <summary>
    /// Verifies a configured message interaction
    /// </summary>
    public class ConfiguredMessageVerifier : IConfiguredMessageVerifier
    {
        private readonly MessageHandle message;
        private readonly IMessageMockServer server;
        private readonly MessagePactHandle pact;
        private readonly PactConfig config;

        /// <summary>
        /// Initialises a new instance of the <see cref="ConfiguredMessageVerifier"/>
        /// </summary>
        /// <param name="server">Message server</param>
        /// <param name="pact">Pact handle</param>
        /// <param name="message">Message handle</param>
        /// <param name="config">Pact configuration</param>
        internal ConfiguredMessageVerifier(IMessageMockServer server, MessagePactHandle pact, MessageHandle message, PactConfig config)
        {
            this.server = server;
            this.pact = pact;
            this.message = message;
            this.config = config;
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

                this.WritePact();
            }
            catch (Exception e)
            {
                throw new PactMessageConsumerVerificationException($"The message {this.message} could not be verified by the consumer handler", e);
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

                this.WritePact();
            }
            catch (Exception e)
            {
                throw new PactMessageConsumerVerificationException($"The message {this.message} could not be verified by the consumer handler", e);
            }
        }

        /// <summary>
        /// Try to read the reified message
        /// </summary>
        /// <typeparam name="T">the type of message</typeparam>
        /// <returns>the message</returns>
        private T MessageReified<T>()
        {
            string reified = this.server.Reify(this.message);
            NativeMessage content = JsonConvert.DeserializeObject<NativeMessage>(reified);

            T messageReified = JsonConvert.DeserializeObject<T>(content.Contents.ToString());
            return messageReified;
        }

        /// <summary>
        /// Write the pact file
        /// </summary>
        private void WritePact()
        {
            this.server.WriteMessagePactFile(this.pact, this.config.PactDir, false);
        }
    }
}
