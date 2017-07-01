using System;
using System.Text;

namespace PactNet
{
    public class PactUriOptions
    {
        private const string AuthScheme = "Basic";
        private readonly string _username;
        private readonly string _password;

        internal string AuthorizationScheme
        {
            get { return AuthScheme; }
        }

        internal string AuthorizationValue
        {
            get
            {
                return Convert.ToBase64String(
                    Encoding.UTF8.GetBytes(
                    String.Format("{0}:{1}", _username, _password)));
            }
        }

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

            _username = username;
            _password = password;
        }
    }
}
