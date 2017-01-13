using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using PactNet.Comparers;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Comparers;
using PactNet.Mocks.MockHttpService.Models;
using PactNet.Mocks.MockHttpService.Validators;
using PactNet.Models;
using PactNet.Reporters;
using Xunit;

namespace PactNet.Tests.Mocks.MockHttpService.Validators
{
    public class ProviderServiceValidatorTests
    {
        private IProviderServiceResponseComparer _mockResponseComparer;
        private IHttpRequestSender _mockHttpRequestSender;
        private IReporter _mockReporter;

        private IProviderServiceValidator GetSubject()
        {
            _mockResponseComparer = Substitute.For<IProviderServiceResponseComparer>();
            _mockHttpRequestSender = Substitute.For<IHttpRequestSender>();
            _mockReporter = Substitute.For<IReporter>();

            return new ProviderServiceValidator(_mockResponseComparer, _mockHttpRequestSender, _mockReporter, new PactVerifierConfig());
        }

        [Fact]
        public async Task Validate_WithNullPactFile_ThrowsArgumentException()
        {
            var validator = GetSubject();

            await Assert.ThrowsAsync<ArgumentException>(async () => await validator.Validate(null, null));
        }

        [Fact]
        public async Task Validate_WithNullConsumer_ThrowsArgumentException()
        {
            var pact = new ProviderServicePactFile
            {
                Provider = new Pacticipant { Name = "My Provider" }
            };

            var validator = GetSubject();

            await Assert.ThrowsAsync<ArgumentException>(async () => await validator.Validate(pact, null));

        }

        [Fact]
        public async Task Validate_WithNullConsumerName_ThrowsArgumentException()
        {
            var pact = new ProviderServicePactFile
            {
                Consumer = new Pacticipant(),
                Provider = new Pacticipant { Name = "My Provider" }
            };

            var validator = GetSubject();

            await Assert.ThrowsAsync<ArgumentException>(async () => await validator.Validate(pact, null));

        }

        [Fact]
        public async Task Validate_WithEmptyConsumerName_ThrowsArgumentException()
        {
            var pact = new ProviderServicePactFile
            {
                Consumer = new Pacticipant { Name = string.Empty },
                Provider = new Pacticipant { Name = "My Provider" }
            };

            var validator = GetSubject();

            await Assert.ThrowsAsync<ArgumentException>(async () => await validator.Validate(pact, null));

        }

        [Fact]
        public async Task Validate_WithNullProvider_ThrowsArgumentException()
        {
            var pact = new ProviderServicePactFile
            {
                Consumer = new Pacticipant { Name = "My client" },
            };

            var validator = GetSubject();

            await Assert.ThrowsAsync<ArgumentException>(async () => await validator.Validate(pact, null));

        }

        [Fact]
        public async Task Validate_WithNullProviderName_ThrowsArgumentException()
        {
            var pact = new ProviderServicePactFile
            {
                Consumer = new Pacticipant { Name = "My client" },
                Provider = new Pacticipant()
            };

            var validator = GetSubject();

            await Assert.ThrowsAsync<ArgumentException>(async () => await validator.Validate(pact, null));

        }

        [Fact]
        public async Task Validate_WithEmptyProviderName_ThrowsArgumentException()
        {
            var pact = new ProviderServicePactFile
            {
                Consumer = new Pacticipant { Name = "My client" },
                Provider = new Pacticipant { Name = string.Empty },
            };

            var validator = GetSubject();

            await Assert.ThrowsAsync<ArgumentException>(async () => await validator.Validate(pact, null));
        }

