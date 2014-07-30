using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using NSubstitute;
using PactNet.Mocks.MockHttpService.Comparers;
using PactNet.Mocks.MockHttpService.Mappers;
using PactNet.Mocks.MockHttpService.Models;
using PactNet.Mocks.MockHttpService.Validators;
using PactNet.Models;
using PactNet.Tests.Fakes;
using Xunit;

namespace PactNet.Tests.Mocks.MockHttpService.Validators
{
    public class ProviderServiceValidatorTests
    {
        private IProviderServiceValidator GetSubject()
        {
            return new ProviderServiceValidator(new HttpClient());
        }

        [Fact]
        public void Validate_WithNullPactFile_ThrowsArgumentException()
        {
            var providerServiceValidator = GetSubject();

            Assert.Throws<ArgumentException>(() => providerServiceValidator.Validate(null, null));
        }

        [Fact]
        public void Validate_WithNullConsumer_ThrowsArgumentException()
        {
            var pact = new ServicePactFile
            {
                Provider = new PactParty { Name = "My Provider" }
            };
            var providerServiceValidator = GetSubject();

            Assert.Throws<ArgumentException>(() => providerServiceValidator.Validate(pact, null));
        }

        [Fact]
        public void Validate_WithNullConsumerName_ThrowsArgumentException()
        {
            var pact = new ServicePactFile
            {
                Consumer = new PactParty(),
                Provider = new PactParty { Name = "My Provider" }
            };
            var providerServiceValidator = GetSubject();

            Assert.Throws<ArgumentException>(() => providerServiceValidator.Validate(pact, null));
        }

        [Fact]
        public void Validate_WithEmptyConsumerName_ThrowsArgumentException()
        {
            var pact = new ServicePactFile
            {
                Consumer = new PactParty { Name = String.Empty },
                Provider = new PactParty { Name = "My Provider" }
            };
            var providerServiceValidator = GetSubject();

            Assert.Throws<ArgumentException>(() => providerServiceValidator.Validate(pact, null));
        }

        [Fact]
        public void Validate_WithNullProvider_ThrowsArgumentException()
        {
            var pact = new ServicePactFile
            {
                Consumer = new PactParty { Name = "My client" },
            };
            var providerServiceValidator = GetSubject();

            Assert.Throws<ArgumentException>(() => providerServiceValidator.Validate(pact, null));
        }

        [Fact]
        public void Validate_WithNullProviderName_ThrowsArgumentException()
        {
            var pact = new ServicePactFile
            {
                Consumer = new PactParty { Name = "My client" },
                Provider = new PactParty()
            };
            var providerServiceValidator = GetSubject();

            Assert.Throws<ArgumentException>(() => providerServiceValidator.Validate(pact, null));
        }

        [Fact]
        public void Validate_WithEmptyProviderName_ThrowsArgumentException()
        {
            var pact = new ServicePactFile
            {
                Consumer = new PactParty { Name = "My client" },
                Provider = new PactParty { Name = String.Empty },
            };
            var providerServiceValidator = GetSubject();

            Assert.Throws<ArgumentException>(() => providerServiceValidator.Validate(pact, null));
        }

        [Fact]
        public void Validate_WithNullInteractionsInPactFile_DoesNotCallHttpClientOrAnyOfTheMappersOrValidators()
        {
            var pact = new ServicePactFile
            {
                Consumer = new PactParty { Name = "My client" },
                Provider = new PactParty { Name = "My Provider" }
            };
            var mockProviderServiceResponseComparer = Substitute.For<IPactProviderServiceResponseComparer>();
            var fakeHttpClient = new FakeHttpClient();
            var mockHttpRequestMessageMapper = Substitute.For<IHttpRequestMessageMapper>();
            var mockPactProviderServiceResponseMapper = Substitute.For<IPactProviderServiceResponseMapper>();

            var providerServiceValidator = new ProviderServiceValidator(
                mockProviderServiceResponseComparer,
                fakeHttpClient, 
                mockHttpRequestMessageMapper, 
                mockPactProviderServiceResponseMapper);

            providerServiceValidator.Validate(pact, null);

            mockProviderServiceResponseComparer.Received(0).Compare(Arg.Any<PactProviderServiceResponse>(), Arg.Any<PactProviderServiceResponse>());
            Assert.Equal(0, fakeHttpClient.SendAsyncCallCount);
            mockHttpRequestMessageMapper.Received(0).Convert(Arg.Any<PactProviderServiceRequest>());
            mockPactProviderServiceResponseMapper.Received(0).Convert(Arg.Any<HttpResponseMessage>());
        }

