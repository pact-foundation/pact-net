using System;
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
        private FakeHttpMessageHandler _fakeHttpMessageHandler;
        private IHttpRequestMessageMapper _mockHttpRequestMessageMapper;
        private IProviderServiceResponseMapper _mockProviderServiceResponseMapper;

        private IHttpRequestSender GetSubject(string baseAddress = "http://localhost")
        {
            _fakeHttpMessageHandler = new FakeHttpMessageHandler();
            _mockHttpRequestMessageMapper = Substitute.For<IHttpRequestMessageMapper>();
            _mockProviderServiceResponseMapper = Substitute.For<IProviderServiceResponseMapper>();

            var httpClient = new HttpClient(_fakeHttpMessageHandler);
            if (baseAddress != null)
            {
                httpClient.BaseAddress = new Uri(baseAddress);
            }

            return new HttpClientRequestSender(
                httpClient,
                _mockHttpRequestMessageMapper,
                _mockProviderServiceResponseMapper);
        }

        [Fact]
        public void Send_WhenCalled_CallsConvertOnHttpRequestMessageMapper()
        {
            var request = new ProviderServiceRequest();
            var requestSender = GetSubject();

            _mockHttpRequestMessageMapper.Convert(Arg.Any<ProviderServiceRequest>()).Returns(new HttpRequestMessage());

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

            Assert.Equal(1, _fakeHttpMessageHandler.RequestsReceived.Count());
            Assert.Equal(convertedRequest, _fakeHttpMessageHandler.RequestsReceived.First());
        }

        [Fact]
        public void Send_WhenCalled_CallsConvertOnProviderServiceResponseMapper()
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            var requestSender = GetSubject();

            _fakeHttpMessageHandler.Response = response;

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

            var requestSender = GetSubject("http://localhost/api/v2/");

            _mockHttpRequestMessageMapper.Convert(Arg.Any<ProviderServiceRequest>()).Returns(new HttpRequestMessage());

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

            var requestSender = GetSubject("http://my-hostname:1234");

            _mockHttpRequestMessageMapper.Convert(Arg.Any<ProviderServiceRequest>()).Returns(new HttpRequestMessage());

            requestSender.Send(request);

            _mockHttpRequestMessageMapper.Received(1).Convert(Arg.Is<ProviderServiceRequest>(x => x.Path == path));
            Assert.Equal(path, request.Path);
        }

        [Fact]
        public void Send_WhenBaseAddressIsNull_ThePathIsNotAltered()
        {
            const string path = "/testing/hi";
            var request = new ProviderServiceRequest
            {
                Path = path
            };

            var requestSender = GetSubject(null);

            _mockHttpRequestMessageMapper.Convert(Arg.Any<ProviderServiceRequest>()).Returns(new HttpRequestMessage(HttpMethod.Get, "http://tester/"));

            requestSender.Send(request);

            _mockHttpRequestMessageMapper.Received(1).Convert(Arg.Is<ProviderServiceRequest>(x => x.Path == path));
            Assert.Equal(path, request.Path);
        }
    }
}
