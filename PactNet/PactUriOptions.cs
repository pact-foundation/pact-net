using System;
using System.Text;

namespace PactNet
{
    public class PactUriOptions
    {
        private const string AuthScheme = "Basic";

        internal string Username { get; }
        internal string Password { get; }
        internal string AuthorizationScheme => AuthScheme;
        internal string AuthorizationValue => Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Username}:{Password}"));

        public PactUriOptions(string username, string password)
        {
            if (String.IsNullOrEmpty(username))
            {
                throw new ArgumentException("username is null or empty.");
            }

            if (username.Contains(":"))
            {
                throw new ArgumentException("username contains a ':' character, which is not allowed.");
            }

            if (String.IsNullOrEmpty(password))
            {
                throw new ArgumentException("password is null or empty.");
            }

            Username = username;
            Password = password;
        }
    }
}
