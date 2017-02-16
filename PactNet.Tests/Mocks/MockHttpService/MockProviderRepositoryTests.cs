using System;
using System.Linq;
using System.Net;
using NSubstitute;
using PactNet.Comparers;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Comparers;
using PactNet.Models.ProviderService;
using Xunit;

namespace PactNet.Tests.Mocks.MockHttpService
{
    public class MockProviderRepositoryTests
    {
        private IProviderServiceRequestComparer _mockComparer;

        private IMockProviderRepository GetSubject()
        {
            _mockComparer = Substitute.For<IProviderServiceRequestComparer>();

            return new MockProviderRepository(_mockComparer);
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
        public void AddInteraction_WhenAddingADuplicateTestScopedInteraction_ThrowsPactFailureException()
        {
            var interaction1 = new ProviderServiceInteraction { Description = "My description 1" };
            var interaction2 = new ProviderServiceInteraction { Description = "My description 1" };

            var repo = GetSubject();

            repo.AddInteraction(interaction1);

            Assert.Throws<PactFailureException>(() => repo.AddInteraction(interaction2));

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
            repo.ClearTestScopedState();

            repo.AddInteraction(interaction2);

            Assert.Equal(interaction1, repo.Interactions.Single());
            Assert.Equal(interaction2, repo.TestScopedInteractions.Single());
        }

        [Fact]
        public void AddInteraction_WhenAddingAnInteractionThatHasBeenAddedInAPreviousTestHoweverTheDataIsDifferent_ThrowsPactFailureException()
        {
            var interaction1 = new ProviderServiceInteraction { Description = "My description 1", Request = new ProviderServiceRequest { Method = HttpVerb.Get } };
            var interaction2 = new ProviderServiceInteraction { Description = "My description 1", Request = new ProviderServiceRequest { Method = HttpVerb.Delete } };

            var repo = GetSubject();

            repo.AddInteraction(interaction1);
            repo.ClearTestScopedState();

            Assert.Throws<PactFailureException>(() => repo.AddInteraction(interaction2));

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
            var expectedRequest = new ProviderServiceRequest
            {
                Method = HttpVerb.Get,
                Path = "/"
            };

            var repo = GetSubject();

            Assert.Throws<PactFailureException>(() => repo.GetMatchingTestScopedInteraction(expectedRequest));
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
                    Status = (int) HttpStatusCode.NoContent
                }
            };

            var nonMatchingRequest = new ProviderServiceRequest
            {
                Method = HttpVerb.Get,
                Path = "/tester"
            };

            var repo = GetSubject();

            repo.AddInteraction(interaction);

            Assert.Throws<PactFailureException>(() => repo.GetMatchingTestScopedInteraction(nonMatchingRequest));
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

            Assert.Throws<PactFailureException>(() => repo.GetMatchingTestScopedInteraction(interaction1.Request));
        }

        [Fact]
        public void GetMatchingTestScopedInteraction_WithOneMatchingTestScopedInteraction_DoesNotThrowAPactFailureException()
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

            _mockComparer
                .Compare(expectedInteraction.Request, expectedInteraction.Request)
                .Returns(new ComparisonResult());

            repo.AddInteraction(expectedInteraction);

            var interaction = repo.GetMatchingTestScopedInteraction(expectedInteraction.Request);

            Assert.Equal(expectedInteraction, interaction);
        }

        [Fact]
        public void ClearTestScopedState_WithNoHandledRequest_HandledRequestIsEmpty()
        {
            var repo = GetSubject();

            repo.ClearTestScopedState();

            Assert.Empty(repo.HandledRequests);
        }

        [Fact]
        public void ClearTestScopedState_WithHandledRequests_HandledRequestIsEmpty()
        {
            var handledRequest1 = new HandledRequest(new ProviderServiceRequest(), new ProviderServiceInteraction());
            var handledRequest2 = new HandledRequest(new ProviderServiceRequest(), new ProviderServiceInteraction());

            var repo = GetSubject();

            repo.AddHandledRequest(handledRequest1);
            repo.AddHandledRequest(handledRequest2);

            repo.ClearTestScopedState();

            Assert.Empty(repo.HandledRequests);
        }

        [Fact]
        public void ClearTestScopedState_WithNoTestScopedInteractions_TestScopedInteractionsRequestIsEmpty()
        {
            var repo = GetSubject();

            repo.ClearTestScopedState();

            Assert.Empty(repo.TestScopedInteractions);
        }

        [Fact]
        public void ClearTestScopedState_WithTestScopedInteractions_TestScopedInteractionsRequestIsEmpty()
        {
            var interaction1 = new ProviderServiceInteraction { Description = "My description 1" };
            var interaction2 = new ProviderServiceInteraction { Description = "My description 2" };

            var repo = GetSubject();

            repo.AddInteraction(interaction1);
            repo.AddInteraction(interaction2);

            repo.ClearTestScopedState();

            Assert.Empty(repo.TestScopedInteractions);
        }

        [Fact]
        public void ClearTestScopedState_WithTestContextSet_TestContextIsNull()
        {
            var repo = GetSubject();

            repo.TestContext = "Blah blah";

            repo.ClearTestScopedState();

            Assert.Null(repo.TestContext);
        }
    }
}
