using System;
using System.Text;

namespace PactNet
{
    public class PactUriOptions
    {
        public string Username { get; }
        public string Password { get; }
        public string Token { get; }
        public string AuthorizationScheme { get; }
        public string AuthorizationValue { get; }

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
            AuthorizationScheme = "Basic";
            AuthorizationValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Username}:{Password}"));
        }

        public PactUriOptions(string token)
        {
            if (String.IsNullOrEmpty(token))
            {
                throw new ArgumentException("token is null or empty.");
            }

            Token = token;
            AuthorizationScheme = "Bearer";
            AuthorizationValue = token;
        }
    }
}
