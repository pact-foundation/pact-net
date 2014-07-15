using System.Collections.Generic;
using System.Text;
using NSubstitute;
using PactNet.Mocks.MockHttpService.Mappers;
using Xunit;

namespace PactNet.Tests.Mocks.MockHttpService.Mappers
{
    public class HttpBodyContentMapperTests
    {
        public IHttpBodyContentMapper GetSubject()
        {
            return new HttpBodyContentMapper();
        }

        [Fact]
        public void Convert1_WithNullBody_ReturnsNull()
        {
            var mapper = GetSubject();

            var result = mapper.Convert(body: null, headers: null);

            Assert.Null(result);
        }

        [Fact]
        public void Convert1_WithBodyAndNullHeaders_ReturnsBodyWithUtf8EncodingAndPlainTextContentType()
        {
            const string body = "This is my content";
            var mapper = GetSubject();

            var result = mapper.Convert(body: body, headers: null);

            Assert.Equal(body, result.Content);
            Assert.Equal(Encoding.UTF8, result.Encoding);
            Assert.Equal("text/plain", result.ContentType);
        }

        [Fact]
        public void Convert1_WithJsonBody_ReturnsJsonBodyWithUtf8EncodingAndApplicationJsonContentType()
        {
            const string contentTypeString = "application/json";
            var body = new
            {
                Test = "tester",
                Test2 = 1
            };
            var headers = new Dictionary<string, string>
            {
                { "Content-Type", contentTypeString }
            };
            const string jsonBody = "{\"Test\":\"tester\",\"Test2\":1}";
            var mapper = GetSubject();

            var result = mapper.Convert(body: body, headers: headers);

            Assert.Equal(jsonBody, result.Content);
            Assert.Equal(Encoding.UTF8, result.Encoding);
            Assert.Equal(contentTypeString, result.ContentType);
        }

        [Fact]
        public void Convert1_WithJsonBodyAndDifferentCasings_ReturnsUnNormalisedCasingJsonBodyWithUtf8EncodingAndApplicationJsonContentType()
        {
            var body = new
            {
                Test = "testeR",
                tesT2 = 1
            };
            var headers = new Dictionary<string, string>
            {
                { "content-type", "Application/Json" }
            };
            const string jsonBody = "{\"Test\":\"testeR\",\"tesT2\":1}";
            var mapper = GetSubject();

            var result = mapper.Convert(body: body, headers: headers);

            Assert.Equal(jsonBody, result.Content);
            Assert.Equal(Encoding.UTF8, result.Encoding);
            Assert.Equal("application/json", result.ContentType);
        }

        [Fact]
        public void Convert1_WithAsciiHtmlBody_CallsEncodingMapperAndReturnsBodyWithAsciiEncodingAndHtmlContentType()
        {
            const string contentTypeString = "text/html";
            const string body = "<p>This is my content</p>";
            const string encodingString = "us-ascii";
            var encoding = Encoding.ASCII;
            var headers = new Dictionary<string, string>
            {
                { "Content-Type", contentTypeString + "; charset=" + encodingString }
            };
            var mockEncodingMapper = Substitute.For<IEncodingMapper>();
            mockEncodingMapper.Convert(encodingString).Returns(encoding);

            var mapper = new HttpBodyContentMapper(mockEncodingMapper);

            var result = mapper.Convert(body: body, headers: headers);

            Assert.Equal(body, result.Content);
            Assert.Equal(encoding, result.Encoding);
            Assert.Equal(contentTypeString, result.ContentType);
        }

        //

        [Fact]
        public void Convert2_WithNullContent_ReturnsNull()
        {
            var mapper = GetSubject();

            var result = mapper.Convert(content: null, headers: null);

            Assert.Null(result);
        }

        [Fact]
        public void Convert2_WithBodyAndNullHeaders_ReturnsBodyWithUtf8EncodingAndPlainTextContentType()
        {
            const string content = "This is my content";
            var mapper = GetSubject();

            var result = mapper.Convert(content: content, headers: null);

            Assert.Equal(content, result.Content);
            Assert.Equal(Encoding.UTF8, result.Encoding);
            Assert.Equal("text/plain", result.ContentType);
        }

        [Fact]
        public void Convert2_WithJsonContentAndDifferentCasings_ReturnsUnNormalisedCasingJsonBodyWithUtf8EncodingAndApplicationJsonContentType()
        {
            var body = new
            {
                Test = "testeR",
                tesT2 = 1
            };
            var headers = new Dictionary<string, string>
            {
                { "content-type", "Application/Json" }
            };
            const string content = "{\"Test\":\"testeR\",\"tesT2\":1}";
            var mapper = GetSubject();

            var result = mapper.Convert(content: content, headers: headers);

            Assert.Equal(body.Test, (string)result.Body.Test);
            Assert.Equal(body.tesT2, (int)result.Body.tesT2);
            Assert.Equal(Encoding.UTF8, result.Encoding);
            Assert.Equal("application/json", result.ContentType);
        }

        [Fact]
        public void Convert2_WithAsciiHtmlContent_CallsEncodingMapperAndReturnsBodyWithAsciiEncodingAndHtmlContentType()
        {
            const string contentTypeString = "text/html";
            const string content = "<p>This is my content</p>";
            const string encodingString = "us-ascii";
            var encoding = Encoding.ASCII;
            var headers = new Dictionary<string, string>
            {
                { "Content-Type", contentTypeString + "; charset=" + encodingString }
            };
            var mockEncodingMapper = Substitute.For<IEncodingMapper>();
            mockEncodingMapper.Convert(encodingString).Returns(encoding);

            var mapper = new HttpBodyContentMapper(mockEncodingMapper);

            var result = mapper.Convert(content: content, headers: headers);

            Assert.Equal(content, result.Body);
            Assert.Equal(encoding, result.Encoding);
            Assert.Equal(contentTypeString, result.ContentType);
        }

    }
}
