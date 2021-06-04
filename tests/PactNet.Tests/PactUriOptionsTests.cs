using System;
using FluentAssertions;
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

            Action action = () => new PactUriOptions(username, password);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Ctor_WithInvalidUsername_ThrowsArgumentException()
        {
            const string username = "some:user";
            const string password = "somepassword";

            Action action = () => new PactUriOptions(username, password);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Ctor_WithEmptyPassword_ThrowsArgumentException()
        {
            const string username = "someuser";
            const string password = "";

            Action action = () => new PactUriOptions(username, password);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Ctor_WithValidUsernameAndPassword_ReturnsCorrectUsernameAndPassword()
        {
            const string username = "Aladdin";
            const string password = "open sesame";

            var options = new PactUriOptions(username, password);

            options.Username.Should().Be(username);
            options.Password.Should().Be(password);
        }

        [Fact]
        public void Ctor_WithEmptyToken_ThrowsArgumentException()
        {
            const string token = "";

            Action action = () => new PactUriOptions(token);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Ctor_WithValidToken_ReturnsCorrectToken()
        {
            const string token = "MyToken";

            var options = new PactUriOptions(token);

            options.Token.Should().Be(token);
        }
    }
}
