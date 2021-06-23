//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Net;
//using System.Text;
//using System.Text.Json;
//using System.Threading.Tasks;

//using Microsoft.AspNetCore.Http;

//using PactNet.Models;

//using Provider.Domain.Models;

//namespace Provider.Tests
//{
//    public class MessageMiddleware
//    {
//        private static readonly JsonSerializerOptions Options = new()
//        {
//            PropertyNameCaseInsensitive = true
//        };

//        private readonly IDictionary<string, Action<IDictionary<string, string>>> providerStates;
//        private readonly RequestDelegate next;

//        public MessageMiddleware(RequestDelegate next)
//        {
//            this.next = next;
//        }

//        public async Task InvokeAsync(HttpContext context)
//        {
//            if (!(context.Request.Path.Value?.StartsWith("/pact-messages") ?? false))
//            {
//                await next(context);
//                return;
//            }

//            context.Response.StatusCode = (int)HttpStatusCode.OK;

//            string jsonRequestBody;
//            using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8))
//            {
//                jsonRequestBody = await reader.ReadToEndAsync();
//            }

//            var actual = new List<Event>
//            {
//                new Event {
//                    EventId = Guid.Parse("45D80D13-D5A2-48D7-8353-CBB4C0EAABF5"),
//                    Timestamp = DateTime.Parse("2014-06-30T01:37:41.0660548"),
//                    EventType = "SearchView"
//                }
//            };

//            var messageRaw = JsonSerializer.Serialize(actual);

//            var message = JsonSerializer.Deserialize<Message>(messageRaw, Options);

//            //Logger<XUnitOutput> outputLogger = new Logger<XUnitOutput>();
//            //outputLogger.LogDebug();

//            ////A null or empty provider state key must be handled
//            //if (!IsNullOrEmpty(providerState?.State))
//            //{
//            //    this.providerStates[providerState.State].Invoke(providerState.Params);
//            //}

//            await context.Response.WriteAsJsonAsync(message);
//        }
//    }
//}
