using System;
using System.Text;
using PactNet.Mocks.MockHttpService.Mappers;
using PactNet.Mocks.MockHttpService.Models;
using Xunit;

namespace PactNet.Tests.Mocks.MockHttpService.Mappers
{
    public class HttpContentMapperTests
    {
        public IHttpContentMapper GetSubject()
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
            var httpBodyContent = new HttpBodyContent(String.Empty, "text/plain", Encoding.UTF8);
            var mapper = GetSubject();

            var result = mapper.Convert(httpBodyContent);

            Assert.Empty(result.ReadAsStringAsync().Result);
        }
    }
}
