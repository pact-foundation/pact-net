using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace PactNet.AspNetCore.Tests
{
    public class BaseMiddlewareTest
    {
        public bool NextTriggered { get; set; }

        public RequestDelegate NextHandle => context =>
        {
            NextTriggered = true;
            return Task.FromResult(true);
        };
    }
}