        [Fact]
        public void Validate_WithNullInteractionsInPactFile_DoesNotCallTheHttpRequestSenderOrRequestComparer()
        {
            var pact = new ProviderServicePactFile
            {
                Consumer = new Pacticipant { Name = "My client" },
                Provider = new Pacticipant { Name = "My Provider" }
            };

            var validator = GetSubject();

            validator.Validate(pact, null);

            _mockResponseComparer.Received(0).Compare(Arg.Any<ProviderServiceResponse>(), Arg.Any<ProviderServiceResponse>());
            _mockHttpRequestSender.Received(0).Send(Arg.Any<ProviderServiceRequest>());
        }

        [Fact]
        public void Validate_WithEmptyInteractionsInPactFile_DoesNotCallTheHttpRequestSenderOrRequestComparer()
        {
            var pact = new ProviderServicePactFile
            {
                Consumer = new Pacticipant { Name = "My client" },
                Provider = new Pacticipant { Name = "My Provider" },
                Interactions = new List<ProviderServiceInteraction>()
            };

            var validator = GetSubject();

            validator.Validate(pact, null);

            _mockResponseComparer.Received(0).Compare(Arg.Any<ProviderServiceResponse>(), Arg.Any<ProviderServiceResponse>());
            _mockHttpRequestSender.Received(0).Send(Arg.Any<ProviderServiceRequest>());
        }

        [Fact]
        public void Validate_WithInteractionsInPactFile_CallsSendOnHttpRequestSender()
        {
            var pact = new ProviderServicePactFile
            {
                Consumer = new Pacticipant { Name = "My client" },
                Provider = new Pacticipant { Name = "My Provider" },
                Interactions = new List<ProviderServiceInteraction>
                {
                    new ProviderServiceInteraction
                    {
                        Description = "My interaction",
                        Request = new ProviderServiceRequest(),
                        Response = new ProviderServiceResponse()
                    }
                }
            };

            var validator = GetSubject();

            validator.Validate(pact, null);

            _mockHttpRequestSender.Received(1).Send(pact.Interactions.First().Request);
        }

        [Fact]
        public void Validate_WithInteractionsInPactFile_CallsCompareOnTheProviderServiceResponseComparer()
        {
            var pact = new ProviderServicePactFile
            {
                Consumer = new Pacticipant { Name = "My client" },
                Provider = new Pacticipant { Name = "My Provider" },
                Interactions = new List<ProviderServiceInteraction>
                {
                    new ProviderServiceInteraction
                    {
                        Description = "My interaction",
                        Request = new ProviderServiceRequest(),
                        Response = new ProviderServiceResponse()
                    }
                }
            };

            var validator = GetSubject();

            validator.Validate(pact, null);

            _mockResponseComparer.Received(1).Compare(pact.Interactions.First().Response, Arg.Any<ProviderServiceResponse>());
        }

        [Fact]
        public async Task Validate_WhenProviderServiceResponseComparerThrowsPactFailureException_ThrowsPactFailureException()
        {
            var pact = new ProviderServicePactFile
            {
                Consumer = new Pacticipant { Name = "My client" },
                Provider = new Pacticipant { Name = "My Provider" },
                Interactions = new List<ProviderServiceInteraction>
                {
                    new ProviderServiceInteraction
                    {
                        Description = "My interaction"
                    }
                }
            };

            var validator = GetSubject();

            _mockResponseComparer
                .When(x => x.Compare(Arg.Any<ProviderServiceResponse>(), Arg.Any<ProviderServiceResponse>()))
                .Do(x => { throw new PactFailureException("Expected response cannot be null"); });

            await Assert.ThrowsAsync<PactFailureException>(async () => await validator.Validate(pact, null));
        }

        [Fact]
        public void Validate_WithNoInteractionsAndProviderStatesSetUpDefined_SetUpActionIsNotInvoked()
        {
            var actionInkoved = false;
            var pact = new ProviderServicePactFile
            {
                Consumer = new Pacticipant { Name = "My client" },
                Provider = new Pacticipant { Name = "My Provider" },
                Interactions = null
            };
            var providerStates = new ProviderStates(setUp: () => { actionInkoved = true; }, tearDown: null);

            var validator = GetSubject();

            validator.Validate(pact, providerStates);

            Assert.False(actionInkoved, "Provider states pact setUp action is not invoked");
        }