        [Fact]
        public void Validate_WithEmptyInteractionsInPactFile_DoesNotCallHttpClientOrAnyOfTheMappersOrValidators()
        {
            var pact = new ServicePactFile
            {
                Consumer = new PactParty { Name = "My client" },
                Provider = new PactParty { Name = "My Provider" },
                Interactions = new List<PactServiceInteraction>()
            };
            var mockProviderServiceResponseComparer = Substitute.For<IPactProviderServiceResponseComparer>();
            var fakeHttpClient = new FakeHttpClient();
            var mockHttpRequestMessageMapper = Substitute.For<IHttpRequestMessageMapper>();
            var mockPactProviderServiceResponseMapper = Substitute.For<IPactProviderServiceResponseMapper>();

            var providerServiceValidator = new ProviderServiceValidator(
                mockProviderServiceResponseComparer,
                fakeHttpClient,
                mockHttpRequestMessageMapper,
                mockPactProviderServiceResponseMapper);

            providerServiceValidator.Validate(pact, null);
            Assert.Equal(0, fakeHttpClient.SendAsyncCallCount);
            mockProviderServiceResponseComparer.Received(0).Compare(Arg.Any<PactProviderServiceResponse>(), Arg.Any<PactProviderServiceResponse>());
            mockHttpRequestMessageMapper.Received(0).Convert(Arg.Any<PactProviderServiceRequest>());
            mockPactProviderServiceResponseMapper.Received(0).Convert(Arg.Any<HttpResponseMessage>());
        }

        [Fact]
        public void Validate_WithInteractionsInPactFile_CallsConvertOnTheHttpRequestMessageMapper()
        {
            var pact = new ServicePactFile
            {
                Consumer = new PactParty { Name = "My client" },
                Provider = new PactParty { Name = "My Provider" },
                Interactions = new List<PactServiceInteraction>
                {
                    new PactServiceInteraction
                    {
                        Description = "My interaction"
                    }
                }
            };
            var mockProviderServiceResponseComparer = Substitute.For<IPactProviderServiceResponseComparer>();
            var fakeHttpClient = new FakeHttpClient();
            var mockHttpRequestMessageMapper = Substitute.For<IHttpRequestMessageMapper>();
            var mockPactProviderServiceResponseMapper = Substitute.For<IPactProviderServiceResponseMapper>();

            var providerServiceValidator = new ProviderServiceValidator(
                mockProviderServiceResponseComparer,
                fakeHttpClient,
                mockHttpRequestMessageMapper,
                mockPactProviderServiceResponseMapper);

            providerServiceValidator.Validate(pact, null);

            mockHttpRequestMessageMapper.Received(1).Convert(Arg.Any<PactProviderServiceRequest>());
        }

        [Fact]
        public void Validate_WithInteractionsInPactFile_CallsConvertOnTheProviderServiceResponseMapper()
        {
            var pact = new ServicePactFile
            {
                Consumer = new PactParty { Name = "My client" },
                Provider = new PactParty { Name = "My Provider" },
                Interactions = new List<PactServiceInteraction>
                {
                    new PactServiceInteraction
                    {
                        Description = "My interaction"
                    }
                }
            };
            var mockProviderServiceResponseComparer = Substitute.For<IPactProviderServiceResponseComparer>();
            var fakeHttpClient = new FakeHttpClient();
            var mockHttpRequestMessageMapper = Substitute.For<IHttpRequestMessageMapper>();
            var mockPactProviderServiceResponseMapper = Substitute.For<IPactProviderServiceResponseMapper>();

            var providerServiceValidator = new ProviderServiceValidator(
                mockProviderServiceResponseComparer,
                fakeHttpClient,
                mockHttpRequestMessageMapper,
                mockPactProviderServiceResponseMapper);

            providerServiceValidator.Validate(pact, null);

            mockPactProviderServiceResponseMapper.Received(1).Convert(Arg.Any<HttpResponseMessage>());
        }

