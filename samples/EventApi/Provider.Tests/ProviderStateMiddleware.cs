using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using static System.String;

namespace Provider.Tests
{
    public class ProviderStateMiddleware
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly IDictionary<string, Action<IDictionary<string, string>>> providerStates;
        private readonly RequestDelegate next;

        public ProviderStateMiddleware(RequestDelegate next)
        {
            this.next = next;

            this.providerStates = new Dictionary<string, Action<IDictionary<string, string>>>
            {
                {
                    "there are events with ids '45D80D13-D5A2-48D7-8353-CBB4C0EAABF5', '83F9262F-28F1-4703-AB1A-8CFD9E8249C9' and '3E83A96B-2A0C-49B1-9959-26DF23F83AEB'",
                    this.InsertEventsIntoDatabase
                },
                {
                    "there is an event with id '83f9262f-28f1-4703-ab1a-8cfd9e8249c9'",
                    this.InsertEventIntoDatabase
                },
                {
                    "there is one event with type 'DetailsView'",
                    this.EnsureOneDetailsViewEventExists
                }
            };
        }

        private void InsertEventsIntoDatabase(IDictionary<string, string> parameters)
        {

        }

        private void InsertEventIntoDatabase(IDictionary<string, string> parameters)
        {

        }

        private void EnsureOneDetailsViewEventExists(IDictionary<string, string> parameters)
        {

        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!(context.Request.Path.Value?.StartsWith("/provider-states") ?? false))
            {
                await this.next.Invoke(context);
                return;
            }

            context.Response.StatusCode = (int)HttpStatusCode.OK;

            if (context.Request.Method == HttpMethod.Post.ToString())
            {
                string jsonRequestBody;
                using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8))
                {
                    jsonRequestBody = await reader.ReadToEndAsync();
                }

                var providerState = JsonSerializer.Deserialize<ProviderState>(jsonRequestBody, Options);

                //A null or empty provider state key must be handled
                if (!IsNullOrEmpty(providerState?.State))
                {
                    this.providerStates[providerState.State].Invoke(providerState.Params);
                }

                await context.Response.WriteAsync(Empty);
            }
        }
    }
}
