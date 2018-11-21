using System;
using System.Collections.Generic;
using System.Web.Http;
using PactNet.PactVerification;
using Provider.Api.Web.Models;
using Provider.Api.Web.Publishers;

namespace Provider.Api.Web.Tests.Controllers
{
	public class VerificationController : ApiController
	{
		private readonly EventsPublisher _eventsPublisher = new EventsPublisher();

		[HttpPost]
		[Route("")]
		public IHttpActionResult Invoke(MessagePactDescription description)
		{
			var messageInvoker = new MessageInvoker(new Dictionary<string, Action>
				{
					{
						"Event With id 45D80D13-D5A2-48D7-8353-CBB4C0EAABF5 is in the database",
						InsertEventIntoDatabase
					},
					{
						"there is one event with type DetailsView",
						EnsureOneDetailsViewEventExists
					}
				},
				new Dictionary<string, Func<object>>
				{
					{"Event with id 45D80D13-D5A2-48D7-8353-CBB4C0EAABF5 updated", () => _eventsPublisher.GetEventUpdatedMessage(new Event
					{
						EventId = new Guid("45D80D13-D5A2-48D7-8353-CBB4C0EAABF5")
					}) }
				}
			);

			var message = messageInvoker.Invoke(description);
			return Ok(new { contents = message });
		}

		private void EnsureOneDetailsViewEventExists()
		{
			//ImplementProviderState
		}

		private void InsertEventIntoDatabase()
		{
			//ImplementProviderState
		}
	}
}
