using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using NSubstitute;
using PactNet.Mocks.MockHttpService.Mappers;
using PactNet.Mocks.MockHttpService.Models;
using Xunit;

namespace PactNet.Tests.Mocks.MockHttpService.Mappers
{
    public class HttpRequestMessageMapperTests
    {
        private IHttpMethodMapper _mockHttpMethodMapper;
        private IHttpContentMapper _mockHttpContentMapper;
        private IHttpBodyContentMapper _mockHttpBodyContentMapper;

        private IHttpRequestMessageMapper GetSubject()
        {
            _mockHttpMethodMapper = Substitute.For<IHttpMethodMapper>();
            _mockHttpContentMapper = Substitute.For<IHttpContentMapper>();
            _mockHttpBodyContentMapper = Substitute.For<IHttpBodyContentMapper>();

            return new HttpRequestMessageMapper(_mockHttpMethodMapper, _mockHttpContentMapper, _mockHttpBodyContentMapper);
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
            var request = new ProviderServiceRequest
            {
                Method = HttpVerb.Post,
                Path = "/events"
            };

            var mapper = GetSubject();

            _mockHttpMethodMapper.Convert(HttpVerb.Post).Returns(HttpMethod.Post);

            mapper.Convert(request);

            _mockHttpMethodMapper.Received(1).Convert(request.Method);
        }

        [Fact]
        public void Convert_WithHeader_HeaderIsAddedToHttpRequestMessage()
        {
            var request = new ProviderServiceRequest
            {
                Method = HttpVerb.Post,
                Path = "/events",
                Headers = new Dictionary<string, string>
                {
                    { "X-Custom", "Tester" }
                }
            };

            var mapper = GetSubject();

            _mockHttpMethodMapper.Convert(HttpVerb.Post).Returns(HttpMethod.Post);
            _mockHttpBodyContentMapper.Convert(Arg.Any<object>(), request.Headers).Returns(new HttpBodyContent(content: Encoding.UTF8.GetBytes(String.Empty), contentType: new MediaTypeHeaderValue("text/plain") { CharSet = "utf-8" }));

            var result = mapper.Convert(request);

            Assert.Equal(request.Headers.First().Key, result.Headers.First().Key);
            Assert.Equal(request.Headers.First().Value, result.Headers.First().Value.First());
        }

        [Fact]
        public void Convert_WithPlainContentTypeHeader_HeaderIsNotAddedToHttpRequestMessageAndHttpContentMapperIsCalledWithContentType()
        {
            const string contentTypeString = "text/plain";
            var request = new ProviderServiceRequest
            {
                Method = HttpVerb.Post,
                Path = "/events",
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", contentTypeString }
                },
                Body = new { }
            };
            var httpBodyContent = new HttpBodyContent(content: Encoding.UTF8.GetBytes(String.Empty), contentType: new MediaTypeHeaderValue(contentTypeString) { CharSet = "utf-8" });

            var mapper = GetSubject();

            _mockHttpMethodMapper.Convert(HttpVerb.Post).Returns(HttpMethod.Post);
            _mockHttpBodyContentMapper.Convert(Arg.Any<object>(), request.Headers).Returns(httpBodyContent);

            var result = mapper.Convert(request);

            Assert.Empty(result.Headers);
            _mockHttpContentMapper.Received(1).Convert(httpBodyContent);
        }

        [Fact]
        public void Convert_WithPlainContentTypeHeaderLowercased_HeaderIsNotAddedToHttpRequestMessageAndHttpContentMapperIsCalledWithContentType()
        {
            const string contentTypeString = "text/plain";
            var request = new ProviderServiceRequest
            {
                Method = HttpVerb.Post,
                Path = "/events",
                Headers = new Dictionary<string, string>
                {
                    { "content-type", contentTypeString }
                },
                Body = new { }
            };
            var httpBodyContent = new HttpBodyContent(content: Encoding.UTF8.GetBytes(String.Empty), contentType: new MediaTypeHeaderValue(contentTypeString) { CharSet = "utf-8" });

            var mapper = GetSubject();

            _mockHttpMethodMapper.Convert(HttpVerb.Post).Returns(HttpMethod.Post);
            _mockHttpBodyContentMapper.Convert(Arg.Any<object>(), request.Headers).Returns(httpBodyContent);

            var result = mapper.Convert(request);

            Assert.Empty(result.Headers);
            _mockHttpContentMapper.Received(1).Convert(httpBodyContent);
        }

        [Fact]
        public void Convert_WithPlainContentTypeAndUtf8CharsetHeader_HeaderIsNotAddedToHttpRequestMessageAndHttpContentMapperIsCalledWithEncodingAndContentType()
        {
            const string contentTypeString = "text/plain";
            const string encodingString = "utf-8";
            var encoding = Encoding.UTF8;
            var request = new ProviderServiceRequest
            {
                Method = HttpVerb.Post,
                Path = "/events",
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", contentTypeString + "; charset=" + encodingString }
                },
                Body = new { }
            };
            var httpBodyContent = new HttpBodyContent(content: Encoding.UTF8.GetBytes(String.Empty), contentType: new MediaTypeHeaderValue(contentTypeString) { CharSet = encodingString });

