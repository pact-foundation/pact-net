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

        private IHttpRequestSender GetSubject(FakeHttpClient fakeHttpClient = null)
        {
            _fakeHttpClient = fakeHttpClient ?? new FakeHttpClient();
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
            var requestSender = GetSubject(new FakeHttpClient(response));

            _mockHttpRequestMessageMapper.Convert(Arg.Any<ProviderServiceRequest>())
                .Returns(new HttpRequestMessage());

            requestSender.Send(new ProviderServiceRequest());

            _mockProviderServiceResponseMapper.Received(1).Convert(response);
        }

        [Fact]
        public void Send_WhenBaseAddressHasATrailingSlash_TheLeadingSlashOnTheRequestPathIsRemoved()
        {
            var request = new ProviderServiceRequest
            {
                Path = "/testing/hi"
            };

            var requestSender = GetSubject(new FakeHttpClient(baseAddress: "http://localhost/api/v2/"));

            requestSender.Send(request);

            _mockHttpRequestMessageMapper.Received(1).Convert(Arg.Is<ProviderServiceRequest>(x => x.Path == "testing/hi"));
            Assert.Equal("testing/hi", request.Path);
        }

        [Fact]
        public void Send_WhenBaseAddressDoesNotHaveATrailingSlash_ThePathIsNotAltered()
        {
            const string path = "/testing/hi";
            var request = new ProviderServiceRequest
            {
                Path = path
            };

            var requestSender = GetSubject(new FakeHttpClient(baseAddress: "http://my-hostname:1234"));

            requestSender.Send(request);

            _mockHttpRequestMessageMapper.Received(1).Convert(Arg.Is<ProviderServiceRequest>(x => x.Path == path));
            Assert.Equal(path, request.Path);
        }
    }
}
