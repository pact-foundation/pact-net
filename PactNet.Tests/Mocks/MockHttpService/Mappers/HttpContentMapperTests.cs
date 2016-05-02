using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using PactNet.Mocks.MockHttpService.Mappers;
using PactNet.Mocks.MockHttpService.Models;
using Xunit;

namespace PactNet.Tests.Mocks.MockHttpService.Mappers
{
    public class HttpContentMapperTests
    {
        private IHttpContentMapper GetSubject()
        {
            return new HttpContentMapper();
        }

        [Fact]
        public void Convert_WithNullHttpBodyContent_ReturnsNull()
        {
            var mapper = GetSubject();

            var result = mapper.Convert(null);

            Assert.Null(result);
        }

        [Fact]
        public void Convert_WithEmptyContent_ReturnsNull()
        {
            var httpBodyContent = new HttpBodyContent(content: Encoding.UTF8.GetBytes(String.Empty), contentType: "text/plain", encoding: Encoding.UTF8);
            var mapper = GetSubject();

            var result = mapper.Convert(httpBodyContent);

            Assert.Empty(result.ReadAsStringAsync().Result);
        }

        [Fact]
        public void Convert_WithContentTypeContainingParameter_ReturnsContentWithContentTypeHeader()
        {
            const string contentType = "text/plain";
            const string content = "test";
            var httpBodyContent = new HttpBodyContent(content: Encoding.UTF8.GetBytes(content), contentType: contentType + "; version=1", encoding: Encoding.UTF8);
            IHttpContentMapper mapper = GetSubject();

            HttpContent result = mapper.Convert(httpBodyContent);

            Assert.Equal(contentType, result.Headers.ContentType.MediaType);
            Assert.Contains(new NameValueHeaderValue("version", "1"), result.Headers.ContentType.Parameters);
            Assert.Equal("utf-8", result.Headers.ContentType.CharSet);
            Assert.Equal(content, result.ReadAsStringAsync().Result);
        }
    }
}