using System;

namespace PactNet.Schemas.Interfaces
{
    public interface IPactSchemaVerifier
    {
        /// <summary>
        /// Define a set up and/or tear down action for a specific state specified by the consumer.
        /// This is where you should set up test data, so that you can fulfil the contract outlined by a consumer.
        /// </summary>
        /// <param name="providerState">The name of the provider state as defined by the consumer interaction, which lives in the Pact file.</param>
        /// <param name="setUp">A set up action that will be run before the interaction verify, if the provider has specified it in the interaction. If no action is required please use an empty lambda () => {}.</param>
        /// <param name="tearDown">A tear down action that will be run after the interaction verify, if the provider has specified it in the interaction. If no action is required please use an empty lambda () => {}.</param>
        IPactSchemaVerifier ProviderState(string providerState, Action setUp = null, Action tearDown = null);
        IPactSchemaVerifier HonoursPactWith(string consumerName);
        IPactSchemaVerifier PactUri(string uri, PactUriOptions options = null);
        void Verify(string description = null, string providerState = null);
    }
}
