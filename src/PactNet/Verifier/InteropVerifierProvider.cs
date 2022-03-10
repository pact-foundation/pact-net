using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using PactNet.Exceptions;
using PactNet.Interop;

namespace PactNet.Verifier
{
    /// <summary>
    /// Native pact verifier
    /// </summary>
    internal class InteropVerifierProvider : IVerifierProvider
    {
        private readonly PactVerifierConfig config;

        private IntPtr handle;

        /// <summary>
        /// Initialises a new instance of the <see cref="InteropVerifierProvider"/> class.
        /// </summary>
        /// <param name="config">Pact verifier config</param>
        public InteropVerifierProvider(PactVerifierConfig config)
        {
            this.config = config;
        }

        /// <summary>
        /// Initialise the verifier
        /// </summary>
        public void Initialise()
        {
            NativeInterop.LogToBuffer(config.LogLevel switch
            {
                PactLogLevel.Trace => LevelFilter.Trace,
                PactLogLevel.Debug => LevelFilter.Debug,
                PactLogLevel.Information => LevelFilter.Info,
                PactLogLevel.Warn => LevelFilter.Warn,
                PactLogLevel.Error => LevelFilter.Error,
                PactLogLevel.None => LevelFilter.Off,
                _ => throw new ArgumentOutOfRangeException(nameof(config.LogLevel), config.LogLevel, "Invalid log level")
            });

            this.handle = NativeInterop.VerifierNewForApplication("pact-net", typeof(InteropVerifierProvider).Assembly.GetName().Version.ToString());
        }

        /// <summary>
        /// Set provider info
        /// </summary>
        /// <param name="name">Name of the provider</param>
        /// <param name="scheme">Provider URI scheme</param>
        /// <param name="host">Provider URI host</param>
        /// <param name="port">Provider URI port</param>
        /// <param name="path">Provider URI path</param>
        public void SetProviderInfo(string name, string scheme, string host, ushort port, string path)
        {
            NativeInterop.VerifierSetProviderInfo(this.handle, name, scheme, host, port, path);
        }

        /// <summary>
        /// Set filter info. Null arguments indicate the option is unused
        /// </summary>
        /// <param name="description">Filter by description</param>
        /// <param name="state">Filter by provider state</param>
        /// <param name="noState">Filter to only interactions with (false) or without (true) provider state</param>
        public void SetFilterInfo(string description = null, string state = null, bool? noState = null)
        {
            NativeInterop.VerifierSetFilterInfo(this.handle, description, state, ToSafeByte(noState));
        }

        /// <summary>
        /// Set the provider state endpoint
        /// </summary>
        /// <param name="url">URL of the endpoint</param>
        /// <param name="teardown">Invoke a teardown to the provider state endpoint after each interaction</param>
        /// <param name="body">Use request body for provider state requests instead of query params</param>
        public void SetProviderState(Uri url, bool teardown, bool body)
        {
            NativeInterop.VerifierSetProviderState(this.handle, url.AbsoluteUri, ToSafeByte(teardown), ToSafeByte(body));
        }

        /// <summary>
        /// Set verification options
        /// </summary>
        /// <param name="disableSslVerification">Disable SSL verification</param>
        /// <param name="requestTimeout">Request timeout</param>
        public void SetVerificationOptions(bool disableSslVerification, TimeSpan requestTimeout)
        {
            uint timeout = Convert.ToUInt32(requestTimeout.TotalMilliseconds);

            NativeInterop.VerifierSetVerificationOptions(this.handle,
                                                         ToSafeByte(disableSslVerification),
                                                         timeout);
        }

        /// <summary>
        /// Set publish options
        /// </summary>
        /// <param name="providerVersion">Provider version</param>
        /// <param name="buildUrl">URL of the build that ran the verification</param>
        /// <param name="providerTags">Provider tags</param>
        /// <param name="providerBranch">Provider branch</param>
        public void SetPublishOptions(string providerVersion, Uri buildUrl, ICollection<string> providerTags, string providerBranch)
        {
            NativeInterop.VerifierSetPublishOptions(this.handle,
                                                    providerVersion,
                                                    buildUrl?.AbsoluteUri,
                                                    providerTags.ToArray(),
                                                    (ushort)providerTags.Count,
                                                    providerBranch);
        }

        /// <summary>
        /// Set consumer filters
        /// </summary>
        /// <param name="consumerFilters">Consumer filters</param>
        public void SetConsumerFilters(ICollection<string> consumerFilters)
        {
            NativeInterop.VerifierSetConsumerFilters(this.handle, consumerFilters.ToArray(), (ushort)consumerFilters.Count);
        }

