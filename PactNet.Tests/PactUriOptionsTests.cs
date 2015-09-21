using System;
using Xunit;

namespace PactNet.Tests
{
    public class PactUriOptionsTests
    {
        [Fact]
        public void Ctor_WithEmptyUsername_ThrowsArgumentException()
        {
            const string username = "";
            const string password = "somepassword";
            
            Assert.Throws<ArgumentException>(() => new PactUriOptions(username, password));
        }

        [Fact]
        public void Ctor_WithInvalidUsername_ThrowsArgumentException()
        {
            const string username = "some:user";
            const string password = "somepassword";

            Assert.Throws<ArgumentException>(() => new PactUriOptions(username, password));
        }

        [Fact]
        public void Ctor_WithEmptyPassword_ThrowsArgumentException()
        {
            const string username = "someuser";
            const string password = "";

            Assert.Throws<ArgumentException>(() => new PactUriOptions(username, password));
        }

        [Fact]
        public void Ctor_WithValidUsernameAndPassword_ReturnsCorrectAuthorizationSchemeAndValue()
        {
            const string username = "Aladdin";
            const string password = "open sesame";
            var expectedAuthScheme = "Basic";
            var expectedAuthValue = "QWxhZGRpbjpvcGVuIHNlc2FtZQ==";

            var options = new PactUriOptions(username, password);

            Assert.Equal(expectedAuthScheme, options.AuthorizationScheme);
            Assert.Equal(expectedAuthValue, options.AuthorizationValue);
        }
    }
}