        [Fact]
        public void Validate_WithInteractionsInPactFile_CallsValidateOnTheProviderServiceResponseValidator()
        {
            var pact = new ServicePactFile
            {
                Consumer = new PactParty { Name = "My client" },
                Provider = new PactParty { Name = "My Provider" },
                Interactions = new List<PactServiceInteraction>
                {
                    new PactServiceInteraction
                    {
                        Description = "My interaction"
                    }
                }
            };
            var mockProviderServiceResponseComparer = Substitute.For<IPactProviderServiceResponseComparer>();
            var fakeHttpClient = new FakeHttpClient();
            var mockHttpRequestMessageMapper = Substitute.For<IHttpRequestMessageMapper>();
            var mockPactProviderServiceResponseMapper = Substitute.For<IPactProviderServiceResponseMapper>();

            var providerServiceValidator = new ProviderServiceValidator(
                mockProviderServiceResponseComparer,
                fakeHttpClient,
                mockHttpRequestMessageMapper,
                mockPactProviderServiceResponseMapper);

            providerServiceValidator.Validate(pact, null);

            mockProviderServiceResponseComparer.Received(1).Compare(Arg.Any<PactProviderServiceResponse>(), Arg.Any<PactProviderServiceResponse>());
        }

        [Fact]
        public void Validate_WhenProviderServiceResponseValidatorThrowsACompareFailedException_ThrowsACompareFailedException()
        {
            var pact = new ServicePactFile
            {
                Consumer = new PactParty { Name = "My client" },
                Provider = new PactParty { Name = "My Provider" },
                Interactions = new List<PactServiceInteraction>
                {
                    new PactServiceInteraction
                    {
                        Description = "My interaction"
                    }
                }
            };
            var mockProviderServiceResponseComparer = Substitute.For<IPactProviderServiceResponseComparer>();
            var fakeHttpClient = new FakeHttpClient();
            var mockHttpRequestMessageMapper = Substitute.For<IHttpRequestMessageMapper>();
            var mockPactProviderServiceResponseMapper = Substitute.For<IPactProviderServiceResponseMapper>();

            mockProviderServiceResponseComparer
                .When(x => x.Compare(Arg.Any<PactProviderServiceResponse>(), Arg.Any<PactProviderServiceResponse>()))
                .Do(x => { throw new CompareFailedException("Expected response cannot be null"); });

            var providerServiceValidator = new ProviderServiceValidator(
                mockProviderServiceResponseComparer,
                fakeHttpClient,
                mockHttpRequestMessageMapper,
                mockPactProviderServiceResponseMapper);

            Assert.Throws<CompareFailedException>(() => providerServiceValidator.Validate(pact, null));
        }

        [Fact]
        public void Validate_WithNoInteractionsAndProviderStatesSetUpDefined_SetUpActionIsNotInvoked()
        {
            var actionInkoved = false;
            var pact = new ServicePactFile
            {
                Consumer = new PactParty { Name = "My client" },
                Provider = new PactParty { Name = "My Provider" },
                Interactions = null
            };
            var providerStates = new ProviderStates(setUp: () => { actionInkoved = true; }, tearDown: null);

            var mockProviderServiceResponseComparer = Substitute.For<IPactProviderServiceResponseComparer>();
            var fakeHttpClient = new FakeHttpClient();
            var mockHttpRequestMessageMapper = Substitute.For<IHttpRequestMessageMapper>();
            var mockPactProviderServiceResponseMapper = Substitute.For<IPactProviderServiceResponseMapper>();

            var providerServiceValidator = new ProviderServiceValidator(
                mockProviderServiceResponseComparer,
                fakeHttpClient,
                mockHttpRequestMessageMapper,
                mockPactProviderServiceResponseMapper);

            providerServiceValidator.Validate(pact, providerStates);

            Assert.False(actionInkoved, "Provider states pact setUp action is not invoked");
        }

