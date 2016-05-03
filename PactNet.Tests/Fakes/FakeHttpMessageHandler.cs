using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PactNet.Tests.Fakes
{
    public class FakeHttpMessageHandler : DelegatingHandler
    {
        private readonly IList<HttpRequestMessage> _requestsReceived;
        public IEnumerable<HttpRequestMessage> RequestsReceived { get { return _requestsReceived; } }

        private readonly IList<string> _requestContentReceived;
        public IEnumerable<string> RequestContentReceived { get { return _requestContentReceived; } }

        public HttpResponseMessage Response { get; set; }

        public FakeHttpMessageHandler(HttpResponseMessage response = null)
        {
            Response = response ?? new HttpResponseMessage(HttpStatusCode.OK);
            _requestsReceived = new List<HttpRequestMessage>();
            _requestContentReceived = new List<string>();
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _requestsReceived.Add(request);
            if (request.Content != null)
            {
                _requestContentReceived.Add(request.Content.ReadAsStringAsync().Result);
            }

            var tcs = new TaskCompletionSource<HttpResponseMessage>();
            tcs.SetResult(Response);

            return tcs.Task;
        }
    }
}