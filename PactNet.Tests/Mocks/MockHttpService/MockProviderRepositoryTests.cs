using System;
using System.Linq;
using System.Net;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Models;
using Xunit;

namespace PactNet.Tests.Mocks.MockHttpService
{
    public class MockProviderRepositoryTests
    {
        private IMockProviderRepository GetSubject()
        {
            return new MockProviderRepository();
        }

        [Fact]
        public void AddInteraction_WithNullInteraction_ThrowsArgumentNullException()
        {
            var repo = GetSubject();

            Assert.Throws<ArgumentNullException>(() => repo.AddInteraction(null));
        }

        [Fact]
        public void AddInteraction_WithInteraction_InteractionIsAddedToInteractionsAndTestScopedInteractions()
        {
            var interaction = new ProviderServiceInteraction { Description = "My description 1" };

            var repo = GetSubject();

            repo.AddInteraction(interaction);

            Assert.Equal(interaction, repo.Interactions.Single());
            Assert.Equal(interaction, repo.TestScopedInteractions.Single());
        }

        [Fact]
        public void AddInteraction_WhenAddingADuplicateTestScopedInteraction_ThrowsInvalidOperationException()
        {
            var interaction1 = new ProviderServiceInteraction { Description = "My description 1" };
            var interaction2 = new ProviderServiceInteraction { Description = "My description 1" };

            var repo = GetSubject();

            repo.AddInteraction(interaction1);

            Assert.Throws<InvalidOperationException>(() => repo.AddInteraction(interaction2));

            Assert.Equal(interaction1, repo.Interactions.Single());
            Assert.Equal(interaction1, repo.TestScopedInteractions.Single());
        }

        [Fact]
        public void AddInteraction_WhenAddingAnInteractionThatHasBeenAddedInAPreviousTest_InteractionIsDeDuplicatedFromInteractions()
        {
            var interaction1 = new ProviderServiceInteraction { Description = "My description 1" };
            var interaction2 = new ProviderServiceInteraction { Description = "My description 1" };

            var repo = GetSubject();

            repo.AddInteraction(interaction1);
            repo.ClearTestScopedInteractions();

            repo.AddInteraction(interaction2);

            Assert.Equal(interaction1, repo.Interactions.Single());
            Assert.Equal(interaction2, repo.TestScopedInteractions.Single());
        }

        [Fact]
        public void AddInteraction_WhenAddingAnInteractionThatHasBeenAddedInAPreviousTestHoweverTheDataIsDifferent_ThrowsInvalidOperationException()
        {
            var interaction1 = new ProviderServiceInteraction { Description = "My description 1", Request = new ProviderServiceRequest { Method = HttpVerb.Get } };
            var interaction2 = new ProviderServiceInteraction { Description = "My description 1", Request = new ProviderServiceRequest { Method = HttpVerb.Delete } };

            var repo = GetSubject();

            repo.AddInteraction(interaction1);
            repo.ClearTestScopedInteractions();

            Assert.Throws<InvalidOperationException>(() => repo.AddInteraction(interaction2));

            Assert.Equal(interaction1, repo.Interactions.Single());
            Assert.Empty(repo.TestScopedInteractions);
        }

        [Fact]
        public void AddHandledRequest_WithNullHandledRequest_ThrowsArgumentNullException()
        {
            var repo = GetSubject();

            Assert.Throws<ArgumentNullException>(() => repo.AddHandledRequest(null));
        }

        [Fact]
        public void AddHandledRequest_WithHandledRequest_AddsHandledRequest()
        {
            var handledRequest = new HandledRequest(new ProviderServiceRequest(), new ProviderServiceInteraction());

            var repo = GetSubject();

            repo.AddHandledRequest(handledRequest);

            Assert.Equal(handledRequest, repo.HandledRequests.First());
        }

        [Fact]
        public void GetMatchingTestScopedInteraction_WhenNoTestScopedInteractionsHaveBeenRegistered_ThrowsPactFailureException()
        {
            var repo = GetSubject();

            Assert.Throws<PactFailureException>(() => repo.GetMatchingTestScopedInteraction(HttpVerb.Get, "/"));
        }

        [Fact]
        public void GetMatchingTestScopedInteraction_WithNoMatchingTestScopedInteraction_ThrowsPactFailureException()
        {
            var interaction = new ProviderServiceInteraction
            {
                Description = "My description",
                Request = new ProviderServiceRequest
                {
                    Method = HttpVerb.Head,
                    Path = "/tester"
                },
                Response = new ProviderServiceResponse
                {
                    Status = (int)HttpStatusCode.NoContent
                }
            };

            var repo = GetSubject();

            repo.AddInteraction(interaction);

            Assert.Throws<PactFailureException>(() => repo.GetMatchingTestScopedInteraction(HttpVerb.Get, "/tester"));
        }

