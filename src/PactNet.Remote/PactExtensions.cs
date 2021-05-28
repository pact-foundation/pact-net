using System;

namespace PactNet.Remote
{
    /// <summary>
    /// Extensions for <see cref="IPact"/>
    /// </summary>
    public static class PactExtensions
    {
        /// <summary>
        /// Use an existing remote server running at the given URI
        /// </summary>
        /// <param name="pact">Pact details</param>
        /// <param name="uri">Remote server URI</param>
        /// <returns>Interaction builder</returns>
        public static IPactBuilder UsingRemoteBackend(this IPact pact, Uri uri)
        {
            var builder = new RemotePactBuilder(pact.Consumer, pact.Provider, uri, pact.Config);
            return builder;
        }
    }
}