using System;
using PactNet.Mocks.MockHttpService.Mappers;
using PactNet.Models.ProviderService;
using Xunit;

namespace PactNet.Tests.Mocks.MockHttpService.Mappers
{
    public class HttpVerbMapperTests
    {
        private IHttpVerbMapper GetSubject()
        {
            return new HttpVerbMapper();
        }

        [Fact]
        public void Convert_WithValueThatDoesNotHaveAMap_ThrowsArgumentException()
        {
            var mapper = GetSubject();

            Assert.Throws<ArgumentException>(() => mapper.Convert("blah"));
        }

        [Fact]
        public void Convert_WithGetString_ReturnsHttpVerbGet()
        {
            var mapper = GetSubject();

            var result = mapper.Convert("GET");

            Assert.Equal(HttpVerb.Get, result);
        }

        [Fact]
        public void Convert_WithPostString_ReturnsHttpVerbPost()
        {
            var mapper = GetSubject();

            var result = mapper.Convert("POST");

            Assert.Equal(HttpVerb.Post, result);
        }

        [Fact]
        public void Convert_WithPutString_ReturnsHttpVerbPut()
        {
            var mapper = GetSubject();

            var result = mapper.Convert("PUT");

            Assert.Equal(HttpVerb.Put, result);
        }

        [Fact]
        public void Convert_WithDeleteString_ReturnsHttpVerbDelete()
        {
            var mapper = GetSubject();

            var result = mapper.Convert("DELETE");

            Assert.Equal(HttpVerb.Delete, result);
        }

        [Fact]
        public void Convert_WithHeadString_ReturnsHttpVerbHead()
        {
            var mapper = GetSubject();

            var result = mapper.Convert("HEAD");

            Assert.Equal(HttpVerb.Head, result);
        }

        [Fact]
        public void Convert_WithPatchString_ReturnsHttpVerbPatch()
        {
            var mapper = GetSubject();

            var result = mapper.Convert("PATCH");

            Assert.Equal(HttpVerb.Patch, result);
        }
    }
}
