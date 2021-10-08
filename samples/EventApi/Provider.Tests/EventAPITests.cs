using System;
using System.Collections.Generic;
using System.IO;
using PactNet.AspNetCore.ProviderState;
using PactNet.Infrastructure.Outputters;
using PactNet.Native.Verifier;
using PactNet.Verifier;
using Provider.Api.Web.Tests;
using Xunit;
using Xunit.Abstractions;

namespace Provider.Tests
{
    public class EventApiTests : IClassFixture<EventApiFixture>
    {
        private readonly EventApiFixture fixture;
        private readonly ITestOutputHelper output;
        private readonly ProviderStateOptions options;

        public EventApiTests(EventApiFixture fixture, ITestOutputHelper output)
        {
            this.fixture = fixture;
            this.output = output;
            this.options = fixture.GetOptions();
        }

        [Fact]
        public void EnsureEventApiHonoursPactWithConsumer()
        {
            var config = new PactVerifierConfig
            {
                Outputters = new List<IOutput>
                {
                    new XUnitOutput(this.output)
                }
            };

            string pactPath = Path.Combine("..",
                                           "..",
                                           "..",
                                           "..",
                                           "Consumer.Tests",
                                           "pacts",
                                           "Event API Consumer-Event API.json");

            //Act / Assert
            IPactVerifier pactVerifier = new PactVerifier(config);
            pactVerifier
                .ServiceProvider("Event API", this.fixture.ServerUri)
                .HonoursPactWith("Event API Consumer")
                .FromPactFile(new FileInfo(pactPath))
                .WithProviderStateUrl(new Uri(this.fixture.ServerUri, options.RouteProviderState))
                .WithProviderStates(providerStates =>
                {
                    providerStates
                        .Add(
                            "there are events with ids '45D80D13-D5A2-48D7-8353-CBB4C0EAABF5', '83F9262F-28F1-4703-AB1A-8CFD9E8249C9' and '3E83A96B-2A0C-49B1-9959-26DF23F83AEB'",
                            InsertEventsIntoDatabase)
                        .Add("there is an event with id '83f9262f-28f1-4703-ab1a-8cfd9e8249c9'", InsertEventIntoDatabase)
                        .Add("there is one event with type 'DetailsView'", this.EnsureOneDetailsViewEventExists);
                })
                .Verify();

            Assert.True(HasExecuteInsertEventsIntoDatabase);
            Assert.True(HasExecuteInsertEventIntoDatabase);
            Assert.True(HasExecuteEnsureOneDetailsViewEventExists);
        }

        public bool HasExecuteInsertEventsIntoDatabase { get; set; }
        public bool HasExecuteInsertEventIntoDatabase { get; set; }
        public bool HasExecuteEnsureOneDetailsViewEventExists { get; set; }

        private void InsertEventsIntoDatabase()
        {
            HasExecuteInsertEventsIntoDatabase = true;
        }

        private void InsertEventIntoDatabase()
        {
            HasExecuteInsertEventIntoDatabase = true;
        }

        private void EnsureOneDetailsViewEventExists()
        {
            HasExecuteEnsureOneDetailsViewEventExists = true;
        }
    }
}
