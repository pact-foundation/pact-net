using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using NSubstitute;
using PactNet.Mocks.MockHttpService.Mappers;
using PactNet.Mocks.MockHttpService.Models;
using Xunit;

namespace PactNet.Tests.Mocks.MockHttpService.Mappers
{
    public class HttpRequestMessageMapperTests
    {
        public IHttpRequestMessageMapper GetSubject()
        {
            return new HttpRequestMessageMapper();
        }

        [Fact]
        public void Convert_WithNullRequest_ReturnsNull()
        {
            var mapper = GetSubject();

            var result = mapper.Convert(null);

            Assert.Null(result);
        }

        [Fact]
        public void Convert_WithRequest_CallsHttpMethodMapper()
        {
            var request = new PactProviderServiceRequest
            {
                Method = HttpVerb.Post,
                Path = "/events"
            };

            var mockHttpMethodMapper = Substitute.For<IHttpMethodMapper>();
            var mockHttpContentMapper = Substitute.For<IHttpContentMapper>();
            var mockEncodingMapper = Substitute.For<IEncodingMapper>();

            mockHttpMethodMapper.Convert(HttpVerb.Post).Returns(HttpMethod.Post);

            IHttpRequestMessageMapper mapper = new HttpRequestMessageMapper(
                mockHttpMethodMapper, 
                mockHttpContentMapper, 
                mockEncodingMapper);

            mapper.Convert(request);

            mockHttpMethodMapper.Received(1).Convert(request.Method);
        }

        [Fact]
        public void Convert_WithHeader_HeaderIsAddedToHttpRequestMessage()
        {
            var request = new PactProviderServiceRequest
            {
                Method = HttpVerb.Post,
                Path = "/events",
                Headers = new Dictionary<string, string>
                {
                    { "X-Custom", "Tester" }
                }
            };

            var mockHttpMethodMapper = Substitute.For<IHttpMethodMapper>();
            var mockHttpContentMapper = Substitute.For<IHttpContentMapper>();
            var mockEncodingMapper = Substitute.For<IEncodingMapper>();

            mockHttpMethodMapper.Convert(HttpVerb.Post).Returns(HttpMethod.Post);

            IHttpRequestMessageMapper mapper = new HttpRequestMessageMapper(
                mockHttpMethodMapper,
                mockHttpContentMapper,
                mockEncodingMapper);

            var result = mapper.Convert(request);

            Assert.Equal(request.Headers.First().Key, result.Headers.First().Key);
            Assert.Equal(request.Headers.First().Value, result.Headers.First().Value.First());
        }

        [Fact]
        public void Convert_WithPlainContentTypeHeader_HeaderIsNotAddedToHttpRequestMessageAndHttpContentMapperIsCalledWithContentType()
        {
            var request = new PactProviderServiceRequest
            {
                Method = HttpVerb.Post,
                Path = "/events",
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "text/plain" }
                },
                Body = new {}
            };

            var mockHttpMethodMapper = Substitute.For<IHttpMethodMapper>();
            var mockHttpContentMapper = Substitute.For<IHttpContentMapper>();
            var mockEncodingMapper = Substitute.For<IEncodingMapper>();

            mockHttpMethodMapper.Convert(HttpVerb.Post).Returns(HttpMethod.Post);

            IHttpRequestMessageMapper mapper = new HttpRequestMessageMapper(
                mockHttpMethodMapper,
                mockHttpContentMapper,
                mockEncodingMapper);

            var result = mapper.Convert(request);