        /// <summary>
        /// Add a header which will be used in all calls from the verifier to the provider, for example
        /// an Authorization header with a valid auth token
        /// </summary>
        /// <param name="name">Header name</param>
        /// <param name="value">Header value</param>
        /// <returns>Fluent builder</returns>
        public void AddCustomHeader(string name, string value)
        {
            NativeInterop.AddCustomHeader(this.handle, name, value);
        }

        /// <summary>
        /// Add a file source
        /// </summary>
        /// <param name="file">File</param>
        public void AddFileSource(FileInfo file)
        {
            NativeInterop.VerifierAddFileSource(this.handle, file.FullName);
        }

        /// <summary>
        /// Add a directory source
        /// </summary>
        /// <param name="directory">Directory</param>
        /// <remarks>Can be used with <see cref="IVerifierProvider.SetConsumerFilters"/> to filter the files in the directory</remarks>
        public void AddDirectorySource(DirectoryInfo directory)
        {
            NativeInterop.VerifierAddDirectorySource(this.handle, directory.FullName);
        }

        /// <summary>
        /// Add a URL source
        /// </summary>
        /// <param name="url">URL to the pact file</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="token">Authentication token</param>
        public void AddUrlSource(Uri url, string username, string password, string token)
        {
            NativeInterop.VerifierUrlSource(this.handle, url.AbsoluteUri, username, password, token);
        }

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
        public void AddBrokerSource(Uri url,
                                    string username,
                                    string password,
                                    string token,
                                    bool enablePending,
                                    DateTime? includeWipPactsSince,
                                    ICollection<string> providerTags,
                                    string providerBranch,
                                    ICollection<string> consumerVersionSelectors,
                                    ICollection<string> consumerVersionTags)
        {
            NativeInterop.VerifierBrokerSourceWithSelectors(this.handle,
                                                            url.AbsoluteUri,
                                                            username,
                                                            password,
                                                            token,
                                                            ToSafeByte(enablePending),
                                                            includeWipPactsSince?.ToString("yyyy-MM-dd"),
                                                            providerTags.ToArray(),
                                                            (ushort)providerTags.Count,
                                                            providerBranch,
                                                            consumerVersionSelectors.ToArray(),
                                                            (ushort)consumerVersionSelectors.Count,
                                                            consumerVersionTags.ToArray(),
                                                            (ushort)consumerVersionTags.Count);
        }

        /// <summary>
        /// Verify the pact from the given args
        /// </summary>
        /// <exception cref="PactFailureException">Verification failed</exception>
        public void Execute()
        {
            int result = NativeInterop.VerifierExecute(this.handle);

            if (result == 0)
            {
                this.config.WriteLine("Pact verification successful\n");
                this.PrintOutput();
                return;
            }

            this.config.WriteLine("Pact verification failed\n");
            this.PrintOutput();

            string error = result switch
            {
                1 => "Pact verification failed",
                2 => "Failed to run the verification",
                _ => $"An unknown error occurred: {result}"
            };

            throw new PactFailureException(error);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.
        /// </summary>
        ~InteropVerifierProvider()
        {
            this.ReleaseUnmanagedResources();
        }

        /// <summary>
        /// Release unmanaged resources
        /// </summary>
        private void ReleaseUnmanagedResources()
        {
            if (this.handle != IntPtr.Zero)
            {
                NativeInterop.VerifierShutdown(this.handle);
            }

            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// Convert a nullable bool to a byte indicating true/false
        /// </summary>
        /// <param name="value">Bool value</param>
        /// <returns>Byte indicating true or false</returns>
        private static byte ToSafeByte(bool? value)
        {
            return value.GetValueOrDefault(false)
                       ? (byte)1
                       : (byte)0;
        }

        /// <summary>
        /// Print output and logs of the verifier
        /// </summary>
        private void PrintOutput()
        {
            IntPtr outputPtr = NativeInterop.VerifierOutput(this.handle, 1);

            string output = outputPtr == IntPtr.Zero
                                ? "ERROR: Unable to retrieve verifier output"
                                : Marshal.PtrToStringAnsi(outputPtr);

            this.config.WriteLine("Verifier Output");
            this.config.WriteLine("---------------");
            this.config.WriteLine(output);

            IntPtr logsPtr = NativeInterop.VerifierLogs(this.handle);

            string logs = logsPtr == IntPtr.Zero
                              ? "ERROR: Unable to retrieve verifier logs"
                              : Marshal.PtrToStringAnsi(logsPtr);

            this.config.WriteLine("Verifier Logs");
            this.config.WriteLine("-------------");
            this.config.WriteLine(logs);
        }
    }
}
