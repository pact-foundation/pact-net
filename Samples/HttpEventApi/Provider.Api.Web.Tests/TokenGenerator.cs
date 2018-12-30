using System;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler;
using System.Security.Claims;
using Microsoft.Owin.Security.DataProtection;

namespace Provider.Api.Web.Tests
{
    public class TokenGenerator
    {
        private readonly IDataProtector _dataProtector;

        public TokenGenerator(IDataProtector dataProtector)
        {
            _dataProtector = dataProtector;
        }

        public string Generate()
        {
            // Generate an OAuth bearer token for ASP.NET/Owin Web Api service that uses the default OAuthBearer token middleware.
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "WebApiUser"),
                new Claim(ClaimTypes.Role, "User"),
                new Claim(ClaimTypes.Role, "PowerUser"),
            };
            var identity = new ClaimsIdentity(claims, "Test");

            // Use the same token generation logic as the OAuthBearer Owin middleware. 
            var tdf = new TicketDataFormat(_dataProtector);
            var ticket = new AuthenticationTicket(identity, new AuthenticationProperties { ExpiresUtc = DateTime.UtcNow.AddHours(1) });
            var accessToken = tdf.Protect(ticket);

            return accessToken;
        }
    }
}
