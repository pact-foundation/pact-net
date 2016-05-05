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
            var httpBodyContent = new HttpBodyContent(content: Encoding.UTF8.GetBytes(String.Empty), contentType: new MediaTypeHeaderValue("text/plain") { CharSet = "utf-8" });
            var mapper = GetSubject();

            var result = mapper.Convert(httpBodyContent);

            Assert.Empty(result.ReadAsStringAsync().Result);
        }

        [Fact]
        public void Convert_WithContentTypeContainingParameter_ReturnsContentWithContentTypeHeader()
        {
            const string contentType = "text/plain";
            const string content = "test";
            NameValueHeaderValue versionParameter = new NameValueHeaderValue("version", "1");

            var httpBodyContent = new HttpBodyContent(
                content: Encoding.UTF8.GetBytes(content),
                contentType: new MediaTypeHeaderValue(contentType) { CharSet = "utf-8", Parameters = { versionParameter } });
            IHttpContentMapper mapper = GetSubject();

            HttpContent result = mapper.Convert(httpBodyContent);

            Assert.Equal(contentType, result.Headers.ContentType.MediaType);
            Assert.Contains(versionParameter, result.Headers.ContentType.Parameters);
            Assert.Equal("utf-8", result.Headers.ContentType.CharSet);
            Assert.Equal(content, result.ReadAsStringAsync().Result);
        }
    }
}