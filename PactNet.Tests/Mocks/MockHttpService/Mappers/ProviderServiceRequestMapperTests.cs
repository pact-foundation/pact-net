using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using NSubstitute;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Mappers;
using PactNet.Mocks.MockHttpService.Models;
using Xunit;

namespace PactNet.Tests.Mocks.MockHttpService.Mappers
{
    public class ProviderServiceRequestMapperTests
    {
        private IProviderServiceRequestMapper GetSubject()
        {
            return new ProviderServiceRequestMapper();
        }

        [Fact]
        public void Convert_WithNullRequest_ReturnsNull()
        {
            var mapper = GetSubject();

            var result = mapper.Convert(null);

            Assert.Null(result);
        }

        [Fact]
        public void Convert_WithMethod_CallsHttpVerbMapperAndSetsHttpMethod()
        {
            const HttpVerb httpVerb = HttpVerb.Get;
            var request = Substitute.For<IRequestWrapper>();

            request.Method.Returns("GET");
            request.Path.Returns("/events");

            var mockHttpVerbMapper = Substitute.For<IHttpVerbMapper>();
            var mockHttpBodyContentMapper = Substitute.For<IHttpBodyContentMapper>();
            mockHttpVerbMapper.Convert("GET").Returns(httpVerb);

            var mapper = new ProviderServiceRequestMapper(mockHttpVerbMapper, mockHttpBodyContentMapper);

            var result = mapper.Convert(request);

            Assert.Equal(httpVerb, result.Method);
            mockHttpVerbMapper.Received(1).Convert("GET");
        }

        [Fact]
        public void Convert_WithPath_CorrectlySetsPath()
        {
            const string path = "/events";
            const HttpVerb httpVerb = HttpVerb.Get;
            var request = Substitute.For<IRequestWrapper>();

            request.Method.Returns("GET");
            request.Path.Returns(path);

            var mockHttpVerbMapper = Substitute.For<IHttpVerbMapper>();
            var mockHttpBodyContentMapper = Substitute.For<IHttpBodyContentMapper>();
            mockHttpVerbMapper.Convert("GET").Returns(httpVerb);

            var mapper = new ProviderServiceRequestMapper(mockHttpVerbMapper, mockHttpBodyContentMapper);

            var result = mapper.Convert(request);

            Assert.Equal(path, result.Path);
        }

        [Fact]
        public void Convert_WithPathAndEmptyQuery_QueryIsSetToNull()
        {
            const string path = "/events";

            const HttpVerb httpVerb = HttpVerb.Get;
            var request = Substitute.For<IRequestWrapper>();

            request.Method.Returns("GET");
            request.Path.Returns(path);

            var mockHttpVerbMapper = Substitute.For<IHttpVerbMapper>();
            var mockHttpBodyContentMapper = Substitute.For<IHttpBodyContentMapper>();
            mockHttpVerbMapper.Convert("GET").Returns(httpVerb);

            var mapper = new ProviderServiceRequestMapper(mockHttpVerbMapper, mockHttpBodyContentMapper);

            var result = mapper.Convert(request);

            Assert.Null(result.Query);
        }

        [Fact]
        public void Convert_WithPathAndQuery_CorrectlySetsPathAndQuery()
        {
            const string path = "/events";
            const string query = "test=2&test2=hello";
            const HttpVerb httpVerb = HttpVerb.Get;
            var request = GetPreCannedRequest();

            request.Path.Returns(path);
            request.Query.Returns("?" + query);

            var mockHttpVerbMapper = Substitute.For<IHttpVerbMapper>();
            var mockHttpBodyContentMapper = Substitute.For<IHttpBodyContentMapper>();
            mockHttpVerbMapper.Convert("GET").Returns(httpVerb);

            var mapper = new ProviderServiceRequestMapper(mockHttpVerbMapper, mockHttpBodyContentMapper);

            var result = mapper.Convert(request);

            Assert.Equal(path, result.Path);
            Assert.Equal(query, result.Query);
        }