        [Fact]
        public void Validate_ProviderStatesSetUpDefined_SetUpActionIsInvokedForEachInteraction()
        {
            var actionInvocationCount = 0;
            var pact = new ProviderServicePactFile
            {
                Consumer = new Pacticipant { Name = "My client" },
                Provider = new Pacticipant { Name = "My Provider" },
                Interactions = new List<ProviderServiceInteraction>
                {
                    new ProviderServiceInteraction
                    {
                        Description = "My interaction"
                    },
                    new ProviderServiceInteraction
                    {
                        Description = "My interaction 2"
                    }
                }
            };
            var providerStates = new ProviderStates(setUp: () => { actionInvocationCount++; }, tearDown: null);

            var validator = GetSubject();

            validator.Validate(pact, providerStates);

            Assert.Equal(pact.Interactions.Count(), actionInvocationCount);
        }

        [Fact]
        public void Validate_ProviderStatesTearDownDefined_TearDownActionIsInvokedForEachInteraction()
        {
            var actionInvocationCount = 0;
            var pact = new ProviderServicePactFile
            {
                Consumer = new Pacticipant { Name = "My client" },
                Provider = new Pacticipant { Name = "My Provider" },
                Interactions = new List<ProviderServiceInteraction>
                {
                    new ProviderServiceInteraction
                    {
                        Description = "My interaction"
                    },
                    new ProviderServiceInteraction
                    {
                        Description = "My interaction 2"
                    }
                }
            };
            var providerStates = new ProviderStates(setUp: null, tearDown: () => { actionInvocationCount++; });

            var validator = GetSubject();

            validator.Validate(pact, providerStates);

            Assert.Equal(pact.Interactions.Count(), actionInvocationCount);
        }

        [Fact]
        public async Task Validate_WhenProviderServiceResponseComparerThrowsAndProviderStatesTearDownDefined_TearDownActionIsInvoked()
        {
            var actionInkoved = false;
            var pact = new ProviderServicePactFile
            {
                Consumer = new Pacticipant { Name = "My client" },
                Provider = new Pacticipant { Name = "My Provider" },
                Interactions = new List<ProviderServiceInteraction>
                {
                    new ProviderServiceInteraction
                    {
                        Description = "My interaction"
                    }
                }
            };
            var providerStates = new ProviderStates(setUp: null, tearDown: () => { actionInkoved = true; });

            var validator = GetSubject();

            _mockResponseComparer
                .When(x => x.Compare(Arg.Any<ProviderServiceResponse>(), Arg.Any<ProviderServiceResponse>()))
                .Do(x => { throw new PactFailureException("Expected response cannot be null"); });

            await Assert.ThrowsAsync<PactFailureException>(async () => await validator.Validate(pact, providerStates));

            Assert.True(actionInkoved, "Provider states pact tearDown action is invoked");
        }

        [Fact]
        public void Validate_WhenInteractionDefinesAProviderStateAndProviderStateSetUpDefined_SetUpActionIsInvoked()
        {
            var actionInkoved = false;
            var pact = new ProviderServicePactFile
            {
                Consumer = new Pacticipant { Name = "My client" },
                Provider = new Pacticipant { Name = "My Provider" },
                Interactions = new List<ProviderServiceInteraction>
                {
                    new ProviderServiceInteraction
                    {
                        ProviderState = "My Provider State",
                        Description = "My interaction"
                    }
                }
            };
            var providerStates = new ProviderStates();
            providerStates.Add(new ProviderState("My Provider State", setUp: () => { actionInkoved = true; }, tearDown: null));

            var validator = GetSubject();

            validator.Validate(pact, providerStates);

            Assert.True(actionInkoved, "Provider state setUp action is invoked");
        }

