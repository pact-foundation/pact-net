
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace ReadMe.Provider.Tests
{
    public static class TokenGenerator
    {
        public static string Generate()
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "WebApiUser"),
            };
            var identity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Issuer = "https://something.api.com",
                Audience = "something-api",
                Subject = identity,
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(Startup.IssuerSigningKey, SecurityAlgorithms.HmacSha256Signature),
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
