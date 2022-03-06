using System;
using System.Collections.Generic;
using System.IO;
using PactNet.Exceptions;

namespace PactNet.Verifier
{
    /// <summary>
    /// Provider of the backend verification process
    /// </summary>
    internal interface IVerifierProvider : IDisposable
    {
        /// <summary>
        /// Initialise the verifier
        /// </summary>
        void Initialise();

        /// <summary>
        /// Set provider info
        /// </summary>
        /// <param name="name">Name of the provider</param>
        /// <param name="scheme">Provider URI scheme</param>
        /// <param name="host">Provider URI host</param>
        /// <param name="port">Provider URI port</param>
        /// <param name="path">Provider URI path</param>
        void SetProviderInfo(string name, string scheme, string host, ushort port, string path);

        /// <summary>
        /// Set filter info. Null arguments indicate the option is unused
        /// </summary>
        /// <param name="description">Filter by description</param>
        /// <param name="state">Filter by provider state</param>
        /// <param name="noState">Filter to only interactions with (false) or without (true) provider state</param>
        void SetFilterInfo(string description = null, string state = null, bool? noState = null);

        /// <summary>
        /// Set the provider state endpoint
        /// </summary>
        /// <param name="url">URL of the endpoint</param>
        /// <param name="teardown">Invoke a teardown to the provider state endpoint after each interaction</param>
        /// <param name="body">Use request body for provider state requests instead of query params</param>
        void SetProviderState(Uri url, bool teardown, bool body);

        /// <summary>
        /// Set verification options
        /// </summary>
        /// <param name="disableSslVerification">Disable SSL verification</param>
        /// <param name="requestTimeout">Request timeout</param>
        void SetVerificationOptions(bool disableSslVerification,
                                    TimeSpan requestTimeout);

        /// <summary>
        /// Set publish options
        /// </summary>
        /// <param name="providerVersion">Provider version</param>
        /// <param name="buildUrl">URL of the build that ran the verification</param>
        /// <param name="providerTags">Provider tags</param>
        /// <param name="providerBranch">Provider branch</param>
        void SetPublishOptions(string providerVersion,
                               Uri buildUrl,
                               ICollection<string> providerTags,
                               string providerBranch);

        /// <summary>
        /// Set consumer filters
        /// </summary>
        /// <param name="consumerFilters">Consumer filters</param>
        void SetConsumerFilters(ICollection<string> consumerFilters);

        /// <summary>
        /// Add a header which will be used in all calls from the verifier to the provider, for example
        /// an Authorization header with a valid auth token
        /// </summary>
        /// <param name="name">Header name</param>
        /// <param name="value">Header value</param>
        /// <returns>Fluent builder</returns>
        void AddCustomHeader(string name, string value);

        /// <summary>
        /// Add a file source
        /// </summary>
        /// <param name="file">File</param>
        void AddFileSource(FileInfo file);

        /// <summary>
        /// Add a directory source
        /// </summary>
        /// <param name="directory">Directory</param>
        /// <remarks>Can be used with <see cref="SetConsumerFilters"/> to filter the files in the directory</remarks>
        void AddDirectorySource(DirectoryInfo directory);

        /// <summary>
        /// Add a URL source
        /// </summary>
        /// <param name="url">URL to the pact file</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="token">Authentication token</param>
        void AddUrlSource(Uri url, string username, string password, string token);

        /// <summary>
        /// Add a pact broker source
        /// </summary>
        /// <param name="url">Pact broker URL</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="token">Authentication token</param>
        /// <param name="enablePending">Enable pending pacts</param>
        /// <param name="includeWipPactsSince">Include WIP pacts since this date</param>
        /// <param name="providerTags">Provider tags</param>
        /// <param name="providerBranch">Provider branch</param>
        /// <param name="consumerVersionSelectors">Consumer version selectors</param>
        /// <param name="consumerVersionTags">Consumer version tags</param>
        void AddBrokerSource(Uri url,
                             string username,
                             string password,
                             string token,
                             bool enablePending,
                             DateTime? includeWipPactsSince,
                             ICollection<string> providerTags,
                             string providerBranch,
                             ICollection<string> consumerVersionSelectors,
                             ICollection<string> consumerVersionTags);

        /// <summary>
        /// Verify the pact from the given args
        /// </summary>
        /// <exception cref="PactFailureException">Verification failed</exception>
        void Execute();
    }
}