        [Fact]
        public void Validate_WhenInteractionDefinesAProviderStateAndProviderStateTearDownDefined_TearDownActionIsInvoked()
        {
            var actionInkoved = false;
            var pact = new ProviderServicePactFile
            {
                Consumer = new Pacticipant { Name = "My client" },
                Provider = new Pacticipant { Name = "My Provider" },
                Interactions = new List<ProviderServiceInteraction>
                {
                    new ProviderServiceInteraction
                    {
                        ProviderState = "My Provider State",
                        Description = "My interaction"
                    }
                }
            };
            var providerStates = new ProviderStates();
            providerStates.Add(new ProviderState("My Provider State", setUp: null, tearDown: () => { actionInkoved = true; }));

            var validator = GetSubject();

            validator.Validate(pact, providerStates);

            Assert.True(actionInkoved, "Provider state tearDown action is invoked");
        }

        [Fact]
        public async Task Validate_WhenInteractionDefinesAProviderStateAndProviderStateTearDownDefinedAndProviderServiceResponseComparerThrows_TearDownActionIsInvoked()
        {
            var actionInkoved = false;
            var pact = new ProviderServicePactFile
            {
                Consumer = new Pacticipant { Name = "My client" },
                Provider = new Pacticipant { Name = "My Provider" },
                Interactions = new List<ProviderServiceInteraction>
                {
                    new ProviderServiceInteraction
                    {
                        ProviderState = "My Provider State",
                        Description = "My interaction"
                    }
                }
            };
            var providerStates = new ProviderStates();
            providerStates.Add(new ProviderState("My Provider State", setUp: null, tearDown: () => { actionInkoved = true; }));

            var validator = GetSubject();

            _mockResponseComparer
                .When(x => x.Compare(Arg.Any<ProviderServiceResponse>(), Arg.Any<ProviderServiceResponse>()))
                .Do(x => { throw new PactFailureException("Expected response cannot be null"); });

            await Assert.ThrowsAsync<PactFailureException>(async () => await validator.Validate(pact, providerStates));

            Assert.True(actionInkoved, "Provider state tearDown action is invoked");
        }

        [Fact]
        public async Task Validate_WhenInteractionDefinesAProviderStateButNoProviderStateIsSupplied_ThrowsInvalidOperationException()
        {
            var pact = new ProviderServicePactFile
            {
                Consumer = new Pacticipant { Name = "My client" },
                Provider = new Pacticipant { Name = "My Provider" },
                Interactions = new List<ProviderServiceInteraction>
                {
                    new ProviderServiceInteraction
                    {
                        ProviderState = "My Provider State",
                        Description = "My interaction"
                    }
                }
            };
            var providerStates = new ProviderStates();

            var validator = GetSubject();

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await validator.Validate(pact, providerStates));
        }

