using System.Linq;
using System.Text;
using PactNet.Mocks.MockHttpService.Mappers;
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
        public void Convert_WithNullBody_ReturnsNull()
        {
            var mapper = GetSubject();

            var mappedBody = mapper.Convert(null, null, null);

            Assert.Null(mappedBody);
        }

        [Fact]
        public void Convert_WithBodyAndNullEncodingAndNullContentType_ReturnsPlaintextUtf8HttpContent()
        {
            var body = "Just a simple plaintext body";
            var mapper = GetSubject();

            var mappedBody = mapper.Convert(body, null, null);

            var plainTextBody = mappedBody.ReadAsStringAsync().Result;
            var contentTypeHeader = mappedBody.Headers.First(x => x.Key.Equals("Content-Type"));
            var contentLengthHeader = mappedBody.Headers.FirstOrDefault(x => x.Key.Equals("Content-Length"));

            Assert.Equal(body, plainTextBody);
            Assert.Equal("text/plain; charset=utf-8", contentTypeHeader.Value.First());
            Assert.NotNull(contentLengthHeader);
        }

        [Fact]
        public void Convert_WithUtf8Encoding_ReturnsPlaintextUtf8HttpContent()
        {
            var body = "Just a simple plaintext body";
            var mapper = GetSubject();

            var mappedBody = mapper.Convert(body, Encoding.UTF8, null);

            var plainTextBody = mappedBody.ReadAsStringAsync().Result;
            var contentTypeHeader = mappedBody.Headers.First(x => x.Key.Equals("Content-Type"));
            var contentLengthHeader = mappedBody.Headers.FirstOrDefault(x => x.Key.Equals("Content-Length"));

            Assert.Equal(body, plainTextBody);
            Assert.Equal("text/plain; charset=utf-8", contentTypeHeader.Value.First());
            Assert.NotNull(contentLengthHeader);
        }

        [Fact]
        public void Convert_WithAsciiEncoding_ReturnsPlaintextAsciiHttpContent()
        {
            var body = "Just a simple plaintext body";
            var mapper = GetSubject();

            var mappedBody = mapper.Convert(body, Encoding.ASCII, null);

            var plainTextBody = mappedBody.ReadAsStringAsync().Result;
            var contentTypeHeader = mappedBody.Headers.First(x => x.Key.Equals("Content-Type"));
            var contentLengthHeader = mappedBody.Headers.FirstOrDefault(x => x.Key.Equals("Content-Length"));

            Assert.Equal(body, plainTextBody);
            Assert.Equal("text/plain; charset=us-ascii", contentTypeHeader.Value.First());
            Assert.NotNull(contentLengthHeader);
        }

        [Fact]
        public void Convert_WithUtf32Encoding_ReturnsPlaintextUtf32HttpContent()
        {
            var body = "Just a simple plaintext body";
            var mapper = GetSubject();

            var mappedBody = mapper.Convert(body, Encoding.UTF32, null);

            var plainTextBody = mappedBody.ReadAsStringAsync().Result;
            var contentTypeHeader = mappedBody.Headers.First(x => x.Key.Equals("Content-Type"));
            var contentLengthHeader = mappedBody.Headers.FirstOrDefault(x => x.Key.Equals("Content-Length"));

            Assert.Equal(body, plainTextBody);
            Assert.Equal("text/plain; charset=utf-32", contentTypeHeader.Value.First());
            Assert.NotNull(contentLengthHeader);
        }

        [Fact]
        public void Convert_WithPlaintextContentType_ReturnsPlaintextUtf8HttpContent()
        {
            var body = "Just a simple plaintext body";
            var mapper = GetSubject();

            var mappedBody = mapper.Convert(body, Encoding.UTF8, "text/plain");

            var plainTextBody = mappedBody.ReadAsStringAsync().Result;
            var contentTypeHeader = mappedBody.Headers.First(x => x.Key.Equals("Content-Type"));
            var contentLengthHeader = mappedBody.Headers.FirstOrDefault(x => x.Key.Equals("Content-Length"));

            Assert.Equal(body, plainTextBody);
            Assert.Equal("text/plain; charset=utf-8", contentTypeHeader.Value.First());
            Assert.NotNull(contentLengthHeader);
        }

        [Fact]
        public void Convert_WithJsonBodyAndJsonContentType_ReturnsJsonUtf8HttpContent()
        {
            var body = new
            {
                Test1 = "Hi",
                Test2 = 3
            };
            var mapper = GetSubject();

            var mappedBody = mapper.Convert(body, Encoding.UTF8, "application/json");

            var jsonBody = mappedBody.ReadAsStringAsync().Result;
            var contentTypeHeader = mappedBody.Headers.First(x => x.Key.Equals("Content-Type"));
            var contentLengthHeader = mappedBody.Headers.FirstOrDefault(x => x.Key.Equals("Content-Length"));

            Assert.Equal("{\"Test1\":\"Hi\",\"Test2\":3}", jsonBody);
            Assert.Equal("application/json; charset=utf-8", contentTypeHeader.Value.First());
            Assert.NotNull(contentLengthHeader);
        }

        [Fact]
        public void Convert_WithMixedCasePropertyNamesInJsonBodyAndJsonContentType_ReturnsJsonUtf8HttpContentWithoutPropertyNamesBeingNormalised()
        {
            var body = new
            {
                Test1 = "Hi",
                test2 = 3,
                TesT3 = new { IteM = "yaaR" }
            };
            var mapper = GetSubject();

            var mappedBody = mapper.Convert(body, Encoding.UTF8, "application/json");

            var jsonBody = mappedBody.ReadAsStringAsync().Result;
            var contentTypeHeader = mappedBody.Headers.First(x => x.Key.Equals("Content-Type"));
            var contentLengthHeader = mappedBody.Headers.FirstOrDefault(x => x.Key.Equals("Content-Length"));

            Assert.Equal("{\"Test1\":\"Hi\",\"test2\":3,\"TesT3\":{\"IteM\":\"yaaR\"}}", jsonBody);
            Assert.Equal("application/json; charset=utf-8", contentTypeHeader.Value.First());
            Assert.NotNull(contentLengthHeader);
        }
    }
}
