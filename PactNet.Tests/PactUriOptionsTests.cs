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
        [InlineData( ""        , ""         ) ] 
        [InlineData( null      , ""         ) ]
        [InlineData( ""        , null       ) ]
        [InlineData( null      , null       ) ]
        [InlineData( "bad:name", ""         ) ] // RFC 2617 compliance
        [InlineData( "bad:name", null       ) ] // RFC 2617 compliance
        [InlineData( "goodname", null       ) ] // RFC 2617 compliance
        [InlineData( "goodname", ""         ) ] // RFC 2617 compliance
        public void Ctor_takesUserNameandPasswordParamsThatThrowArgException(string username, string password)
        {
            Assert.Throws <ArgumentException> (() => new PactUriOptions(username, password));
        }

        [Fact]
        public void Ctor_WithEmptyUsername_ThrowsArgumentException()
        {
            const string username = "";
            const string password = "somepassword";
            
            Assert.Throws<ArgumentException>(() => new PactUriOptions().SetBasicAuthentication(username, password));
        }

        [Fact]
        public void Ctor_With_ValidUserNamePassword()
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
        public void Ctor_SetUserNameWhenTokenIsSet_ThrowsInvalidOperationException()
        {
            const string username = "Aladdin";
            const string password = "open sesame";
            const string token = "sometokenvalue";

            var options = new PactUriOptions(token);

            Assert.Throws<InvalidOperationException>( () => options.SetBasicAuthentication(username, password) );

        }

        [Fact]
        public void Ctor_SetBearerTokenWhenUserNamePasswordEmptyAreSet_ThrowsArgumentException()
        {
            const string username = "Aladdin";
            const string password = "";

            Assert.Throws< ArgumentException>( () => new PactUriOptions(username, password) );
        }

        [Fact]
        public void Ctor_SetBearerTokenWhenUserNamePasswordNullAreSet_ThrowsArgumentException()
        {
            const string username = "Aladdin";
            const string password = null;

            Assert.Throws<ArgumentException>( () => new PactUriOptions(username, password));
        }

        [Fact]
        public void Ctor_SetBearerTokenWhenUserNameEmptyPasswordAreSet_ThrowsArgumentException()
        {
            const string username = "";
            const string password = "password";

            Assert.Throws<ArgumentException>( () => new PactUriOptions(username, password));
        }

        [Fact]
        public void Ctor_SetBearerTokenWhenUserNameNullPasswordAreSet_ThrowsArgumentException()
        {
            const string username = null;
            const string password = "password";

            Assert.Throws<ArgumentException>(() => new PactUriOptions(username, password));
        }


        [Fact]
        public void Ctor_SetBearerTokenWhenUserNamePasswordAreSet_ThrowsInvalidOperationException()
        {
            const string username = "Aladdin";
            const string password = "open sesame";
            const string token = "sometokenvalue";

            var options = new PactUriOptions(username, password);

            Assert.Throws<InvalidOperationException>(() => options.SetBearerAuthentication(token));
        }

        [Fact]
        public void Ctor_WithInvalidUsername_ThrowsArgumentException()
        {
            const string username = "some:user";
            const string password = "somepassword";

            Assert.Throws<ArgumentException>(() => new PactUriOptions().SetBasicAuthentication(username, password));
        }

        [Fact]
        public void Ctor_WithEmptyPassword_ThrowsArgumentException()
        {
            const string username = "someuser";
            const string password = "";

            Assert.Throws<ArgumentException>(() => new PactUriOptions().SetBasicAuthentication(username, password));
        }

        [Fact]
        public void Ctor_UserNameWithAtSymbol()
        {
            const string username = "some@user";
            const string password = "password";

            var options = new PactUriOptions(username, password);

            Assert.Equal(options.Username, username);
        }

        [Fact]
        public void Ctor_PasswordWithAtSymbol()
        {
            const string username = "someuser";
            const string password = "pass@word";

            var options = new PactUriOptions(username, password);

            Assert.Equal(options.Password, password);
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
