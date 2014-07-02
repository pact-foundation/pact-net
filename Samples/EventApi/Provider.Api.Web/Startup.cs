using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.Owin;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;
using Provider.Api.Web;
using Provider.Api.Web.Controllers;

[assembly: OwinStartup("ApiConfiguration", typeof(Startup))]
namespace Provider.Api.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            config.MapHttpAttributeRoutes();

            app.UseWebApi(config);

            var json = config.Formatters.JsonFormatter;
            json.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            json.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            json.SerializerSettings.Formatting = Formatting.None;
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            var builder = new ContainerBuilder();
            
            builder.RegisterApiControllers(typeof(TestController).Assembly);
            var container = builder.Build();

            app.UseAutofacMiddleware(container);
            app.UseAutofacWebApi(config);
        }
    }
}