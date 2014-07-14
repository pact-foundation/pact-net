using System.Collections.Generic;
using System.IO;
using System.Linq;
using NSubstitute;
using Nancy;
using PactNet.Mocks.MockHttpService.Mappers;
using PactNet.Mocks.MockHttpService.Models;
using Xunit;

namespace PactNet.Tests.Mocks.MockHttpService.Mappers
{
    public class NancyResponseMapperTests
    {
        public INancyResponseMapper GetSubject()
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
            var response = new PactProviderServiceResponse
            {
                Status = 200
            };

            var mapper = GetSubject();

            var result = mapper.Convert(response);

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public void Convert_WithResponseWithHeader_ReturnsNancyResponseWithHeaderAndAdditionalEmptyContentLengthHeader()
        {
            var response = new PactProviderServiceResponse
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

            Assert.Equal("Content-Length", result.Headers.Last().Key);
            Assert.Equal("0", result.Headers.Last().Value);
        }

        [Fact]
        public void Convert_WithResponseThatHasANullBodyAndAContentLengthHeader_ReturnsNancyResponseWithNullBodyAndZeroContentLengthHeader()
        {
            var response = new PactProviderServiceResponse
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
            Assert.Equal("0", result.Headers.Last().Value);
            stream.Close();
            stream.Dispose();
        }

        [Fact]
        public void Convert_WithPlainTextBody_CallsConvertOnHttpBodyContentMapperAndAssignsContents()
        {
            const string contentTypeString = "text/plain";
            var response = new PactProviderServiceResponse
            {
                Status = 200,
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", contentTypeString }
                },
                Body = "This is a plain body"
            };
            var httpBodyContent = new HttpBodyContent(response.Body, contentTypeString, null);

            var mockHttpBodyContentMapper = Substitute.For<IHttpBodyContentMapper>();

            mockHttpBodyContentMapper.Convert(Arg.Any<object>(), response.Headers)
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
            mockHttpBodyContentMapper.Received(1).Convert(Arg.Any<object>(), response.Headers);
        }

        [Fact]
        public void Convert_WithJsonBody_CallsConvertOnHttpBodyContentMapperAndAssignsContents()
        {
            const string contentTypeString = "application/json";
            var response = new PactProviderServiceResponse
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
            var httpBodyContent = new HttpBodyContent(jsonBody, contentTypeString, null);

            var mockHttpBodyContentMapper = Substitute.For<IHttpBodyContentMapper>();

            mockHttpBodyContentMapper.Convert(Arg.Any<object>(), response.Headers)
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
            mockHttpBodyContentMapper.Received(1).Convert(Arg.Any<object>(), response.Headers);
        }
    }
}
