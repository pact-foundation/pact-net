using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Owin.Security.DataProtection;

namespace Provider.Api.Web.Tests
{
    public class AuthorizationTokenReplacementMiddleware
    {
        private readonly Func<IDictionary<string, object>, Task> _next;
        private readonly TokenGenerator _tokenGenerator;

        public AuthorizationTokenReplacementMiddleware(Func<IDictionary<string, object>, Task> next, IDataProtector dataProtector)
        {
            _next = next;
            _tokenGenerator = new TokenGenerator(dataProtector);
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            var headers = environment["owin.RequestHeaders"] as IDictionary<string, string[]>;

            Debug.Assert(headers != null, "headers != null");
            if (headers.ContainsKey("Authorization") && headers["Authorization"][0] == "Bearer SomeValidAuthToken")
            {
                headers["Authorization"][0] = $"Bearer {_tokenGenerator.Generate()}";
            }

            await _next.Invoke(environment);
        }
    }
}