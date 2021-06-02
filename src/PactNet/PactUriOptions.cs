using System;

namespace PactNet
{
    /// <summary>
    /// Pact URI options
    /// </summary>
    public class PactUriOptions
    {
        /// <summary>
        /// Authentication username
        /// </summary>
        public string Username { get; }

        /// <summary>
        /// Authentication password
        /// </summary>
        public string Password { get; }

        /// <summary>
        /// Authentication token
        /// </summary>
        public string Token { get; }

        /// <summary>
        /// Initialises a new instance of the <see cref="PactUriOptions"/> class using basic authentication.
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        public PactUriOptions(string username, string password)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException($"{nameof(username)} is null or empty.");
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException($"{nameof(password)} is null or empty.");
            }

            if (username.Contains(":"))
            {
                throw new ArgumentException($"{nameof(username)} contains a ':' character, which is not allowed.");
            }

            this.Username = username;
            this.Password = password;
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="PactUriOptions"/> class using bearer token authentication.
        /// </summary>
        /// <param name="token">Bearer token</param>
        public PactUriOptions(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentException($"{nameof(token)} is null or empty.");
            }

            this.Token = token;
        }
    }
}
