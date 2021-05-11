using System;
using Xunit;

namespace PactNet.Tests
{
    public class PactUriOptionsTests
    {

        // This is an experimental test set.
        // Use of Data Driving improves readability and 
        // shows all the permutations run by one test
        [Theory]
        [InlineData( ""        , ""            , "username is null or empty." ) ] 
        [InlineData( ""        , "somepassword", "username is null or empty." ) ] 
        [InlineData( "goodname", ""            , "password is null or empty." ) ] 
        [InlineData( null      , ""            , "username is null or empty." ) ]
        [InlineData( ""        , null          , "username is null or empty." ) ]
        [InlineData( null      , null          , "username is null or empty." ) ]
        [InlineData( "bad:name", ""            , "username contains a ':' character, which is not allowed." ) ] // RFC 2617 compliance
        [InlineData( "bad:name", "goodpass"    , "username contains a ':' character, which is not allowed." ) ] // RFC 2617 compliance
        [InlineData( "bad:name", null          , "username contains a ':' character, which is not allowed." ) ] // RFC 2617 compliance
        [InlineData( "goodname", null          , "password is null or empty." ) ] // RFC 2617 compliance
        [InlineData( "goodname", ""            , "password is null or empty." ) ] // RFC 2617 compliance
        public void Ctor_takesInvalidUserNameandPasswordParamsThatThrowArgException(string username, string password, string expectedMessage)
        {
            Exception e = Assert.Throws <ArgumentException> (() => new PactUriOptions(username, password));
            Assert.Equal(expectedMessage , e.Message);
        }

        [Theory]
        [InlineData("some@user","password")]
        [InlineData("someuser","password")]
        [InlineData("someuser","pass word")]
        public void Ctor_AllowUserNamesAndPasswords(string username, string password )
        {
            var options = new PactUriOptions(username, password);
            Assert.Equal(options.Username, username);
        }

        [Fact]
        public void Ctor_With_ValidUserNamePassword_ValidateAuthorisationSchemaAndvalue()
        {
            const string username = "Aladdin";
            const string password = "open sesame";
            const string expectedAuthScheme = "Basic";
            const string expectedAuthValue = "QWxhZGRpbjpvcGVuIHNlc2FtZQ==";

            var options = new PactUriOptions(username, password);

            Assert.Equal(expectedAuthScheme, options.AuthorizationScheme);
            Assert.Equal(expectedAuthValue, options.AuthorizationValue);

        }

        [Fact]
        public void Ctor_GivenTokenIsSet_WhenUserNameIsSet_ThenThrowsInvalidOperationException()
        {
            const string username = "Aladdin";
            const string password = "open sesame";
            const string token = "sometokenvalue";
            string expectedMessage = "You can't set both bearer and basic authentication at the same time";

            var options = new PactUriOptions(token);

            Exception e = Assert.Throws<InvalidOperationException>( () => options.SetBasicAuthentication(username, password) );
            Assert.Equal(expectedMessage, e.Message);
        }

        [Fact]
        public void Ctor_GivenUserNamePasswordIsSet_WhenBearerTokenIsSet_ThenThrowsInvalidOperationException()
        {
            const string username = "Aladdin";
            const string password = "password";
            const string token = "token";
            const string expectedMessage = "You can't set both bearer and basic authentication at the same time";

            var options = new PactUriOptions(username, password);

            Exception e = Assert.Throws< InvalidOperationException>( () => options.SetBearerAuthentication(token) );
            Assert.Equal(expectedMessage, e.Message);
        }

        [Fact]
        public void Ctor_GivenUserNamePasswordAreSet_WhenBearerTokenIsSet_ThenThrowsInvalidOperationException()
        {
            const string username = "Aladdin";
            const string password = "open sesame";
            const string token = "sometokenvalue";
            const string expectedMessage = "You can't set both bearer and basic authentication at the same time";

            var options = new PactUriOptions(username, password);

            Exception e = Assert.Throws<InvalidOperationException>(() => options.SetBearerAuthentication(token));
            Assert.Equal(expectedMessage, e.Message);
        }

        [Fact]
        public void Ctor_WithValidUsernameAndPassword_ReturnsCorrectAuthorizationSchemeAndValue()
        {
            const string username = "Aladdin";
            const string password = "open sesame";
            const string expectedAuthScheme = "Basic";
            const string expectedAuthValue = "QWxhZGRpbjpvcGVuIHNlc2FtZQ==";

            var options = new PactUriOptions().SetBasicAuthentication(username, password);

            Assert.Equal(expectedAuthScheme, options.AuthorizationScheme);
            Assert.Equal(expectedAuthValue, options.AuthorizationValue);
        }

        [Fact]
        public void Ctor_WithValidUsernameAndPassword_ReturnsCorrectUsernameAndPassword()
        {
            const string username = "Aladdin";
            const string password = "open sesame";

            var options = new PactUriOptions().SetBasicAuthentication(username, password);

            Assert.Equal(username, options.Username);
            Assert.Equal(password, options.Password);
        }

        [Fact]
        public void Ctor_WithEmptyToken_ThrowsArgumentException()
        {
            const string token = "";

            Assert.Throws<ArgumentException>(() => new PactUriOptions().SetBearerAuthentication(token));
        }

        [Fact]
        public void Ctor_WithValidToken_ReturnsCorrectAuthorizationSchemeAndValue()
        {
            const string token = "MyToken";
            const string expectedAuthScheme = "Bearer";
            const string expectedAuthValue = token;

            var options = new PactUriOptions().SetBearerAuthentication(token);

            Assert.Equal(expectedAuthScheme, options.AuthorizationScheme);
            Assert.Equal(expectedAuthValue, options.AuthorizationValue);
        }

        [Fact]
        public void Ctor_WithValidToken_ReturnsCorrectToken()
        {
            const string token = "MyToken";

            var options = new PactUriOptions().SetBearerAuthentication(token);

            Assert.Equal(token, options.Token);
        }

        [Fact]
        public void Ctor_SetSslCaPathEmpty_ThrowsArgumentException()
        {
            const string sslpath = "";

            Assert.Throws<ArgumentException>(() => new PactUriOptions().SetSslCaFilePath(sslpath));
        }

        [Fact]
        public void Ctor_SetSslCaPathNull_ThrowsArgumentException()
        {
            const string sslpath = null;

            Assert.Throws<ArgumentException>(() => new PactUriOptions().SetSslCaFilePath(sslpath));
        }

        [Fact]
        public void CTor_SetHttpProxyEmpty_ThrowsArgumentException()
        {
            const string proxy = "";
            Assert.Throws<ArgumentException>(() => new PactUriOptions().SetHttpProxy(proxy));
        }

        [Fact]
        public void CTor_SetHttpProxyNull_ThrowsArgumentException()
        {
            const string proxy = null;
            Assert.Throws<ArgumentException>(() => new PactUriOptions().SetHttpProxy(proxy));
        }



        [Fact]
        public void CTor_SetHttpsProxiesEmpty_ThrowsArgumentException()
        {
            const string httpProxy = "somevalue";
            const string httpsProxy = "";
            Assert.Throws<ArgumentException>( () => new PactUriOptions().SetHttpProxy( httpProxy, httpsProxy ) );
        }

        [Fact]
        public void CTor_SetHttpsProxiesNull_ThrowsArgumentException()
        {
            const string httpProxy = "somevalue";
            const string httpsProxy = null;
            Assert.Throws<ArgumentException>(() => new PactUriOptions().SetHttpProxy( httpProxy, httpsProxy ) );
        }
    }
}
