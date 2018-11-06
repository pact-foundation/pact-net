using System;
using System.Collections.Generic;
using System.Web.Http;
using PactNet.PactVerification;
using Provider.Api.Web.Models;
using Provider.Api.Web.Publishers;

namespace Provider.Api.Web.Tests.VerificationController
{
	public class VerificationController : ApiController, IProducerHttpProxy
    {
        private readonly EventsPublisher _eventsPublisher = new EventsPublisher();

		[HttpGet]
        [Route("/Invoke/")]
        public string Invoke(dynamic messageDescription)
        {
            var messageInvoker = new MessageInvoker(new Dictionary<string, Action>
                {
                    {"Event With id 45D80D13-D5A2-48D7-8353-CBB4C0EAABF5 is in the database", InsertEventIntoDatabase}
                },
                new Dictionary<string, Func<string>>
                {
                    {"Event with id 45D80D13-D5A2-48D7-8353-CBB4C0EAABF5 updated", () => _eventsPublisher.GetEventUpdatedMessage(new Event
                    {
                        EventId = Guid.ParseExact("45D80D13-D5A2-48D7-8353-CBB4C0EAABF5", "N")
                    }) }
                }
            );

            return messageInvoker.Invoke(messageDescription);
        }

        private void InsertEventIntoDatabase()
        {
            //ImplementProviderState
        }
    }
}
