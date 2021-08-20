using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Moq;
using PactNet.AspNetCore.Messaging;
using PactNet.Native;
using Provider.Api;
using Provider.Api.Controllers;
using Provider.Domain;
using Provider.Domain.Models;
using Provider.Infrastructure;
using EventHandler = Provider.Domain.Handlers.EventHandler;

namespace Provider
{
    public class Startup
    {
        public static readonly SymmetricSecurityKey IssuerSigningKey = new(Encoding.UTF8.GetBytes("super secret key"));

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
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
                        options.Authority = "https://id.example.org";
                        options.Audience = "events-api";
                        options.TokenValidationParameters.IssuerSigningKey = IssuerSigningKey;

                        // prevent the OIDC metadata lookup since this isn't a real ID provider
                        options.Configuration = new OpenIdConnectConfiguration
                        {
                            Issuer = "https://id.example.org"
                        };
                        options.RequireHttpsMetadata = false;
                    });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Provider", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Provider v1"));
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            //app.UseMessaging();

            //SetupScenarios();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void SetupScenarios()
        {
            MessageScenarioBuilder
                .NewScenario
                .WhenReceiving("receiving events from the queue")
                .WillPublishMessage(() =>
                {
                    var mockEventProducer = new Mock<IEventProducer>();
                    List<Event> eventsPushed = null;
                    mockEventProducer
                        .Setup(x => x.Send(It.IsAny<IReadOnlyCollection<Event>>()))
                        .Callback((IReadOnlyCollection<Event> events) => { eventsPushed = events.ToList(); });

                    var controller = new EventsController(new FakeEventRepository(), new EventHandler(mockEventProducer.Object, new EventDispatcher()));

                    controller.ImportToQueue();

                    return eventsPushed;
                });

            MessageScenarioBuilder
                .NewScenario
                .WhenReceiving("dispatch event from the queue")
                .WillPublishMessage(() =>
                {
                    var mockEventDispatcher = new Mock<IEventDispatcher>();
                    Event eventPushed = null;
                    mockEventDispatcher
                        .Setup(x => x.Send(It.IsAny<Event>()))
                        .Callback((Event eventSingle) => { eventPushed = eventSingle; });

                    var controller = new EventsController(new FakeEventRepository(), new EventHandler(new Mock<IEventProducer>().Object, mockEventDispatcher.Object));

                    controller.DispatchLastEvent();

                    return eventPushed;
                });
        }

        private class FakeEventRepository : IEventRepository
        {
            public IReadOnlyCollection<Event> GetAllEvents()
            {
                return new List<Event>
                {
                    new Event
                    {
                        EventId = Guid.Parse("45D80D13-D5A2-48D7-8353-CBB4C0EAABF5"),
                        Timestamp = DateTime.Parse("2014-06-30T01:37:41.0660548"),
                        EventType = "SearchView"
                    }
                };
            }
        }
    }
}
