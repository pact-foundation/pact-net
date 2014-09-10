using System.Linq;
using System.Net;
using System.Net.Http;
using NSubstitute;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Mappers;
using PactNet.Mocks.MockHttpService.Models;
using PactNet.Tests.Fakes;
using Xunit;

namespace PactNet.Tests.Mocks.MockHttpService
{
    public class HttpClientRequestSenderTests
    {
        private FakeHttpClient _fakeHttpClient;
        private IHttpRequestMessageMapper _mockHttpRequestMessageMapper;
        private IProviderServiceResponseMapper _mockProviderServiceResponseMapper;

        private IHttpRequestSender GetSubject(HttpResponseMessage httpResponseMessage = null)
        {
            _fakeHttpClient = new FakeHttpClient(httpResponseMessage);
            _mockHttpRequestMessageMapper = Substitute.For<IHttpRequestMessageMapper>();
            _mockProviderServiceResponseMapper = Substitute.For<IProviderServiceResponseMapper>();

            return new HttpClientRequestSender(
                _fakeHttpClient,
                _mockHttpRequestMessageMapper,
                _mockProviderServiceResponseMapper);
        }

        [Fact]
        public void Send_WhenCalled_CallsConvertOnHttpRequestMessageMapper()
        {
            var request = new ProviderServiceRequest();
            var requestSender = GetSubject();

            requestSender.Send(request);

            _mockHttpRequestMessageMapper.Received(1).Convert(request);
        }

        [Fact]
        public void Send_WhenCalled_CallsSendAsyncOnHttpClient()
        {
            var request = new ProviderServiceRequest();
            var requestSender = GetSubject();
            var convertedRequest = new HttpRequestMessage();

            _mockHttpRequestMessageMapper.Convert(request)
                .Returns(convertedRequest);

            requestSender.Send(request);

            Assert.Equal(1, _fakeHttpClient.RequestsRecieved.Count());
            Assert.Equal(convertedRequest, _fakeHttpClient.RequestsRecieved.First());
        }

        [Fact]
        public void Send_WhenCalled_CallsConvertOnProviderServiceResponseMapper()
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            var requestSender = GetSubject(response);

            _mockHttpRequestMessageMapper.Convert(Arg.Any<ProviderServiceRequest>())
                .Returns(new HttpRequestMessage());

            requestSender.Send(new ProviderServiceRequest());

            _mockProviderServiceResponseMapper.Received(1).Convert(response);
        }
    }
}
