using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
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

            return new ProviderServiceValidator(_mockResponseComparer, _mockHttpRequestSender, _mockReporter);
        }

        [Fact]
        public void Validate_WithNullPactFile_ThrowsArgumentException()
        {
            var validator = GetSubject();

            Assert.Throws<ArgumentException>(() => validator.Validate(null, null));
        }

        [Fact]
        public void Validate_WithNullConsumer_ThrowsArgumentException()
        {
            var pact = new ProviderServicePactFile
            {
                Provider = new Party { Name = "My Provider" }
            };

            var validator = GetSubject();

            Assert.Throws<ArgumentException>(() => validator.Validate(pact, null));
        }

        [Fact]
        public void Validate_WithNullConsumerName_ThrowsArgumentException()
        {
            var pact = new ProviderServicePactFile
            {
                Consumer = new Party(),
                Provider = new Party { Name = "My Provider" }
            };

            var validator = GetSubject();

            Assert.Throws<ArgumentException>(() => validator.Validate(pact, null));
        }

        [Fact]
        public void Validate_WithEmptyConsumerName_ThrowsArgumentException()
        {
            var pact = new ProviderServicePactFile
            {
                Consumer = new Party { Name = String.Empty },
                Provider = new Party { Name = "My Provider" }
            };

            var validator = GetSubject();

            Assert.Throws<ArgumentException>(() => validator.Validate(pact, null));
        }

        [Fact]
        public void Validate_WithNullProvider_ThrowsArgumentException()
        {
            var pact = new ProviderServicePactFile
            {
                Consumer = new Party { Name = "My client" },
            };

            var validator = GetSubject();

            Assert.Throws<ArgumentException>(() => validator.Validate(pact, null));
        }

        [Fact]
        public void Validate_WithNullProviderName_ThrowsArgumentException()
        {
            var pact = new ProviderServicePactFile
            {
                Consumer = new Party { Name = "My client" },
                Provider = new Party()
            };

            var validator = GetSubject();

            Assert.Throws<ArgumentException>(() => validator.Validate(pact, null));
        }

        [Fact]
        public void Validate_WithEmptyProviderName_ThrowsArgumentException()
        {
            var pact = new ProviderServicePactFile
            {
                Consumer = new Party { Name = "My client" },
                Provider = new Party { Name = String.Empty },
            };

            var validator = GetSubject();

            Assert.Throws<ArgumentException>(() => validator.Validate(pact, null));
        }

        [Fact]
        public void Validate_WithNullInteractionsInPactFile_DoesNotCallTheHttpRequestSenderOrRequestComparer()
        {
            var pact = new ProviderServicePactFile
            {
                Consumer = new Party { Name = "My client" },
                Provider = new Party { Name = "My Provider" }
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
                Consumer = new Party { Name = "My client" },
                Provider = new Party { Name = "My Provider" },
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
                Consumer = new Party { Name = "My client" },
                Provider = new Party { Name = "My Provider" },
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
                Consumer = new Party { Name = "My client" },
                Provider = new Party { Name = "My Provider" },
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
        public void Validate_WhenProviderServiceResponseComparerThrowsPactFailureException_ThrowsPactFailureException()
        {
            var pact = new ProviderServicePactFile
            {
                Consumer = new Party { Name = "My client" },
                Provider = new Party { Name = "My Provider" },
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

            Assert.Throws<PactFailureException>(() => validator.Validate(pact, null));
        }

        [Fact]
        public void Validate_WithNoInteractionsAndProviderStatesSetUpDefined_SetUpActionIsNotInvoked()
        {
            var actionInkoved = false;
            var pact = new ProviderServicePactFile
            {
                Consumer = new Party { Name = "My client" },
                Provider = new Party { Name = "My Provider" },
                Interactions = null
            };
            var providerStates = new ProviderStates(setUp: () => { actionInkoved = true; }, tearDown: null);

            var validator = GetSubject();

            validator.Validate(pact, providerStates);

            Assert.False(actionInkoved, "Provider states pact setUp action is not invoked");
        }

        [Fact]
        public void Validate_ProviderStatesSetUpDefined_SetUpActionIsInvoked()
        {
            var actionInkoved = false;
            var pact = new ProviderServicePactFile
            {
                Consumer = new Party { Name = "My client" },
                Provider = new Party { Name = "My Provider" },
                Interactions = new List<ProviderServiceInteraction>
                {
                    new ProviderServiceInteraction
                    {
                        Description = "My interaction"
                    }
                }
            };
            var providerStates = new ProviderStates(setUp: () => { actionInkoved = true; }, tearDown: null);

            var validator = GetSubject();

            validator.Validate(pact, providerStates);

            Assert.True(actionInkoved, "Provider states pact setUp action is invoked");
        }

        [Fact]
        public void Validate_ProviderStatesTearDownDefined_TearDownActionIsInvoked()
        {
            var actionInkoved = false;
            var pact = new ProviderServicePactFile
            {
                Consumer = new Party { Name = "My client" },
                Provider = new Party { Name = "My Provider" },
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

            validator.Validate(pact, providerStates);

            Assert.True(actionInkoved, "Provider states pact tearDown action is invoked");
        }

        [Fact]
        public void Validate_WhenProviderServiceResponseComparerThrowsAndProviderStatesTearDownDefined_TearDownActionIsInvoked()
        {
            var actionInkoved = false;
            var pact = new ProviderServicePactFile
            {
                Consumer = new Party { Name = "My client" },
                Provider = new Party { Name = "My Provider" },
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

            Assert.Throws<PactFailureException>(() => validator.Validate(pact, providerStates));

            Assert.True(actionInkoved, "Provider states pact tearDown action is invoked");
        }

        [Fact]
        public void Validate_WhenInteractionDefinesAProviderStateAndProviderStateSetUpDefined_SetUpActionIsInvoked()
        {
            var actionInkoved = false;
            var pact = new ProviderServicePactFile
            {
                Consumer = new Party { Name = "My client" },
                Provider = new Party { Name = "My Provider" },
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
                Consumer = new Party { Name = "My client" },
                Provider = new Party { Name = "My Provider" },
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
        public void Validate_WhenInteractionDefinesAProviderStateAndProviderStateTearDownDefinedAndProviderServiceResponseComparerThrows_TearDownActionIsInvoked()
        {
            var actionInkoved = false;
            var pact = new ProviderServicePactFile
            {
                Consumer = new Party { Name = "My client" },
                Provider = new Party { Name = "My Provider" },
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

            Assert.Throws<PactFailureException>(() => validator.Validate(pact, providerStates));

            Assert.True(actionInkoved, "Provider state tearDown action is invoked");
        }

        [Fact]
        public void Validate_WhenInteractionDefinesAProviderStateButNoProviderStateIsSupplied_ThrowsInvalidOperationException()
        {
            var pact = new ProviderServicePactFile
            {
                Consumer = new Party { Name = "My client" },
                Provider = new Party { Name = "My Provider" },
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

            Assert.Throws<InvalidOperationException>(() => validator.Validate(pact, providerStates));
        }

        [Fact]
        public void Validate_WhenInteractionDefinesAProviderStateAndNoProviderStatesAreSupplied_ThrowsInvalidOperationException()
        {
            var pact = new ProviderServicePactFile
            {
                Consumer = new Party { Name = "My client" },
                Provider = new Party { Name = "My Provider" },
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

            Assert.Throws<InvalidOperationException>(() => validator.Validate(pact, null));
        }

        [Fact]
        public void Validate_WhenInteractionDefinesAProviderStateAndProviderStateIsNotFound_ThrowsInvalidOperationException()
        {
            var pact = new ProviderServicePactFile
            {
                Consumer = new Party { Name = "My client" },
                Provider = new Party { Name = "My Provider" },
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

            Assert.Throws<InvalidOperationException>(() => validator.Validate(pact, providerStates));
        }

        [Fact]
        public void Validate_WhenInteractionDefinesAProviderStateAndProviderStateIsSuppliedWithNoSetUpOrTearDown_NoProviderStateSetUpOrTearDownActionsAreInvoked()
        {
            var pact = new ProviderServicePactFile
            {
                Consumer = new Party { Name = "My client" },
                Provider = new Party { Name = "My Provider" },
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
                Consumer = new Party { Name = "My client" },
                Provider = new Party { Name = "My Provider" },
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
        public void Validate_WhenReporterHasErrors_ThrowsPactFailureException()
        {
            var pact = new ProviderServicePactFile
            {
                Consumer = new Party { Name = "My client" },
                Provider = new Party { Name = "My Provider" },
                Interactions = new List<ProviderServiceInteraction>
                {
                    new ProviderServiceInteraction
                    {
                        Description = "My interaction"
                    },
                }
            };

            var validator = GetSubject();

            _mockReporter
                .When(x => x.ThrowIfAnyErrors())
                .Do(info => { throw new PactFailureException("Compare failed"); });

            Assert.Throws<PactFailureException>(() => validator.Validate(pact, null));
            _mockReporter.Received(1).ThrowIfAnyErrors();
        }
    }
}
