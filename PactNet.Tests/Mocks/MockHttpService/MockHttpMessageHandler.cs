using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PactNet.Tests.Mocks.MockHttpService
{
    public class MockHttpMessageHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
           var response = File.ReadAllText("../../../Samples/EventApi/Consumer.Tests/pacts/consumer-event_api.json");
           var task = new Task<HttpResponseMessage>(() =>
                new HttpResponseMessage
                {
                    Content = new StringContent(response, Encoding.Default, "application/json"),
                    StatusCode = HttpStatusCode.OK,
                    ReasonPhrase = "OK"
                });
            task.Start();
            return task;
        }
    }
}
