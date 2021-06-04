using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Security.DataProtection;

namespace Provider.Api.Web.Tests
{
    public class AuthorizationTokenReplacementMiddleware
    {
        private const string AuthorizationKey = "Authorization";
        private readonly Func<IDictionary<string, object>, Task> _next;
        private readonly TokenGenerator _tokenGenerator;

        public AuthorizationTokenReplacementMiddleware(Func<IDictionary<string, object>, Task> next, IDataProtector dataProtector)
        {
            _next = next;
            _tokenGenerator = new TokenGenerator(dataProtector);
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            IOwinContext context = new OwinContext(environment);

            if (context.Request.Headers != null &&
                context.Request.Headers.ContainsKey(AuthorizationKey) &&
                context.Request.Headers[AuthorizationKey] == "Bearer SomeValidAuthToken")
            {
                context.Request.Headers[AuthorizationKey] = $"Bearer {_tokenGenerator.Generate()}";
            }

            await _next.Invoke(context.Environment);
        }
    }
}