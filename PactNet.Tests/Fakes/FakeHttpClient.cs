using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PactNet.Tests.Fakes
{
    public class FakeHttpClient : HttpClient
    {
        private readonly IList<HttpRequestMessage> _callInfo;
        public IEnumerable<HttpRequestMessage> CallInfo { get { return _callInfo; } }

        public HttpResponseMessage Response { get; set; }

        public FakeHttpClient(HttpResponseMessage response = null)
        {
            Response = response ?? new HttpResponseMessage(HttpStatusCode.OK);
            _callInfo = new List<HttpRequestMessage>();
        }

        public override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _callInfo.Add(request);

            var tcs = new TaskCompletionSource<HttpResponseMessage>();
            tcs.SetResult(Response);

            return tcs.Task;
        }
    }
}