        [Fact]
        public void Validate_ProviderStatesSetUpDefined_SetUpActionIsInvoked()
        {
            var actionInkoved = false;
            var pact = new ServicePactFile
            {
                Consumer = new PactParty { Name = "My client" },
                Provider = new PactParty { Name = "My Provider" },
                Interactions = new List<PactServiceInteraction>
                {
                    new PactServiceInteraction
                    {
                        Description = "My interaction"
                    }
                }
            };
            var providerStates = new ProviderStates(setUp: () => { actionInkoved = true; }, tearDown: null);

            var mockProviderServiceResponseComparer = Substitute.For<IPactProviderServiceResponseComparer>();
            var fakeHttpClient = new FakeHttpClient();
            var mockHttpRequestMessageMapper = Substitute.For<IHttpRequestMessageMapper>();
            var mockPactProviderServiceResponseMapper = Substitute.For<IPactProviderServiceResponseMapper>();

            var providerServiceValidator = new ProviderServiceValidator(
                mockProviderServiceResponseComparer,
                fakeHttpClient,
                mockHttpRequestMessageMapper,
                mockPactProviderServiceResponseMapper);

            providerServiceValidator.Validate(pact, providerStates);

            Assert.True(actionInkoved, "Provider states pact setUp action is invoked");
        }

        [Fact]
        public void Validate_ProviderStatesTearDownDefined_TearDownActionIsInvoked()
        {
            var actionInkoved = false;
            var pact = new ServicePactFile
            {
                Consumer = new PactParty { Name = "My client" },
                Provider = new PactParty { Name = "My Provider" },
                Interactions = new List<PactServiceInteraction>
                {
                    new PactServiceInteraction
                    {
                        Description = "My interaction"
                    }
                }
            };
            var providerStates = new ProviderStates(setUp: null, tearDown: () => { actionInkoved = true; });

            var mockProviderServiceResponseComparer = Substitute.For<IPactProviderServiceResponseComparer>();
            var fakeHttpClient = new FakeHttpClient();
            var mockHttpRequestMessageMapper = Substitute.For<IHttpRequestMessageMapper>();
            var mockPactProviderServiceResponseMapper = Substitute.For<IPactProviderServiceResponseMapper>();

            var providerServiceValidator = new ProviderServiceValidator(
                mockProviderServiceResponseComparer,
                fakeHttpClient,
                mockHttpRequestMessageMapper,
                mockPactProviderServiceResponseMapper);

            providerServiceValidator.Validate(pact, providerStates);

            Assert.True(actionInkoved, "Provider states pact tearDown action is invoked");
        }

        [Fact]
        public void Validate_WhenProviderServiceResponseComparerThrowsAndProviderStatesTearDownDefined_TearDownActionIsInvoked()
        {
            var actionInkoved = false;
            var pact = new ServicePactFile
            {
                Consumer = new PactParty { Name = "My client" },
                Provider = new PactParty { Name = "My Provider" },
                Interactions = new List<PactServiceInteraction>
                {
                    new PactServiceInteraction
                    {
                        Description = "My interaction"
                    }
                }
            };
            var providerStates = new ProviderStates(setUp: null, tearDown: () => { actionInkoved = true; });

            var mockProviderServiceResponseComparer = Substitute.For<IPactProviderServiceResponseComparer>();
            var fakeHttpClient = new FakeHttpClient();
            var mockHttpRequestMessageMapper = Substitute.For<IHttpRequestMessageMapper>();
            var mockPactProviderServiceResponseMapper = Substitute.For<IPactProviderServiceResponseMapper>();

            mockProviderServiceResponseComparer
                .When(x => x.Compare(Arg.Any<PactProviderServiceResponse>(), Arg.Any<PactProviderServiceResponse>()))
                .Do(x => { throw new CompareFailedException("Expected response cannot be null"); });

            var providerServiceValidator = new ProviderServiceValidator(
                mockProviderServiceResponseComparer,
                fakeHttpClient,
                mockHttpRequestMessageMapper,
                mockPactProviderServiceResponseMapper);

            Assert.Throws<CompareFailedException>(() => providerServiceValidator.Validate(pact, providerStates));

            Assert.True(actionInkoved, "Provider states pact tearDown action is invoked");
        }

