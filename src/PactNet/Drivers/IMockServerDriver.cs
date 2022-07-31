using System;

namespace PactNet.Drivers
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
        /// Get a string representing the mismatches following interaction testing
        /// </summary>
        /// <returns>Mismatch string</returns>
        string MockServerMismatches();

        /// <summary>
        /// Get a string representing the mock server logs following interaction testing
        /// </summary>
        /// <returns>Log string</returns>
        string MockServerLogs();
    }
}
