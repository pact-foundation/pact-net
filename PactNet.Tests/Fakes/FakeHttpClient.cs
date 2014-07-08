using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PactNet.Tests.Fakes
{
    public class FakeHttpClient : HttpClient
    {
        public int SendAsyncCallCount { get; private set; }
        private readonly HttpResponseMessage _response;

        public FakeHttpClient(HttpResponseMessage response = null)
        {
            _response = response ?? new HttpResponseMessage(HttpStatusCode.OK);
        }

        public override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            SendAsyncCallCount++;

            var tcs = new TaskCompletionSource<HttpResponseMessage>();
            tcs.SetResult(_response);

            return tcs.Task;
        }
    }
}