        [Fact]
        public async Task Validate_WhenInteractionDefinesAProviderStateAndNoProviderStatesAreSupplied_ThrowsInvalidOperationException()
        {
            var pact = new ProviderServicePactFile
            {
                Consumer = new Pacticipant { Name = "My client" },
                Provider = new Pacticipant { Name = "My Provider" },
                Interactions = new List<ProviderServiceInteraction>
                {
                    new ProviderServiceInteraction
                    {
                        ProviderState = "My Provider State",
                        Description = "My interaction"
                    }
                }
            };

            var validator = GetSubject();

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await validator.Validate(pact, null));
        }

        [Fact]
        public async Task Validate_WhenInteractionDefinesAProviderStateAndProviderStateIsNotFound_ThrowsInvalidOperationException()
        {
            var pact = new ProviderServicePactFile
            {
                Consumer = new Pacticipant { Name = "My client" },
                Provider = new Pacticipant { Name = "My Provider" },
                Interactions = new List<ProviderServiceInteraction>
                {
                    new ProviderServiceInteraction
                    {
                        ProviderState = "My Provider State",
                        Description = "My interaction"
                    }
                }
            };

            var providerStates = new ProviderStates();
            providerStates.Add(new ProviderState("Some other provider state"));

            var validator = GetSubject();

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await validator.Validate(pact, providerStates));
        }

        [Fact]
        public void Validate_WhenInteractionDefinesAProviderStateAndProviderStateIsSuppliedWithNoSetUpOrTearDown_NoProviderStateSetUpOrTearDownActionsAreInvoked()
        {
            var pact = new ProviderServicePactFile
            {
                Consumer = new Pacticipant { Name = "My client" },
                Provider = new Pacticipant { Name = "My Provider" },
                Interactions = new List<ProviderServiceInteraction>
                {
                    new ProviderServiceInteraction
                    {
                        ProviderState = "My Provider State",
                        Description = "My interaction"
                    }
                }
            };
            var providerStates = new ProviderStates();
            providerStates.Add(new ProviderState("My Provider State"));

            var validator = GetSubject();

            validator.Validate(pact, providerStates);
        }

        [Fact]
        public void Validate_WithTwoInteractionsAndProviderStateSetupAndTearDownDefined_SetUpIsInvokedBeforeCompareAndTearDownIsInvokedAfterCompare()
        {
            const string providerState1 = "My Provider State";
            const string providerState2 = "My Provider State 2";

            const string setUpSuffix = "-SetUp";
            const string tearDownSuffix = "-TearDown";

            var actionInvocationLog = new List<string>();
            var pact = new ProviderServicePactFile
            {
                Consumer = new Pacticipant { Name = "My client" },
                Provider = new Pacticipant { Name = "My Provider" },
                Interactions = new List<ProviderServiceInteraction>
                {
                    new ProviderServiceInteraction
                    {
                        ProviderState = providerState1,
                        Description = "My interaction"
                    },
                    new ProviderServiceInteraction
                    {
                        ProviderState = providerState2,
                        Description = "My interaction"
                    },
                }
            };
            var providerStates = new ProviderStates();
            providerStates.Add(new ProviderState(providerState1, setUp: () => actionInvocationLog.Add(providerState1 + setUpSuffix), tearDown: () => actionInvocationLog.Add(providerState1 + tearDownSuffix)));
            providerStates.Add(new ProviderState(providerState2, setUp: () => actionInvocationLog.Add(providerState2 + setUpSuffix), tearDown: () => actionInvocationLog.Add(providerState2 + tearDownSuffix)));

            var validator = GetSubject();

            validator.Validate(pact, providerStates);

            Assert.Equal(4, actionInvocationLog.Count());
            Assert.Equal(providerState1 + setUpSuffix, actionInvocationLog.First());
            Assert.Equal(providerState1 + tearDownSuffix, actionInvocationLog.Skip(1).First());
            Assert.Equal(providerState2 + setUpSuffix, actionInvocationLog.Skip(2).First());
            Assert.Equal(providerState2 + tearDownSuffix, actionInvocationLog.Last());
        }

        [Fact]
        public async Task Validate_WhenAFailureOccurs_ThrowsPactFailureException()
        {
            var pact = new ProviderServicePactFile
            {
                Consumer = new Pacticipant { Name = "My client" },
                Provider = new Pacticipant { Name = "My Provider" },
                Interactions = new List<ProviderServiceInteraction>
                {
                    new ProviderServiceInteraction
                    {
                        Description = "My interaction"
                    },
                }
            };

            var comparisonResult = new ComparisonResult();
            comparisonResult.RecordFailure(new ErrorMessageComparisonFailure("It failed"));

            var validator = GetSubject();

            _mockResponseComparer
                .Compare(Arg.Any<ProviderServiceResponse>(), Arg.Any<ProviderServiceResponse>())
                .Returns(comparisonResult);

            await Assert.ThrowsAsync<PactFailureException>(async () => await validator.Validate(pact, null));
        }
    }
}