        [Fact]
        public void GetMatchingTestScopedInteraction_WithMoreThanOneMatchingTestScopedInteraction_ThrowsPactFailureException()
        {
            var interaction1 = new ProviderServiceInteraction
            {
                Description = "My description",
                Request = new ProviderServiceRequest
                {
                    Method = HttpVerb.Head,
                    Path = "/tester"
                },
                Response = new ProviderServiceResponse
                {
                    Status = (int)HttpStatusCode.NoContent
                }
            };

            var interaction2 = new ProviderServiceInteraction
            {
                Description = "My description 2",
                Request = new ProviderServiceRequest
                {
                    Method = HttpVerb.Head,
                    Path = "/tester"
                },
                Response = new ProviderServiceResponse
                {
                    Status = (int)HttpStatusCode.NoContent
                }
            };

            var repo = GetSubject();

            repo.AddInteraction(interaction1);
            repo.AddInteraction(interaction2);

            Assert.Throws<PactFailureException>(() => repo.GetMatchingTestScopedInteraction(HttpVerb.Head, "/tester"));
        }

        [Fact]
        public void GetMatchingTestScopedInteraction_WithOneMatchingTestScopedInteraction_ThrowsPactFailureException()
        {
            var expectedInteraction = new ProviderServiceInteraction
            {
                Description = "My description",
                Request = new ProviderServiceRequest
                {
                    Method = HttpVerb.Head,
                    Path = "/tester"
                },
                Response = new ProviderServiceResponse
                {
                    Status = (int)HttpStatusCode.NoContent
                }
            };

            var repo = GetSubject();

            repo.AddInteraction(expectedInteraction);

            var interaction = repo.GetMatchingTestScopedInteraction(HttpVerb.Head, "/tester");

            Assert.Equal(expectedInteraction, interaction);
        }

        [Fact]
        public void GetMatchingTestScopedInteraction_WithOneMatchingTestScopedInteraction_WithQueryString()
        {
            var expectedInteraction = new ProviderServiceInteraction
            {
                Description = "My description",
                Request = new ProviderServiceRequest
                {
                    Method = HttpVerb.Get,
                    Path = "/tester",
                    Query = "params=test"

                },
                Response = new ProviderServiceResponse
                {
                    Status = (int)HttpStatusCode.NoContent
                }
            };

            var repo = GetSubject();

            repo.AddInteraction(expectedInteraction);

            var interaction = repo.GetMatchingTestScopedInteraction(HttpVerb.Get, "/tester?params=test");

            Assert.Equal(expectedInteraction, interaction);
        }

        [Fact]
        public void ClearHandledRequests_WithNoHandledRequest_HandledRequestIsEmpty()
        {
            var repo = GetSubject();

            repo.ClearHandledRequests();

            Assert.Empty(repo.HandledRequests);
        }

        [Fact]
        public void ClearHandledRequests_WithHandledRequests_HandledRequestIsEmpty()
        {
            var handledRequest1 = new HandledRequest(new ProviderServiceRequest(), new ProviderServiceInteraction());
            var handledRequest2 = new HandledRequest(new ProviderServiceRequest(), new ProviderServiceInteraction());

            var repo = GetSubject();

            repo.AddHandledRequest(handledRequest1);
            repo.AddHandledRequest(handledRequest2);

            repo.ClearHandledRequests();

            Assert.Empty(repo.HandledRequests);
        }

        [Fact]
        public void ClearTestScopedInteractions_WithNoTestScopedInteractions_TestScopedInteractionsRequestIsEmpty()
        {
            var repo = GetSubject();

            repo.ClearTestScopedInteractions();

            Assert.Empty(repo.TestScopedInteractions);
        }

        [Fact]
        public void ClearTestScopedInteractions_WithTestScopedInteractions_TestScopedInteractionsRequestIsEmpty()
        {
            var interaction1 = new ProviderServiceInteraction { Description = "My description 1" };
            var interaction2 = new ProviderServiceInteraction { Description = "My description 2" };

            var repo = GetSubject();

            repo.AddInteraction(interaction1);
            repo.AddInteraction(interaction2);

            repo.ClearTestScopedInteractions();

            Assert.Empty(repo.TestScopedInteractions);
        }
    }
}
