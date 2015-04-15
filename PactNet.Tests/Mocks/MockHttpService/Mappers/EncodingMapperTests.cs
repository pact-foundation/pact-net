using System;
using System.Text;
using PactNet.Mocks.MockHttpService.Mappers;
using Xunit;

namespace PactNet.Tests.Mocks.MockHttpService.Mappers
{
    public class EncodingMapperTests
    {
        private EncodingMapper GetSubject()
        {
            return new EncodingMapper();
        }

        [Fact]
        public void Convert_WithEncodingThatDoesNotHaveAMap_ThrowsArgumentException()
        {
            var mapper = GetSubject();

            Assert.Throws<ArgumentException>(() => mapper.Convert(String.Empty));
        }

        [Fact]
        public void Convert_WithUtf8String_ReturnUtf8Encoding()
        {
            var mapper = GetSubject();

            var result = mapper.Convert("utf-8");

            Assert.Equal(Encoding.UTF8, result);
        }

        [Fact]
        public void Convert_WithUnicodeString_ReturnUnicodeEncoding()
        {
            var mapper = GetSubject();

            var result = mapper.Convert("utf-16");

            Assert.Equal(Encoding.Unicode, result);
        }

        [Fact]
        public void Convert_WithUtf32String_ReturnUtf32Encoding()
        {
            var mapper = GetSubject();

            var result = mapper.Convert("utf-32");

            Assert.Equal(Encoding.UTF32, result);
        }

        [Fact]
        public void Convert_WithBigEndianUnicodeString_ReturnBigEndianUnicodeEncoding()
        {
            var mapper = GetSubject();

            var result = mapper.Convert("utf-16BE");

            Assert.Equal(Encoding.BigEndianUnicode, result);
        }

        [Fact]
        public void Convert_WithBigEndianUnicodeLowercaseString_ReturnBigEndianUnicodeEncoding()
        {
            var mapper = GetSubject();

            var result = mapper.Convert("utf-16be");

            Assert.Equal(Encoding.BigEndianUnicode, result);
        }

        [Fact]
        public void Convert_WithAsciiString_ReturnAsciiEncoding()
        {
            var mapper = GetSubject();

            var result = mapper.Convert("us-ascii");

            Assert.Equal(Encoding.ASCII, result);
        }
    }
}