        [Fact]
        public void Convert_WithHeaders_CorrectlySetsHeaders()
        {
            var contentType = "text/plain";
            var contentEncoding = "charset=utf-8";

            var customHeaderValue = "Custom Header Value";

            var headers = new Dictionary<string, IEnumerable<string>>
            {
                { "Content-Type", new List<string> { contentType, contentEncoding } },
                { "X-Custom", new List<string> { customHeaderValue } }
            };
            var request = GetPreCannedRequest(headers);

            var mockHttpVerbMapper = Substitute.For<IHttpVerbMapper>();
            var mockHttpBodyContentMapper = Substitute.For<IHttpBodyContentMapper>();
            mockHttpVerbMapper.Convert("GET").Returns(HttpVerb.Get);

            var mapper = new ProviderServiceRequestMapper(mockHttpVerbMapper, mockHttpBodyContentMapper);

            var result = mapper.Convert(request);

            Assert.Equal(contentType + ", " + contentEncoding, result.Headers["Content-Type"]);
            Assert.Equal(customHeaderValue, result.Headers["X-Custom"]);
        }

        [Fact]
        public void Convert_WithPlainTextBody_CallsHttpBodyContentMapperAndCorrectlySetsBody()
        {
            const string content = "Plain text body";
            var request = GetPreCannedRequest(content: content);
            var httpBodyContent = new HttpBodyContent(new DynamicBody { Body = content, ContentType = new MediaTypeHeaderValue("text/plain") { CharSet = "utf-8" } });

            var mockHttpVerbMapper = Substitute.For<IHttpVerbMapper>();
            var mockHttpBodyContentMapper = Substitute.For<IHttpBodyContentMapper>();
            mockHttpVerbMapper.Convert("GET").Returns(HttpVerb.Get);
            mockHttpBodyContentMapper.Convert(Arg.Any<BinaryContentMapRequest>()).Returns(httpBodyContent);

            var mapper = new ProviderServiceRequestMapper(mockHttpVerbMapper, mockHttpBodyContentMapper);

            var result = mapper.Convert(request);

            Assert.Equal(content, result.Body);
            mockHttpBodyContentMapper.Received(1).Convert(Arg.Any<BinaryContentMapRequest>());
        }

        [Fact]
        public void Convert_WithJsonBody_CallsHttpBodyContentMapperAndCorrectlySetsBody()
        {
            var headers = new Dictionary<string, IEnumerable<string>>
            {
                { "Content-Type", new List<string> { "application/json", "charset=utf-8" } }
            };
            var body = new
            {
                Test = "tester",
                test2 = 1
            };
            const string content = "{\"Test\":\"tester\",\"test2\":1}";
            var contentBytes = Encoding.UTF8.GetBytes(content);
            var request = GetPreCannedRequest(headers: headers, content: content);
            var httpBodyContent = new HttpBodyContent(new BinaryContent { Content = contentBytes, ContentType = new MediaTypeHeaderValue("application/json") { CharSet = "utf-8" } });

            var mockHttpVerbMapper = Substitute.For<IHttpVerbMapper>();
            var mockHttpBodyContentMapper = Substitute.For<IHttpBodyContentMapper>();
            mockHttpVerbMapper.Convert("GET").Returns(HttpVerb.Get);
            mockHttpBodyContentMapper.Convert(Arg.Any<BinaryContentMapRequest>()).Returns(httpBodyContent);

            var mapper = new ProviderServiceRequestMapper(mockHttpVerbMapper, mockHttpBodyContentMapper);

            var result = mapper.Convert(request);

            Assert.Equal(body.Test, (string)result.Body.Test);
            Assert.Equal(body.test2, (int)result.Body.test2);
            mockHttpBodyContentMapper.Received(1).Convert(Arg.Any<BinaryContentMapRequest>());
        }

        private IRequestWrapper GetPreCannedRequest(IDictionary<string, IEnumerable<string>> headers = null, string content = null)
        {
            var request = Substitute.For<IRequestWrapper>();

            if (!String.IsNullOrEmpty(content))
            {
                request.Body.Returns(Encoding.UTF8.GetBytes(content));
            }

            request.Method.Returns("GET");
            request.Path.Returns("/events");
            request.Headers.Returns(headers);

            return request;
        }
    }
}