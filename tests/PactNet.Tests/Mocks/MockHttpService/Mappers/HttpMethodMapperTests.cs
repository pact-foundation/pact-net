using System;
using System.Net.Http;
using PactNet.Mocks.MockHttpService.Mappers;
using PactNet.Mocks.MockHttpService.Models;
using Xunit;

namespace PactNet.Tests.Mocks.MockHttpService.Mappers
{
    public class HttpMethodMapperTests
    {
        private IHttpMethodMapper GetSubject()
        {
            return new HttpMethodMapper();
        }

        [Fact]
        public void Convert_WithHttpVerbThatDoesNotHaveAMap_ThrowsArgumentException()
        {
            var mapper = GetSubject();

            Assert.Throws<ArgumentException>(() => mapper.Convert((HttpVerb)10000));
        }

        [Fact]
        public void Convert_WithGetHttpVerb_ReturnsGetHttpMethod()
        {
            var mapper = GetSubject();

            var httpMethod = mapper.Convert(HttpVerb.Get);

            Assert.Equal(HttpMethod.Get, httpMethod);
        }

        [Fact]
        public void Convert_WithPostHttpVerb_ReturnsPostHttpMethod()
        {
            var mapper = GetSubject();

            var httpMethod = mapper.Convert(HttpVerb.Post);

            Assert.Equal(HttpMethod.Post, httpMethod);
        }

        [Fact]
        public void Convert_WithPutHttpVerb_ReturnsPutHttpMethod()
        {
            var mapper = GetSubject();

            var httpMethod = mapper.Convert(HttpVerb.Put);

            Assert.Equal(HttpMethod.Put, httpMethod);
        }

        [Fact]
        public void Convert_WithDeleteHttpVerb_ReturnsDeleteHttpMethod()
        {
            var mapper = GetSubject();

            var httpMethod = mapper.Convert(HttpVerb.Delete);

            Assert.Equal(HttpMethod.Delete, httpMethod);
        }

        [Fact]
        public void Convert_WithHeadHttpVerb_ReturnsHeadHttpMethod()
        {
            var mapper = GetSubject();

            var httpMethod = mapper.Convert(HttpVerb.Head);

            Assert.Equal(HttpMethod.Head, httpMethod);
        }

        [Fact]
        public void Convert_WithPatchHttpVerb_ReturnsPatchHttpMethod()
        {
            var mapper = GetSubject();

            var httpMethod = mapper.Convert(HttpVerb.Patch);

            Assert.Equal("PATCH", httpMethod.ToString());
        }

        [Fact]
        public void Convert_WithOptionsHttpVerb_ReturnsOptionsHttpMethod()
        {
            var mapper = GetSubject();

            var httpMethod = mapper.Convert(HttpVerb.Options);

            Assert.Equal(HttpMethod.Options, httpMethod);
        }

        [Fact]
        public void Convert_WithNotSetHttpVerb_ThrowsArgumentException()
        {
            var mapper = GetSubject();

            Assert.Throws<ArgumentException>(() => mapper.Convert(HttpVerb.NotSet));
        }
    }
}