        [Fact]
        public void Validate_WhenInteractionDefinesAProviderStateAndProviderStateSetUpDefined_SetUpActionIsInvoked()
        {
            var actionInkoved = false;
            var pact = new ServicePactFile
            {
                Consumer = new PactParty { Name = "My client" },
                Provider = new PactParty { Name = "My Provider" },
                Interactions = new List<PactServiceInteraction>
                {
                    new PactServiceInteraction
                    {
                        ProviderState = "My Provider State",
                        Description = "My interaction"
                    }
                }
            };
            var providerStates = new ProviderStates();
            providerStates.Add(new ProviderState("My Provider State", setUp: () => { actionInkoved = true; }, tearDown: null));

            var mockProviderServiceResponseComparer = Substitute.For<IPactProviderServiceResponseComparer>();
            var fakeHttpClient = new FakeHttpClient();
            var mockHttpRequestMessageMapper = Substitute.For<IHttpRequestMessageMapper>();
            var mockPactProviderServiceResponseMapper = Substitute.For<IPactProviderServiceResponseMapper>();

            var providerServiceValidator = new ProviderServiceValidator(
                mockProviderServiceResponseComparer,
                fakeHttpClient,
                mockHttpRequestMessageMapper,
                mockPactProviderServiceResponseMapper);

            providerServiceValidator.Validate(pact, providerStates);

            Assert.True(actionInkoved, "Provider state setUp action is invoked");
        }

        [Fact]
        public void Validate_WhenInteractionDefinesAProviderStateAndProviderStateTearDownDefined_TearDownActionIsInvoked()
        {
            var actionInkoved = false;
            var pact = new ServicePactFile
            {
                Consumer = new PactParty { Name = "My client" },
                Provider = new PactParty { Name = "My Provider" },
                Interactions = new List<PactServiceInteraction>
                {
                    new PactServiceInteraction
                    {
                        ProviderState = "My Provider State",
                        Description = "My interaction"
                    }
                }
            };
            var providerStates = new ProviderStates();
            providerStates.Add(new ProviderState("My Provider State", setUp: null, tearDown: () => { actionInkoved = true; }));

            var mockProviderServiceResponseComparer = Substitute.For<IPactProviderServiceResponseComparer>();
            var fakeHttpClient = new FakeHttpClient();
            var mockHttpRequestMessageMapper = Substitute.For<IHttpRequestMessageMapper>();
            var mockPactProviderServiceResponseMapper = Substitute.For<IPactProviderServiceResponseMapper>();

            var providerServiceValidator = new ProviderServiceValidator(
                mockProviderServiceResponseComparer,
                fakeHttpClient,
                mockHttpRequestMessageMapper,
                mockPactProviderServiceResponseMapper);

            providerServiceValidator.Validate(pact, providerStates);

            Assert.True(actionInkoved, "Provider state tearDown action is invoked");
        }

        [Fact]
        public void Validate_WhenInteractionDefinesAProviderStateAndProviderStateTearDownDefinedAndProviderServiceResponseComparerThrows_TearDownActionIsInvoked()
        {
            var actionInkoved = false;
            var pact = new ServicePactFile
            {
                Consumer = new PactParty { Name = "My client" },
                Provider = new PactParty { Name = "My Provider" },
                Interactions = new List<PactServiceInteraction>
                {
                    new PactServiceInteraction
                    {
                        ProviderState = "My Provider State",
                        Description = "My interaction"
                    }
                }
            };
            var providerStates = new ProviderStates();
            providerStates.Add(new ProviderState("My Provider State", setUp: null, tearDown: () => { actionInkoved = true; }));

            var mockProviderServiceResponseComparer = Substitute.For<IPactProviderServiceResponseComparer>();
            var fakeHttpClient = new FakeHttpClient();
            var mockHttpRequestMessageMapper = Substitute.For<IHttpRequestMessageMapper>();
            var mockPactProviderServiceResponseMapper = Substitute.For<IPactProviderServiceResponseMapper>();

            mockProviderServiceResponseComparer
                .When(x => x.Compare(Arg.Any<PactProviderServiceResponse>(), Arg.Any<PactProviderServiceResponse>()))
                .Do(x => { throw new CompareFailedException("Expected response cannot be null"); });

            var providerServiceValidator = new ProviderServiceValidator(
                mockProviderServiceResponseComparer,
                fakeHttpClient,
                mockHttpRequestMessageMapper,
                mockPactProviderServiceResponseMapper);

            Assert.Throws<CompareFailedException>(() => providerServiceValidator.Validate(pact, providerStates));

            Assert.True(actionInkoved, "Provider state tearDown action is invoked");
        }

