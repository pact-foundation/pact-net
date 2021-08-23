using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace ReadMe.Provider
{
    public class Startup
    {
        public static readonly SymmetricSecurityKey IssuerSigningKey = new(Encoding.UTF8.GetBytes("IssuerSigningKey"));

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                });

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.Authority = "https://something.api.com";
                    options.Audience = "something-api";
                    options.TokenValidationParameters.IssuerSigningKey = IssuerSigningKey;

                    options.Configuration = new OpenIdConnectConfiguration()
                    {
                        Issuer = "https://something.api.com",
                    };
                    options.RequireHttpsMetadata = false;
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
