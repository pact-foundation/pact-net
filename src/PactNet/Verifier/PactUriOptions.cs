using System;
using PactNet.Internal;

namespace PactNet.Verifier
{
    /// <summary>
    /// Options for URI pact sources
    /// </summary>
    internal class PactUriOptions : IPactUriOptions
    {
        private readonly IVerifierProvider provider;
        private readonly Uri uri;

        private string username;
        private string password;
        private string token;

        /// <summary>
        /// Initialises a new instance of the <see cref="PactUriOptions"/> class.
        /// </summary>
        /// <param name="provider">Pact verifier provider</param>
        /// <param name="uri">Pact file URI</param>
        public PactUriOptions(IVerifierProvider provider, Uri uri)
        {
            this.provider = provider;
            this.uri = uri;
        }

        /// <summary>
        /// Use Basic authentication to access the URI
        /// </summary>
        /// <param name="username">Pact broker username</param>
        /// <param name="password">Pact broker password</param>
        /// <returns>Fluent builder</returns>
        public IPactUriOptions BasicAuthentication(string username, string password)
        {
            Guard.NotNullOrEmpty(username, nameof(username));
            Guard.NotNullOrEmpty(password, nameof(password));

            this.username = username;
            this.password = password;

            return this;
        }

        /// <summary>
        /// Use Token authentication to access the URI
        /// </summary>
        /// <param name="token">Auth token</param>
        /// <returns>Fluent builder</returns>
        public IPactUriOptions TokenAuthentication(string token)
        {
            Guard.NotNullOrEmpty(token, nameof(token));

            this.token = token;

            return this;
        }

        /// <summary>
        /// Apply the configured options
        /// </summary>
        public void Apply() => this.provider.AddUrlSource(this.uri, this.username, this.password, this.token);
    }
}
