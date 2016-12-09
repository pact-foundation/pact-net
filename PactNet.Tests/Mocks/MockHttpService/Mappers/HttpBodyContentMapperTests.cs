using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using PactNet.Mocks.MockHttpService.Mappers;
using Xunit;

namespace PactNet.Tests.Mocks.MockHttpService.Mappers
{
    public class HttpBodyContentMapperTests
    {
        private IHttpBodyContentMapper GetSubject()
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
            Assert.Equal("text/plain", result.ContentType.MediaType);
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
            Assert.Equal(contentTypeString, result.ContentType.MediaType);
        }

        [Fact]
        public void Convert1_WithJsonBodyAndDifferentCasings_ReturnsUnchangedCasingJsonBodyWithUtf8EncodingAndApplicationJsonContentType()
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
            Assert.Equal("Application/Json", result.ContentType.MediaType);
        }

        [Fact]
        public void Convert1_WithContentTypeParameterAndUsAsciiCharSet_ReturnsJsonBodyWithAsciiEncodingAndContentType()
        {
            const string contentTypeString = "text/html";
            const string body = "<html><head></head><body><p>This is a test</p></body></html>";
            const string charSet = "us-ascii";
            var encoding = Encoding.GetEncoding(charSet);

            var headers = new Dictionary<string, string>
            {
                { "Content-Type", $"{contentTypeString}; charset={charSet}" }
            };

            var mapper = GetSubject();

            var result = mapper.Convert(body: body, headers: headers);
            
            Assert.Equal(encoding, result.Encoding);
            Assert.Equal(body, result.Content);
            Assert.Equal(contentTypeString, result.ContentType.MediaType);
            Assert.Equal(charSet, result.ContentType.CharSet);
        }

        [Fact]
        public void Convert1_WithContentTypeParameterAndIbm285CharSet_ReturnsJsonBodyWithIbm285EncodingAndContentTypeWithParameters()
        {
            const string contentTypeString = "text/richtext";
            const string body = "string";
            const string parameterName = "date-format";
            const string parameterValue = "json";
            const string charSet = "IBM285";
            var encoding = Encoding.GetEncoding(charSet);

            var headers = new Dictionary<string, string>
            {
                { "Content-Type", $"{contentTypeString}; {parameterName}={parameterValue}; charset={charSet}" }
            };

            var mapper = GetSubject();

            var result = mapper.Convert(body: body, headers: headers);

            Assert.Equal(encoding, result.Encoding);
            Assert.Equal(body, result.Content);
            Assert.Equal(contentTypeString, result.ContentType.MediaType);
            Assert.Equal(charSet, result.ContentType.CharSet);
            Assert.Contains(new NameValueHeaderValue(parameterName, parameterValue), result.ContentType.Parameters);
        }

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

            var result = mapper.Convert(content: Encoding.UTF8.GetBytes(content), headers: null);

            Assert.Equal(content, result.Content);
            Assert.Equal(Encoding.UTF8, result.Encoding);
            Assert.Equal("text/plain", result.ContentType.MediaType);
        }

        [Fact]
        public void Convert2_WithJsonContentAndDifferentCasings_ReturnsUnchangedCasingJsonBodyWithUtf8EncodingAndApplicationJsonContentType()
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

            var result = mapper.Convert(content: Encoding.UTF8.GetBytes(content), headers: headers);

            Assert.Equal(body.Test, (string)result.Body.Test);
            Assert.Equal(body.tesT2, (int)result.Body.tesT2);
            Assert.Equal(Encoding.UTF8, result.Encoding);
            Assert.Equal("Application/Json", result.ContentType.MediaType);
        }

        [Fact]
        public void Convert2_WithContentTypeParameterAndUsAsciiCharSet_ReturnsJsonBodyWithAsciiEncodingAndContentType()
        {
            const string contentTypeString = "text/html";
            const string content = "<html><head></head><body><p>This is a test</p></body></html>";
            const string charSet = "us-ascii";
            var encoding = Encoding.GetEncoding(charSet);

            var headers = new Dictionary<string, string>
            {
                { "Content-Type", $"{contentTypeString}; charset={charSet}" }
            };

            var mapper = GetSubject();

            var result = mapper.Convert(body: encoding.GetBytes(content), headers: headers);
            
            Assert.Equal(encoding, result.Encoding);
            Assert.Equal(content, result.Content);
            Assert.Equal(contentTypeString, result.ContentType.MediaType);
            Assert.Equal(charSet, result.ContentType.CharSet);
        }

        [Fact]
        public void Convert2_WithContentTypeParameterAndIbm285CharSet_ReturnsJsonBodyWithIbm285EncodingAndContentTypeWithParameters()
        {
            const string contentTypeString = "text/richtext";
            const string content = "string";
            const string parameterName = "date-format";
            const string parameterValue = "json";
            const string charSet = "IBM285";
            var encoding = Encoding.GetEncoding(charSet);

            var headers = new Dictionary<string, string>
            {
                { "Content-Type", $"{contentTypeString}; {parameterName}={parameterValue}; charset={charSet}" }
            };

            var mapper = GetSubject();

            var result = mapper.Convert(body: encoding.GetBytes(content), headers: headers);
            
            Assert.Equal(encoding, result.Encoding);
            Assert.Equal(encoding.GetString(encoding.GetBytes(content)), result.Content);
            Assert.Equal(contentTypeString, result.ContentType.MediaType);
            Assert.Equal(charSet, result.ContentType.CharSet);
            Assert.Contains(new NameValueHeaderValue(parameterName, parameterValue), result.ContentType.Parameters);
        }
    }
}