using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.OAuth;
using Owin;

namespace Provider.Api.Web.Tests
{
    public class TestStartup
    {
        public void Configuration(IAppBuilder app)
        {
            var apiStartup = new Startup();
            app.Use<ProviderStateMiddleware>();
            app.Use(typeof(AuthorizationTokenReplacementMiddleware),
                app.CreateDataProtector(typeof(OAuthAuthorizationServerMiddleware).Namespace, "Access_Token", "v1"));

            apiStartup.Configuration(app);
        }
    }
}