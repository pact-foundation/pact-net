using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ReadMe.Provider.Tests
{
    public class AuthorizationTokenMiddleware
    {
        private const string Authorization = nameof(Authorization);
        private readonly RequestDelegate next;

        public AuthorizationTokenMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // create a valid key
            string token = TokenGenerator.Generate();
            context.Request.Headers[Authorization] = $"Bearer {token}";

            await next(context);
        }
    }
}
