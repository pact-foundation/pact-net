using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using Nancy;
using Nancy.IO;
using NSubstitute;
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
            var request = new Request("GET", "/events", "Http");

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
            var request = new Request("GET", path, "Http");

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
            var request = new Request("GET", path, "Http");

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
            request.Url.Path = path;
            request.Url.Query = "?" + query;

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
            var httpBodyContent = new HttpBodyContent(content, new MediaTypeHeaderValue("text/plain") { CharSet = "utf-8" });

            var mockHttpVerbMapper = Substitute.For<IHttpVerbMapper>();
            var mockHttpBodyContentMapper = Substitute.For<IHttpBodyContentMapper>();
            mockHttpVerbMapper.Convert("GET").Returns(HttpVerb.Get);
            mockHttpBodyContentMapper.Convert(content: Arg.Any<byte[]>(), headers: null).Returns(httpBodyContent);

            var mapper = new ProviderServiceRequestMapper(mockHttpVerbMapper, mockHttpBodyContentMapper);

            var result = mapper.Convert(request);

            Assert.Equal(content, result.Body);
            mockHttpBodyContentMapper.Received(1).Convert(content: Arg.Any<byte[]>(), headers: null);
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
            var httpBodyContent = new HttpBodyContent(content: contentBytes, contentType: new MediaTypeHeaderValue("application/json") { CharSet = "utf-8" });

            var mockHttpVerbMapper = Substitute.For<IHttpVerbMapper>();
            var mockHttpBodyContentMapper = Substitute.For<IHttpBodyContentMapper>();
            mockHttpVerbMapper.Convert("GET").Returns(HttpVerb.Get);
            mockHttpBodyContentMapper.Convert(content: Arg.Any<byte[]>(), headers: Arg.Any<IDictionary<string, string>>()).Returns(httpBodyContent);

            var mapper = new ProviderServiceRequestMapper(mockHttpVerbMapper, mockHttpBodyContentMapper);

            var result = mapper.Convert(request);

            Assert.Equal(body.Test, (string)result.Body.Test);
            Assert.Equal(body.test2, (int)result.Body.test2);
            mockHttpBodyContentMapper.Received(1).Convert(content: Arg.Any<byte[]>(), headers: Arg.Any<IDictionary<string, string>>());
        }

        private Request GetPreCannedRequest(IDictionary<string, IEnumerable<string>> headers = null, string content = null)
        {
            RequestStream requestStream = null;

            if (!String.IsNullOrEmpty(content))
            {
                var contentBytes = Encoding.UTF8.GetBytes(content);
                var stream = new MemoryStream(contentBytes);
                requestStream = new RequestStream(stream, contentBytes.Length, true);
            }

            var url = new Url
            {
                HostName = "localhost",
                Scheme = "http",
                Port = 1234,
                Path = "/events"
            };

            Request request;
            if (requestStream != null)
            {
                request = new Request("GET", url, headers: headers, body: requestStream);
            }
            else
            {
                request = new Request("GET", url, headers: headers);
            }

            return request;
        }
    }
}