        [Fact]
        public void Validate_WhenInteractionDefinesAProviderStateButNoProviderStateIsSupplied_ThrowsInvalidOperationException()
        {
            var pact = new ServicePactFile
            {
                Consumer = new PactParty { Name = "My client" },
                Provider = new PactParty { Name = "My Provider" },
                Interactions = new List<PactServiceInteraction>
                {
                    new PactServiceInteraction
                    {
                        ProviderState = "My Provider State",
                        Description = "My interaction"
                    }
                }
            };
            var providerStates = new ProviderStates();

            var mockProviderServiceResponseComparer = Substitute.For<IPactProviderServiceResponseComparer>();
            var fakeHttpClient = new FakeHttpClient();
            var mockHttpRequestMessageMapper = Substitute.For<IHttpRequestMessageMapper>();
            var mockPactProviderServiceResponseMapper = Substitute.For<IPactProviderServiceResponseMapper>();

            var providerServiceValidator = new ProviderServiceValidator(
                mockProviderServiceResponseComparer,
                fakeHttpClient,
                mockHttpRequestMessageMapper,
                mockPactProviderServiceResponseMapper);

            Assert.Throws<InvalidOperationException>(() => providerServiceValidator.Validate(pact, providerStates));
        }

        [Fact]
        public void Validate_WhenInteractionDefinesAProviderStateAndNoProviderStatesAreSupplied_ThrowsInvalidOperationException()
        {
            var pact = new ServicePactFile
            {
                Consumer = new PactParty { Name = "My client" },
                Provider = new PactParty { Name = "My Provider" },
                Interactions = new List<PactServiceInteraction>
                {
                    new PactServiceInteraction
                    {
                        ProviderState = "My Provider State",
                        Description = "My interaction"
                    }
                }
            };

            var mockProviderServiceResponseComparer = Substitute.For<IPactProviderServiceResponseComparer>();
            var fakeHttpClient = new FakeHttpClient();
            var mockHttpRequestMessageMapper = Substitute.For<IHttpRequestMessageMapper>();
            var mockPactProviderServiceResponseMapper = Substitute.For<IPactProviderServiceResponseMapper>();

            var providerServiceValidator = new ProviderServiceValidator(
                mockProviderServiceResponseComparer,
                fakeHttpClient,
                mockHttpRequestMessageMapper,
                mockPactProviderServiceResponseMapper);

            Assert.Throws<InvalidOperationException>(() => providerServiceValidator.Validate(pact, null));
        }

        [Fact]
        public void Validate_WhenInteractionDefinesAProviderStateAndProviderStateIsNoFound_ThrowsInvalidOperationException()
        {
            var pact = new ServicePactFile
            {
                Consumer = new PactParty { Name = "My client" },
                Provider = new PactParty { Name = "My Provider" },
                Interactions = new List<PactServiceInteraction>
                {
                    new PactServiceInteraction
                    {
                        ProviderState = "My Provider State",
                        Description = "My interaction"
                    }
                }
            };

            var providerStates = new ProviderStates();
            providerStates.Add(new ProviderState("Some other provider state"));

            var mockProviderServiceResponseComparer = Substitute.For<IPactProviderServiceResponseComparer>();
            var fakeHttpClient = new FakeHttpClient();
            var mockHttpRequestMessageMapper = Substitute.For<IHttpRequestMessageMapper>();
            var mockPactProviderServiceResponseMapper = Substitute.For<IPactProviderServiceResponseMapper>();

            var providerServiceValidator = new ProviderServiceValidator(
                mockProviderServiceResponseComparer,
                fakeHttpClient,
                mockHttpRequestMessageMapper,
                mockPactProviderServiceResponseMapper);

            Assert.Throws<InvalidOperationException>(() => providerServiceValidator.Validate(pact, providerStates));
        }

