using System;
using System.Text;
using PactNet.Mocks.MockHttpService.Models;
using Xunit;

namespace PactNet.Tests.Mocks.MockHttpService.Models
{
    public class HttpBodyContentTests
    {
        private HttpBodyContent GetSubject()
        {
            return new HttpBodyContent();
        }

        [Fact]
        public void ContentBytes_WithNullContent_ReturnsNull()
        {
            var httpBodyContent = GetSubject();
            httpBodyContent.Content = null;

            Assert.Null(httpBodyContent.ContentBytes);
        }

        [Fact]
        public void ContentBytes_WithEmptyContent_ReturnsEmptyUtf8ByteArray()
        {
            var httpBodyContent = GetSubject();
            httpBodyContent.Content = String.Empty;

            Assert.Empty(httpBodyContent.ContentBytes);
        }

        [Fact]
        public void ContentType_WithNullContentTypeSet_ReturnsPlainContentType()
        {
            var httpBodyContent = GetSubject();
            httpBodyContent.ContentType = null;

            Assert.Equal("text/plain", httpBodyContent.ContentType);
        }

        [Fact]
        public void ContentType_WithEmptyContentTypeSet_ReturnsPlainContentType()
        {
            var httpBodyContent = GetSubject();
            httpBodyContent.ContentType = String.Empty;

            Assert.Equal("text/plain", httpBodyContent.ContentType);
        }

        [Fact]
        public void Encoding_WithNullEncodingSet_ReturnsUtf8Encoding()
        {
            var httpBodyContent = GetSubject();
            httpBodyContent.Encoding = null;

            Assert.Equal(Encoding.UTF8, httpBodyContent.Encoding);
        }
    }
}
