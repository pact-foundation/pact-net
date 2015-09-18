using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PactNet
{
    public class PactUriOptions
    {
        public PactUriOptions(string basicAuthUserName, string basicAuthPassword)
        {
            if (String.IsNullOrEmpty(basicAuthUserName) || String.IsNullOrEmpty(basicAuthPassword))
                throw new ApplicationException("Invalid username or password");

            this.BasicAuthUserName = basicAuthUserName;
            this.BasicAuthPassword = basicAuthPassword;
        }

        public string BasicAuthUserName { get; private set; }
        public string BasicAuthPassword { get; private set; }
    }
}
