using System;
using System.Net.Http.Headers;
using System.Text;
using PactNet.Mocks.MockHttpService.Models;
using Xunit;

namespace PactNet.Tests.Mocks.MockHttpService.Models
{
    public class HttpBodyContentTests
    {
        [Fact]
        public void Ctor1_WithNullBody_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new HttpBodyContent(body: null, contentType: new MediaTypeHeaderValue("text/plain") { CharSet = "utf-8" }));
        }

        [Fact]
        public void Ctor2_WithNullContent_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new HttpBodyContent(content: null, contentType: new MediaTypeHeaderValue("text/plain") { CharSet = "utf-8" }));
        }

        [Fact]
        public void Ctor1_WithNullContentType_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new HttpBodyContent(body: new { }, contentType: null));
        }

        [Fact]
        public void Ctor2_WithNullContentType_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new HttpBodyContent(content: new byte[] { }, contentType: null));
        }

        [Fact]
        public void Ctor1_WithContentTypeAndMissingCharSet_ThrowsArgumentNullException()
        {
            Assert.Throws<InvalidOperationException>(() => new HttpBodyContent(body: new { }, contentType: new MediaTypeHeaderValue("text/plain")));
        }

        [Fact]
        public void Ctor2_WithContentTypeAndMissingCharSet_ThrowsArgumentNullException()
        {
            Assert.Throws<InvalidOperationException>(() => new HttpBodyContent(content: new byte[] { }, contentType: new MediaTypeHeaderValue("text/plain")));
        }

        [Fact]
        public void Ctor1_WithContentAndContentType_SetsContentType()
        {
            const string contentType = "text/html";
            const string parameterName = "date-format";
            const string parameterValue = "json";
            const string charSet = "utf-16";
            const string body = "<html/>";

            var httpBodyContent = new HttpBodyContent(
                body: body,
                contentType: new MediaTypeHeaderValue(contentType) { CharSet = charSet, Parameters = { new NameValueHeaderValue(parameterName, parameterValue) } });

            Assert.Equal(body, httpBodyContent.Content);
            Assert.Equal(contentType, httpBodyContent.ContentType.MediaType);
            Assert.Contains(new NameValueHeaderValue(parameterName, parameterValue), httpBodyContent.ContentType.Parameters);
            Assert.Equal(charSet, httpBodyContent.ContentType.CharSet);
            Assert.Equal(Encoding.Unicode, httpBodyContent.Encoding);
        }

        [Fact]
        public void Ctor2_WithContentAndContentType_SetsContentType()
        {
            const string contentType = "text/html";
            const string parameterName = "date-format";
            const string parameterValue = "json";
            const string charSet = "utf-16";
            const string content = "<html/>";
            byte[] body = Encoding.Unicode.GetBytes(content);

            var httpBodyContent = new HttpBodyContent(
                content: body,
                contentType: new MediaTypeHeaderValue(contentType) { CharSet = charSet, Parameters = { new NameValueHeaderValue(parameterName, parameterValue) } });

            Assert.Equal(content, httpBodyContent.Content);
            Assert.Equal(contentType, httpBodyContent.ContentType.MediaType);
            Assert.Contains(new NameValueHeaderValue(parameterName, parameterValue), httpBodyContent.ContentType.Parameters);
            Assert.Equal(charSet, httpBodyContent.ContentType.CharSet);
            Assert.Equal(Encoding.Unicode, httpBodyContent.Encoding);
        }

        [Fact]
        public void Ctor1_WithJsonBody_SetsBodyAndContent()
        {
            var body = new
            {
                Test = "tester",
                tesTer = 1
            };
            const string content = "{\"Test\":\"tester\",\"tesTer\":1}";
            var httpBodyContent = new HttpBodyContent(body: body, contentType: new MediaTypeHeaderValue("application/json") { CharSet = "utf-8" });

            Assert.Equal(content, httpBodyContent.Content);
            Assert.Equal(body, httpBodyContent.Body);
        }

        [Fact]
        public void Ctor1_WithJsonBodyAndTitleCasedContentType_SetsBodyAndContent()
        {
            var body = new
            {
                Test = "tester",
                tesTer = 1
            };
            const string content = "{\"Test\":\"tester\",\"tesTer\":1}";
            var httpBodyContent = new HttpBodyContent(body: body, contentType: new MediaTypeHeaderValue("Application/Json") { CharSet = "utf-8" });

            Assert.Equal(content, httpBodyContent.Content);
            Assert.Equal(body, httpBodyContent.Body);
        }

        [Fact]
        public void Ctor1_WithCustomJsonBody_SetsBodyAndContent()
        {
            var body = new
            {
                Test = "tester",
                tesTer = 1
            };
            const string content = "{\"Test\":\"tester\",\"tesTer\":1}";
            var httpBodyContent = new HttpBodyContent(body: body, contentType: new MediaTypeHeaderValue("application/x-amz-json-1.1") { CharSet = "utf-8" });

            Assert.Equal(content, httpBodyContent.Content);
            Assert.Equal(body, httpBodyContent.Body);
        }

        [Fact]
        public void Ctor2_WithJsonContent_SetsBodyAndContent()
        {
            var body = new
            {
                Test = "tester",
                tesTer = 1
            };
            const string content = "{\"Test\":\"tester\",\"tesTer\":1}";
            var httpBodyContent = new HttpBodyContent(content: Encoding.UTF8.GetBytes(content), contentType: new MediaTypeHeaderValue("application/json") { CharSet = "utf-8" });

            Assert.Equal(content, httpBodyContent.Content);
            Assert.Equal(body.Test, (string)httpBodyContent.Body.Test);
            Assert.Equal(body.tesTer, (int)httpBodyContent.Body.tesTer);
        }

        [Fact]
        public void Ctor2_WithCustomJsonContent_SetsBodyAndContent()
        {
            var body = new
            {
                Test = "tester",
                tesTer = 1
            };
            const string content = "{\"Test\":\"tester\",\"tesTer\":1}";
            var httpBodyContent = new HttpBodyContent(content: Encoding.UTF8.GetBytes(content), contentType: new MediaTypeHeaderValue("application/x-amz-json-1.1") { CharSet = "utf-8" });

            Assert.Equal(content, httpBodyContent.Content);
            Assert.Equal(body.Test, (string)httpBodyContent.Body.Test);
            Assert.Equal(body.tesTer, (int)httpBodyContent.Body.tesTer);
        }

        [Fact]
        public void Ctor1_WithPlainTextBody_SetsBodyAndContent()
        {
            const string body = "Some plain text";
            var httpBodyContent = new HttpBodyContent(body: body, contentType: new MediaTypeHeaderValue("application/plain") { CharSet = "utf-8" });

            Assert.Equal(body, httpBodyContent.Content);
            Assert.Equal(body, httpBodyContent.Body);
        }

        [Fact]
        public void Ctor2_WithPlainTextContent_SetsBodyAndContent()
        {
            const string content = "Some plain text";
            var httpBodyContent = new HttpBodyContent(content: Encoding.UTF8.GetBytes(content), contentType: new MediaTypeHeaderValue("application/plain") { CharSet = "utf-8" });

            Assert.Equal(content, httpBodyContent.Content);
            Assert.Equal(content, httpBodyContent.Body);
        }

        [Fact]
        public void Ctor1_WithBinaryBody_SetsBodyAndAndBase64EncodesTheContent()
        {
            var body = new byte[] { 1, 2, 3 };

            var httpBodyContent = new HttpBodyContent(body: body, contentType: new MediaTypeHeaderValue("application/octet-stream") { CharSet = "utf-8" });

            Assert.Equal(body, httpBodyContent.Body);
            Assert.Equal(Convert.ToBase64String(body), httpBodyContent.Content);
        }

        [Fact]
        public void Ctor1_WithBase64EncodedBinaryBody_SetsBodyAndAndBase64DecodesTheContent()
        {
            var body = new byte[] { 1, 2, 3 };
            var base64Body = Convert.ToBase64String(body);

            var httpBodyContent = new HttpBodyContent(body: base64Body, contentType: new MediaTypeHeaderValue("application/octet-stream") { CharSet = "utf-8" });

            Assert.Equal(base64Body, httpBodyContent.Body as string);
            Assert.Equal(Encoding.UTF8.GetString(body), httpBodyContent.Content);
        }

        [Fact]
        public void Ctor2_WithBinaryContent_SetsBodyAndBase64EncodesContent()
        {
            const string content = "LOL";
            var contentBytes = Encoding.UTF8.GetBytes(content);
            var httpBodyContent = new HttpBodyContent(content: contentBytes, contentType: new MediaTypeHeaderValue("application/octet-stream") { CharSet = "utf-8" });

            Assert.Equal(content, httpBodyContent.Content);
            Assert.IsType<string>(httpBodyContent.Body);
            Assert.Equal(Convert.ToBase64String(contentBytes), httpBodyContent.Body);
        }

        [Fact]
        public void Ctor2_WithEmptyContent_ReturnsEmptyUtf8ByteArray()
        {
            var httpBodyContent = new HttpBodyContent(content: Encoding.UTF8.GetBytes(String.Empty), contentType: new MediaTypeHeaderValue("text/plain") { CharSet = "utf-8" });

            Assert.Empty(httpBodyContent.ContentBytes);
        }
    }
}