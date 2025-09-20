using System;

namespace PactNet.Interop.Drivers
{
    /// <summary>
    /// Driver for managing a HTTP mock server
    /// </summary>
    internal interface IMockServerDriver : IDisposable
    {
        /// <summary>
        /// Mock server URI
        /// </summary>
        Uri Uri { get; }

        /// <summary>
        /// Mock server port
        /// </summary>
        int Port { get; }

        /// <summary>
        /// Get a string representing the mismatches following interaction testing
        /// </summary>
        /// <returns>Mismatch string</returns>
        string MockServerMismatches();

        /// <summary>
        /// Get a string representing the mock server logs following interaction testing
        /// </summary>
        /// <returns>Log string</returns>
        string MockServerLogs();

        /// <summary>
        /// Returns a boolean value of true if all the expectations of the pact that the mock server was created with have been met.
        /// It will return false if any request did not match, an un-recognised request was received or an expected request was not received.
        /// </summary>
        bool MockServerMatched();
    }
}
