using System;
using PactNet.Mocks.MockHttpService.Models;
using Xunit;

namespace PactNet.Tests.Mocks.MockHttpService.Models
{
    public class 
        ProviderServiceRequestTests
    {
        [Fact]
        public void PathWithQuery_WithNullPathAndQuery_ReturnsNull()
        {
            var request = new ProviderServiceRequest();

            var uri = request.PathWithQuery();

            Assert.Null(uri);
        }

        [Fact]
        public void PathWithQuery_WithJustPath_ReturnsPath()
        {
            var request = new ProviderServiceRequest
            {
                Path = "/events"
            };

            var uri = request.PathWithQuery();

            Assert.Equal(request.Path, uri);
        }

        [Fact]
        public void PathWithQuery_WithJustQuery_ThrowsInvalidOperationException()
        {
            var request = new ProviderServiceRequest
            {
                Query = "test1=1&test2=2"
            };

            Assert.Throws<InvalidOperationException>(() => request.PathWithQuery());
        }

        [Fact]
        public void PathWithQuery_WithPathAndQuery_ReturnsPathWithQuery()
        {
            var request = new ProviderServiceRequest
            {
                Path = "/events",
                Query = "test1=1&test2=2"
            };

            var uri = request.PathWithQuery();

            Assert.Equal(request.Path + "?" + request.Query, uri);
        }
    }
}