        [Fact]
        public void Validate_WhenInteractionDefinesAProviderStateAndProviderStateIsSuppliedWithNoSetUpOrTearDown_NoProviderStateSetUpOrTearDownActionsAreInvoked()
        {
            var pact = new ServicePactFile
            {
                Consumer = new PactParty { Name = "My client" },
                Provider = new PactParty { Name = "My Provider" },
                Interactions = new List<PactServiceInteraction>
                {
                    new PactServiceInteraction
                    {
                        ProviderState = "My Provider State",
                        Description = "My interaction"
                    }
                }
            };
            var providerStates = new ProviderStates();
            providerStates.Add(new ProviderState("My Provider State"));

            var mockProviderServiceResponseComparer = Substitute.For<IPactProviderServiceResponseComparer>();
            var fakeHttpClient = new FakeHttpClient();
            var mockHttpRequestMessageMapper = Substitute.For<IHttpRequestMessageMapper>();
            var mockPactProviderServiceResponseMapper = Substitute.For<IPactProviderServiceResponseMapper>();

            var providerServiceValidator = new ProviderServiceValidator(
                mockProviderServiceResponseComparer,
                fakeHttpClient,
                mockHttpRequestMessageMapper,
                mockPactProviderServiceResponseMapper);

            providerServiceValidator.Validate(pact, providerStates);
        }

        [Fact]
        public void Validate_WithTwoInteractionsAndProviderStateSetupAndTearDownDefined_SetUpIsInvokedBeforeCompareAndTearDownIsInvokedAfterCompare()
        {
            const string providerState1 = "My Provider State";
            const string providerState2 = "My Provider State 2";

            const string setUpSuffix = "-SetUp";
            const string tearDownSuffix = "-TearDown";

            var actionInvocationLog = new List<string>();
            var pact = new ServicePactFile
            {
                Consumer = new PactParty { Name = "My client" },
                Provider = new PactParty { Name = "My Provider" },
                Interactions = new List<PactServiceInteraction>
                {
                    new PactServiceInteraction
                    {
                        ProviderState = providerState1,
                        Description = "My interaction"
                    },
                    new PactServiceInteraction
                    {
                        ProviderState = providerState2,
                        Description = "My interaction"
                    },
                }
            };
            var providerStates = new ProviderStates();
            providerStates.Add(new ProviderState(providerState1, setUp: () => actionInvocationLog.Add(providerState1 + setUpSuffix), tearDown: () => actionInvocationLog.Add(providerState1 + tearDownSuffix)));
            providerStates.Add(new ProviderState(providerState2, setUp: () => actionInvocationLog.Add(providerState2 + setUpSuffix), tearDown: () => actionInvocationLog.Add(providerState2 + tearDownSuffix)));

            var mockProviderServiceResponseComparer = Substitute.For<IPactProviderServiceResponseComparer>();
            var fakeHttpClient = new FakeHttpClient();
            var mockHttpRequestMessageMapper = Substitute.For<IHttpRequestMessageMapper>();
            var mockPactProviderServiceResponseMapper = Substitute.For<IPactProviderServiceResponseMapper>();

            var providerServiceValidator = new ProviderServiceValidator(
                mockProviderServiceResponseComparer,
                fakeHttpClient,
                mockHttpRequestMessageMapper,
                mockPactProviderServiceResponseMapper);

            providerServiceValidator.Validate(pact, providerStates);

            Assert.Equal(4, actionInvocationLog.Count());
            Assert.Equal(providerState1 + setUpSuffix, actionInvocationLog.First());
            Assert.Equal(providerState1 + tearDownSuffix, actionInvocationLog.Skip(1).First());
            Assert.Equal(providerState2 + setUpSuffix, actionInvocationLog.Skip(2).First());
            Assert.Equal(providerState2 + tearDownSuffix, actionInvocationLog.Last());
        }
    }
}
