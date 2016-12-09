using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using Nancy;
using NSubstitute;
using PactNet.Mocks.MockHttpService.Mappers;
using PactNet.Mocks.MockHttpService.Models;
using Xunit;

namespace PactNet.Tests.Mocks.MockHttpService.Mappers
{
    public class NancyResponseMapperTests
    {
        private INancyResponseMapper GetSubject()
        {
            return new NancyResponseMapper();
        }

        [Fact]
        public void Convert_WithNullResponse_ReturnsNull()
        {
            var mapper = GetSubject();

            var result = mapper.Convert(null);

            Assert.Null(result);
        }

        [Fact]
        public void Convert_WithResponseWithStatusCode_ReturnsNancyResponseWithStatusCode()
        {
            var response = new ProviderServiceResponse
            {
                Status = 200
            };

            var mapper = GetSubject();

            var result = mapper.Convert(response);

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public void Convert_WithResponseThatHasANullBodyAndACustomHeader_ReturnsNancyResponseWithHeaderAndDoesNotAddAContentLengthHeader()
        {
            var response = new ProviderServiceResponse
            {
                Status = 200,
                Headers = new Dictionary<string, string>
                {
                    { "X-Test", "Tester" }
                }
            };

            var mapper = GetSubject();

            var result = mapper.Convert(response);

            Assert.Equal(response.Headers.First().Key, result.Headers.First().Key);
            Assert.Equal(response.Headers.First().Value, result.Headers.First().Value);
            Assert.Equal(1, result.Headers.Count());
        }

        [Fact]
        public void Convert_WithResponseThatHasANullBodyAndAContentLengthHeader_ReturnsNancyResponseWithNullBodyAndContentLengthHeader()
        {
            var response = new ProviderServiceResponse
            {
                Status = 200,
                Headers = new Dictionary<string, string>
                {
                    { "Content-Length", "100" }
                }
            };

            var mapper = GetSubject();

            var result = mapper.Convert(response);

            var stream = new MemoryStream();
            result.Contents(stream);
            Assert.Equal(0, stream.Length);
            Assert.Equal("Content-Length", result.Headers.Last().Key);
            Assert.Equal("100", result.Headers.Last().Value);
            stream.Close();
            stream.Dispose();
        }

        [Fact]
        public void Convert_WithPlainTextBody_CallsConvertOnHttpBodyContentMapperAndAssignsContents()
        {
            const string contentTypeString = "text/plain";
            var response = new ProviderServiceResponse
            {
                Status = 200,
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", contentTypeString }
                },
                Body = "This is a plain body"
            };
            var httpBodyContent = new HttpBodyContent(body: response.Body, contentType: new MediaTypeHeaderValue(contentTypeString) { CharSet = "utf-8" });

            var mockHttpBodyContentMapper = Substitute.For<IHttpBodyContentMapper>();

            mockHttpBodyContentMapper.Convert(body: Arg.Any<object>(), headers: response.Headers)
                .Returns(httpBodyContent);

            var mapper = new NancyResponseMapper(mockHttpBodyContentMapper);

            var result = mapper.Convert(response);

            string content;
            using (var stream = new MemoryStream())
            {
                result.Contents(stream);
                stream.Position = 0;
                using (var reader = new StreamReader(stream))
                {
                    content = reader.ReadToEnd();
                }
            }

            Assert.Equal(response.Body, content);
            mockHttpBodyContentMapper.Received(1).Convert(body: Arg.Any<object>(), headers: response.Headers);
        }

        [Fact]
        public void Convert_WithJsonBody_CallsConvertOnHttpBodyContentMapperAndAssignsContents()
        {
            const string contentTypeString = "application/json";
            var response = new ProviderServiceResponse
            {
                Status = 200,
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", contentTypeString }
                },
                Body = new
                {
                    Test = "tester",
                    Test2 = 1
                }
            };
            var jsonBody = "{\"Test\":\"tester\",\"Test2\":1}";
            var httpBodyContent = new HttpBodyContent(content: Encoding.UTF8.GetBytes(jsonBody), contentType: new MediaTypeHeaderValue(contentTypeString) { CharSet = "utf-8" });

            var mockHttpBodyContentMapper = Substitute.For<IHttpBodyContentMapper>();

            mockHttpBodyContentMapper.Convert(body: Arg.Any<object>(), headers: response.Headers)
                .Returns(httpBodyContent);

            var mapper = new NancyResponseMapper(mockHttpBodyContentMapper);

            var result = mapper.Convert(response);

            string content;
            using (var stream = new MemoryStream())
            {
                result.Contents(stream);
                stream.Position = 0;
                using (var reader = new StreamReader(stream))
                {
                    content = reader.ReadToEnd();
                }
            }

            Assert.Equal(jsonBody, content);
            mockHttpBodyContentMapper.Received(1).Convert(body: Arg.Any<object>(), headers: response.Headers);
        }
    }
}