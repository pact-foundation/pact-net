using System;

namespace PactNet
{
    public class PactUriOptions
    {
        public string Username { get; private set; }
        public string Password { get; private set; }
        public string Token { get; private set;  }

        public PactUriOptions() { }

        public PactUriOptions(string username, string password)
        {
            SetBasicAuthenticationInternal(username, password);
        }

        public PactUriOptions(string token)
        {
            SetBearerAuthentication(token);
        }

        public PactUriOptions SetBasicAuthentication(string username, string password)
        {
            if (!string.IsNullOrEmpty(Token))
            {
                throw new InvalidOperationException("You can't set both bearer and basic authentication at the same time");
            }
            SetBasicAuthenticationInternal(username, password);
            return this;
        }

        public PactUriOptions SetBearerAuthentication(string token)
        {
            if (!string.IsNullOrEmpty(Username) || !string.IsNullOrEmpty(Password))
            {
                throw new InvalidOperationException("You can't set both bearer and basic authentication at the same time");
            }
            SetBearerAuthenticationInternal(token);
            return this;
        }

        private void SetBasicAuthenticationInternal(string username, string password)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException($"{nameof(username)} is null or empty.");
            }

            if (username.Contains(":"))
            {
                throw new ArgumentException($"{nameof(username)} contains a ':' character, which is not allowed.");
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException($"{nameof(password)} is null or empty.");
            }

            Username = username;
            Password = password;
        }

        private void SetBearerAuthenticationInternal(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentException($"{nameof(token)} is null or empty.");
            }

            Token = token;
        }
    }
}