            var mapper = GetSubject();

            _mockHttpMethodMapper.Convert(HttpVerb.Post).Returns(HttpMethod.Post);
            _mockHttpBodyContentMapper.Convert(Arg.Any<object>(), request.Headers).Returns(httpBodyContent);

            var result = mapper.Convert(request);

            Assert.Empty(result.Headers);
            _mockHttpBodyContentMapper.Received(1).Convert(request.Body, request.Headers);
            _mockHttpContentMapper.Received(1).Convert(httpBodyContent);
        }

        [Fact]
        public void Convert_WithJsonContentTypeAndUnicodeCharsetHeader_HeaderIsNotAddedToHttpRequestMessageAndHttpContentMapperIsCalledWithEncodingAndContentType()
        {
            const string contentTypeString = "application/json";
            const string encodingString = "utf-16";

            var request = new ProviderServiceRequest
            {
                Method = HttpVerb.Post,
                Path = "/events",
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", contentTypeString + "; charset=" + encodingString }
                },
                Body = new { }
            };
            var httpBodyContent = new HttpBodyContent(content: Encoding.UTF8.GetBytes(String.Empty), contentType: new MediaTypeHeaderValue(contentTypeString) { CharSet = encodingString });

            var mapper = GetSubject();

            _mockHttpMethodMapper.Convert(HttpVerb.Post).Returns(HttpMethod.Post);
            _mockHttpBodyContentMapper.Convert(Arg.Any<object>(), request.Headers).Returns(httpBodyContent);

            var result = mapper.Convert(request);

            Assert.Empty(result.Headers);
            _mockHttpBodyContentMapper.Received(1).Convert(request.Body, request.Headers);
            _mockHttpContentMapper.Received(1).Convert(httpBodyContent);
        }

        [Fact]
        public void Convert_WithContentTypeAndCustomHeader_OnlyCustomHeadersIsAddedToHttpRequestMessage()
        {
            const string contentTypeString = "text/plain";
            var request = new ProviderServiceRequest
            {
                Method = HttpVerb.Post,
                Path = "/events",
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", contentTypeString },
                    { "X-Custom", "My Custom header" }
                },
                Body = new { }
            };
            var httpBodyContent = new HttpBodyContent(content: Encoding.UTF8.GetBytes(String.Empty), contentType: new MediaTypeHeaderValue(contentTypeString) { CharSet = "utf-8" });

            var mapper = GetSubject();

            _mockHttpMethodMapper.Convert(HttpVerb.Post).Returns(HttpMethod.Post);
            _mockHttpBodyContentMapper.Convert(Arg.Any<object>(), request.Headers).Returns(httpBodyContent);

            var result = mapper.Convert(request);

            Assert.Equal(request.Headers.Last().Key, result.Headers.First().Key);
            Assert.Equal(request.Headers.Last().Value, result.Headers.First().Value.First());
        }

        [Fact]
        public void Convert_WithContentLengthHeader_ContentLengthHeaderIsNotAddedToHttpRequestMessage()
        {
            var request = new ProviderServiceRequest
            {
                Method = HttpVerb.Post,
                Path = "/events",
                Headers = new Dictionary<string, string>
                {
                    { "Content-Length", "12" }
                },
                Body = "Some content"
            };

            var mapper = GetSubject();

            _mockHttpMethodMapper.Convert(HttpVerb.Post).Returns(HttpMethod.Post);

            var result = mapper.Convert(request);

            Assert.Equal(0, result.Headers.Count());
        }

        [Fact]
        public void Convert_WithContentLengthHeader_ContentLengthHeaderIsAddedToHttpRequestMessageContentHeaders()
        {
            var request = new ProviderServiceRequest
            {
                Method = HttpVerb.Post,
                Path = "/events",
                Headers = new Dictionary<string, string>
                {
                    { "Content-Length", "12" }
                },
                Body = "Some content"
            };
            var httpBodyContent = new HttpBodyContent(body: request.Body, contentType: new MediaTypeHeaderValue("text/plain") { CharSet = "utf-8" });
            var stringContent = new StringContent(request.Body, Encoding.UTF8, "text/plain");

            var mapper = GetSubject();

            _mockHttpMethodMapper.Convert(HttpVerb.Post).Returns(HttpMethod.Post);
            _mockHttpBodyContentMapper.Convert(Arg.Any<string>(), Arg.Any<IDictionary<string, string>>()).Returns(httpBodyContent);
            _mockHttpContentMapper.Convert(httpBodyContent).Returns(stringContent);

            var result = mapper.Convert(request);

            Assert.Equal(request.Headers.Last().Key, result.Content.Headers.Last().Key);
            Assert.Equal(request.Headers.Last().Value, result.Content.Headers.Last().Value.First());
        }

        [Fact]
        public void Convert_WithContentTypeSpecifiedAndAlsoBeingSetByStringContent_ContentTypeHeaderIsNotReAddedToHttpRequestMessageContentHeaders()
        {
            var request = new ProviderServiceRequest
            {
                Method = HttpVerb.Post,
                Path = "/events",
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "text/plain" }
                },
                Body = "Some content"
            };
            var httpBodyContent = new HttpBodyContent(body: request.Body, contentType: new MediaTypeHeaderValue("text/plain") { CharSet = "utf-8" });
            var stringContent = new StringContent(request.Body, Encoding.UTF8, "text/plain");

            var mapper = GetSubject();

            _mockHttpMethodMapper.Convert(HttpVerb.Post).Returns(HttpMethod.Post);
            _mockHttpBodyContentMapper.Convert(Arg.Any<string>(), Arg.Any<IDictionary<string, string>>()).Returns(httpBodyContent);
            _mockHttpContentMapper.Convert(httpBodyContent).Returns(stringContent);

            var result = mapper.Convert(request);

            Assert.Equal(1, result.Content.Headers.Count());
            Assert.Equal(request.Headers.First().Key, result.Content.Headers.First().Key);
            Assert.Equal("text/plain; charset=utf-8", result.Content.Headers.First().Value.First());
        }

        [Fact]
        public void Convert_WithContentTypeSpecifiedButNotBeingSetByByteArrayContent_ContentTypeHeaderIsNotReAddedToHttpRequestMessageContentHeaders()
        {
            var request = new ProviderServiceRequest
            {
                Method = HttpVerb.Post,
                Path = "/events",
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/octet-stream" }
                },
                Body = Encoding.UTF8.GetBytes("Some content")
            };
            var httpBodyContent = new HttpBodyContent(body: request.Body, contentType: new MediaTypeHeaderValue("text/plain") { CharSet = "utf-8" });
            var byteArrayContent = new ByteArrayContent(request.Body as byte[]);

            var mapper = GetSubject();

            _mockHttpMethodMapper.Convert(HttpVerb.Post).Returns(HttpMethod.Post);
            _mockHttpBodyContentMapper.Convert(body: Arg.Any<byte[]>(), headers: Arg.Any<IDictionary<string, string>>()).Returns(httpBodyContent);
            _mockHttpContentMapper.Convert(httpBodyContent).Returns(byteArrayContent);

            var result = mapper.Convert(request);

            Assert.Equal(1, result.Content.Headers.Count());
            Assert.Equal(request.Headers.First().Key, result.Content.Headers.First().Key);
            Assert.Equal("application/octet-stream", result.Content.Headers.First().Value.First());
        }

        [Fact]
        public void Convert_WithBody_HttpContentMapperIsCalled()
        {
            var request = new ProviderServiceRequest
            {
                Method = HttpVerb.Get,
                Path = "/events",
                Body = new
                {
                    Test = "tester"
                }
            };

            var mapper = GetSubject();

            _mockHttpMethodMapper.Convert(HttpVerb.Get).Returns(HttpMethod.Get);

            mapper.Convert(request);

            _mockHttpBodyContentMapper.Received(1).Convert(request.Body, request.Headers);
        }

        [Fact]
        public void Convert_WithTheWorks_CorrectlyMappedHttpRequestMessageIsReturned()
        {
            const string encodingString = "utf-8";
            const string contentTypeString = "application/json";
            const string bodyJson = "{\"Test\":\"tester\",\"Testing\":1}";

            var request = new ProviderServiceRequest
            {
                Method = HttpVerb.Get,
                Path = "/events",
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", contentTypeString + "; charset=" + encodingString },
                    { "X-Custom", "My Custom header" },
                    { "Content-Length", "1000" }
                },
                Body = new
                {
                    Test = "tester",
                    Testing = 1
                }
            };
            var httpBodyContent = new HttpBodyContent(body: bodyJson, contentType: new MediaTypeHeaderValue(contentTypeString) { CharSet = encodingString });

            var mapper = GetSubject();

            _mockHttpMethodMapper.Convert(HttpVerb.Get).Returns(HttpMethod.Get);
            _mockHttpContentMapper.Convert(httpBodyContent).Returns(new StringContent(bodyJson, Encoding.UTF8, contentTypeString));
            _mockHttpBodyContentMapper.Convert(Arg.Any<object>(), request.Headers).Returns(httpBodyContent);

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
            Assert.Equal(request.Headers.Last().Value, contentLengthHeader.Value.First());
        }
    }
}