            Assert.Empty(result.Headers);
            mockHttpContentMapper.Received(1).Convert(request.Body, null, "text/plain");
        }

        [Fact]
        public void Convert_WithPlainContentTypeHeaderLowercased_HeaderIsNotAddedToHttpRequestMessageAndHttpContentMapperIsCalledWithContentType()
        {
            var request = new PactProviderServiceRequest
            {
                Method = HttpVerb.Post,
                Path = "/events",
                Headers = new Dictionary<string, string>
                {
                    { "content-type", "text/plain" }
                },
                Body = new { }
            };

            var mockHttpMethodMapper = Substitute.For<IHttpMethodMapper>();
            var mockHttpContentMapper = Substitute.For<IHttpContentMapper>();
            var mockEncodingMapper = Substitute.For<IEncodingMapper>();

            mockHttpMethodMapper.Convert(HttpVerb.Post).Returns(HttpMethod.Post);

            IHttpRequestMessageMapper mapper = new HttpRequestMessageMapper(
                mockHttpMethodMapper,
                mockHttpContentMapper,
                mockEncodingMapper);

            var result = mapper.Convert(request);

            Assert.Empty(result.Headers);
            mockEncodingMapper.Received(0).Convert(Arg.Any<string>());
            mockHttpContentMapper.Received(1).Convert(request.Body, null, "text/plain");
        }

        [Fact]
        public void Convert_WithPlainContentTypeAndUtf8CharsetHeader_HeaderIsNotAddedToHttpRequestMessageAndHttpContentMapperIsCalledWithEncodingAndContentType()
        {
            var request = new PactProviderServiceRequest
            {
                Method = HttpVerb.Post,
                Path = "/events",
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "text/plain; charset=utf-8" }
                },
                Body = new { }
            };

            var mockHttpMethodMapper = Substitute.For<IHttpMethodMapper>();
            var mockHttpContentMapper = Substitute.For<IHttpContentMapper>();
            var mockEncodingMapper = Substitute.For<IEncodingMapper>();

            mockHttpMethodMapper.Convert(HttpVerb.Post).Returns(HttpMethod.Post);
            mockEncodingMapper.Convert("utf-8").Returns(Encoding.UTF8);

            IHttpRequestMessageMapper mapper = new HttpRequestMessageMapper(
                mockHttpMethodMapper,
                mockHttpContentMapper,
                mockEncodingMapper);

            var result = mapper.Convert(request);

            Assert.Empty(result.Headers);
            mockEncodingMapper.Received(1).Convert("utf-8");
            mockHttpContentMapper.Received(1).Convert(request.Body, Encoding.UTF8, "text/plain");
        }

        [Fact]
        public void Convert_WithJsonContentTypeAndUnicodeCharsetHeader_HeaderIsNotAddedToHttpRequestMessageAndHttpContentMapperIsCalledWithEncodingAndContentType()
        {
            var request = new PactProviderServiceRequest
            {
                Method = HttpVerb.Post,
                Path = "/events",
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json; charset=utf-16" }
                },
                Body = new { }
            };

            var mockHttpMethodMapper = Substitute.For<IHttpMethodMapper>();
            var mockHttpContentMapper = Substitute.For<IHttpContentMapper>();
            var mockEncodingMapper = Substitute.For<IEncodingMapper>();

            mockHttpMethodMapper.Convert(HttpVerb.Post).Returns(HttpMethod.Post);
            mockEncodingMapper.Convert("utf-16").Returns(Encoding.UTF8);

            IHttpRequestMessageMapper mapper = new HttpRequestMessageMapper(
                mockHttpMethodMapper,
                mockHttpContentMapper,
                mockEncodingMapper);

            var result = mapper.Convert(request);

            Assert.Empty(result.Headers);
            mockEncodingMapper.Received(1).Convert("utf-16");
            mockHttpContentMapper.Received(1).Convert(request.Body, Encoding.UTF8, "application/json");
        }

        [Fact]
        public void Convert_WithContentTypeAndCustomHeader_OnlyCustomHeadersIsAddedToHttpRequestMessage()
        {
            var request = new PactProviderServiceRequest
            {
                Method = HttpVerb.Post,
                Path = "/events",
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "text/plain" },
                    { "X-Custom", "My Custom header" }
                },
                Body = new { }
            };

            var mockHttpMethodMapper = Substitute.For<IHttpMethodMapper>();
            var mockHttpContentMapper = Substitute.For<IHttpContentMapper>();
            var mockEncodingMapper = Substitute.For<IEncodingMapper>();

            mockHttpMethodMapper.Convert(HttpVerb.Post).Returns(HttpMethod.Post);

            IHttpRequestMessageMapper mapper = new HttpRequestMessageMapper(
                mockHttpMethodMapper,
                mockHttpContentMapper,
                mockEncodingMapper);

            var result = mapper.Convert(request);

            Assert.Equal(request.Headers.Last().Key, result.Headers.First().Key);
            Assert.Equal(request.Headers.Last().Value, result.Headers.First().Value.First());
        }

        [Fact]
        public void Convert_WithBody_HttpContentMapperIsCalled()
        {
            var request = new PactProviderServiceRequest
            {
                Method = HttpVerb.Get,
                Path = "/events",
                Body = new
                {
                    Test = "tester"
                }
            };

            var mockHttpMethodMapper = Substitute.For<IHttpMethodMapper>();
            var mockHttpContentMapper = Substitute.For<IHttpContentMapper>();
            var mockEncodingMapper = Substitute.For<IEncodingMapper>();

            mockHttpMethodMapper.Convert(HttpVerb.Get).Returns(HttpMethod.Get);

            IHttpRequestMessageMapper mapper = new HttpRequestMessageMapper(
                mockHttpMethodMapper,
                mockHttpContentMapper,
                mockEncodingMapper);

            mapper.Convert(request);

            mockHttpContentMapper.Received(1).Convert(request.Body, null, null);
        }

        [Fact]
        public void Convert_WithTheWorks_CorrectlyMappedHttpRequestMessageIsReturned()
        {
            const string encodingString = "utf-8";
            var encoding = Encoding.UTF8;
            const string contentTypeString = "application/json";
            const string bodyJson = "{\"Test\":\"tester\",\"Testing\":1}";

            var request = new PactProviderServiceRequest
            {
                Method = HttpVerb.Get,
                Path = "/events",
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", contentTypeString + "; charset=" + encodingString },
                    { "X-Custom", "My Custom header" },
                    { "Content-Length", "10000" }, //This header is removed and replace with the correct value of 29
                },
                Body = new
                {
                    Test = "tester",
                    Testing = 1
                }
            };

            var mockHttpMethodMapper = Substitute.For<IHttpMethodMapper>();
            var mockHttpContentMapper = Substitute.For<IHttpContentMapper>();
            var mockEncodingMapper = Substitute.For<IEncodingMapper>();

            mockHttpMethodMapper.Convert(HttpVerb.Get).Returns(HttpMethod.Get);
            mockHttpContentMapper.Convert(Arg.Any<object>(), encoding, contentTypeString).Returns(new StringContent(bodyJson, encoding, contentTypeString));
            mockEncodingMapper.Convert(encodingString).Returns(Encoding.UTF8);

            IHttpRequestMessageMapper mapper = new HttpRequestMessageMapper(
                mockHttpMethodMapper,
                mockHttpContentMapper,
                mockEncodingMapper);

            var result = mapper.Convert(request);
            var requestContent = result.Content.ReadAsStringAsync().Result;

            var contentTypeHeader = result.Content.Headers.First(x => x.Key.Equals("Content-Type"));
            var customHeader = result.Headers.First(x => x.Key.Equals("X-Custom"));
            var contentLengthHeader = result.Content.Headers.First(x => x.Key.Equals("Content-Length"));

            Assert.Equal(bodyJson, requestContent);

            //Content-Type header
            Assert.Equal(request.Headers.First().Key, contentTypeHeader.Key);
            Assert.Equal(request.Headers.First().Value, contentTypeHeader.Value.First());

            //X-Custom header
            Assert.Equal(request.Headers.Skip(1).First().Key, customHeader.Key);
            Assert.Equal(request.Headers.Skip(1).First().Value, customHeader.Value.First());

            //Content-Length header
            Assert.Equal(request.Headers.Last().Key, contentLengthHeader.Key);
            Assert.Equal("29", contentLengthHeader.Value.First());
        }
    }
}
