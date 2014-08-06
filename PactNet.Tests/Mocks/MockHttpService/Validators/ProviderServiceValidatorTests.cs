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
using PactNet.Reporters;
using PactNet.Tests.Fakes;
using Xunit;

namespace PactNet.Tests.Mocks.MockHttpService.Validators
{
    public class ProviderServiceValidatorTests
    {
        private IProviderServiceValidator GetSubject()
        {
            return new ProviderServiceValidator(new HttpClient(), new Reporter(new NoOpReportOutputter()));
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
            var pact = new ProviderServicePactFile
            {
                Provider = new Party { Name = "My Provider" }
            };
            var providerServiceValidator = GetSubject();

            Assert.Throws<ArgumentException>(() => providerServiceValidator.Validate(pact, null));
        }

        [Fact]
        public void Validate_WithNullConsumerName_ThrowsArgumentException()
        {
            var pact = new ProviderServicePactFile
            {
                Consumer = new Party(),
                Provider = new Party { Name = "My Provider" }
            };
            var providerServiceValidator = GetSubject();

            Assert.Throws<ArgumentException>(() => providerServiceValidator.Validate(pact, null));
        }

        [Fact]
        public void Validate_WithEmptyConsumerName_ThrowsArgumentException()
        {
            var pact = new ProviderServicePactFile
            {
                Consumer = new Party { Name = String.Empty },
                Provider = new Party { Name = "My Provider" }
            };
            var providerServiceValidator = GetSubject();

            Assert.Throws<ArgumentException>(() => providerServiceValidator.Validate(pact, null));
        }

        [Fact]
        public void Validate_WithNullProvider_ThrowsArgumentException()
        {
            var pact = new ProviderServicePactFile
            {
                Consumer = new Party { Name = "My client" },
            };
            var providerServiceValidator = GetSubject();

            Assert.Throws<ArgumentException>(() => providerServiceValidator.Validate(pact, null));
        }

        [Fact]
        public void Validate_WithNullProviderName_ThrowsArgumentException()
        {
            var pact = new ProviderServicePactFile
            {
                Consumer = new Party { Name = "My client" },
                Provider = new Party()
            };
            var providerServiceValidator = GetSubject();

            Assert.Throws<ArgumentException>(() => providerServiceValidator.Validate(pact, null));
        }

        [Fact]
        public void Validate_WithEmptyProviderName_ThrowsArgumentException()
        {
            var pact = new ProviderServicePactFile
            {
                Consumer = new Party { Name = "My client" },
                Provider = new Party { Name = String.Empty },
            };
            var providerServiceValidator = GetSubject();

            Assert.Throws<ArgumentException>(() => providerServiceValidator.Validate(pact, null));
        }

        [Fact]
        public void Validate_WithNullInteractionsInPactFile_DoesNotCallHttpClientOrAnyOfTheMappersOrValidators()
        {
            var pact = new ProviderServicePactFile
            {
                Consumer = new Party { Name = "My client" },
                Provider = new Party { Name = "My Provider" }
            };
            var mockProviderServiceResponseComparer = Substitute.For<IProviderServiceResponseComparer>();
            var fakeHttpClient = new FakeHttpClient();
            var mockHttpRequestMessageMapper = Substitute.For<IHttpRequestMessageMapper>();
            var mockProviderServiceResponseMapper = Substitute.For<IProviderServiceResponseMapper>();

            var providerServiceValidator = new ProviderServiceValidator(
                mockProviderServiceResponseComparer,
                fakeHttpClient, 
                mockHttpRequestMessageMapper, 
                mockProviderServiceResponseMapper,
                new Reporter(new NoOpReportOutputter()));

            providerServiceValidator.Validate(pact, null);

            mockProviderServiceResponseComparer.Received(0).Compare(Arg.Any<ProviderServiceResponse>(), Arg.Any<ProviderServiceResponse>());
            Assert.Equal(0, fakeHttpClient.SendAsyncCallCount);
            mockHttpRequestMessageMapper.Received(0).Convert(Arg.Any<ProviderServiceRequest>());
            mockProviderServiceResponseMapper.Received(0).Convert(Arg.Any<HttpResponseMessage>());
        }

        [Fact]
        public void Validate_WithEmptyInteractionsInPactFile_DoesNotCallHttpClientOrAnyOfTheMappersOrValidators()
        {
            var pact = new ProviderServicePactFile
            {
                Consumer = new Party { Name = "My client" },
                Provider = new Party { Name = "My Provider" },
                Interactions = new List<ProviderServiceInteraction>()
            };
            var mockProviderServiceResponseComparer = Substitute.For<IProviderServiceResponseComparer>();
            var fakeHttpClient = new FakeHttpClient();
            var mockHttpRequestMessageMapper = Substitute.For<IHttpRequestMessageMapper>();
            var mockProviderServiceResponseMapper = Substitute.For<IProviderServiceResponseMapper>();

            var providerServiceValidator = new ProviderServiceValidator(
                mockProviderServiceResponseComparer,
                fakeHttpClient,
                mockHttpRequestMessageMapper,
                mockProviderServiceResponseMapper,
                new Reporter(new NoOpReportOutputter()));

            providerServiceValidator.Validate(pact, null);
            Assert.Equal(0, fakeHttpClient.SendAsyncCallCount);
            mockProviderServiceResponseComparer.Received(0).Compare(Arg.Any<ProviderServiceResponse>(), Arg.Any<ProviderServiceResponse>());
            mockHttpRequestMessageMapper.Received(0).Convert(Arg.Any<ProviderServiceRequest>());
            mockProviderServiceResponseMapper.Received(0).Convert(Arg.Any<HttpResponseMessage>());
        }

        [Fact]
        public void Validate_WithInteractionsInPactFile_CallsConvertOnTheHttpRequestMessageMapper()
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
            var mockProviderServiceResponseComparer = Substitute.For<IProviderServiceResponseComparer>();
            var fakeHttpClient = new FakeHttpClient();
            var mockHttpRequestMessageMapper = Substitute.For<IHttpRequestMessageMapper>();
            var mockProviderServiceResponseMapper = Substitute.For<IProviderServiceResponseMapper>();

            var providerServiceValidator = new ProviderServiceValidator(
                mockProviderServiceResponseComparer,
                fakeHttpClient,
                mockHttpRequestMessageMapper,
                mockProviderServiceResponseMapper,
                new Reporter(new NoOpReportOutputter()));

            providerServiceValidator.Validate(pact, null);

            mockHttpRequestMessageMapper.Received(1).Convert(Arg.Any<ProviderServiceRequest>());
        }

        [Fact]
        public void Validate_WithInteractionsInPactFile_CallsConvertOnTheProviderServiceResponseMapper()
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
            var mockProviderServiceResponseComparer = Substitute.For<IProviderServiceResponseComparer>();
            var fakeHttpClient = new FakeHttpClient();
            var mockHttpRequestMessageMapper = Substitute.For<IHttpRequestMessageMapper>();
            var mockProviderServiceResponseMapper = Substitute.For<IProviderServiceResponseMapper>();

            var providerServiceValidator = new ProviderServiceValidator(
                mockProviderServiceResponseComparer,
                fakeHttpClient,
                mockHttpRequestMessageMapper,
                mockProviderServiceResponseMapper,
                new Reporter(new NoOpReportOutputter()));

            providerServiceValidator.Validate(pact, null);

            mockProviderServiceResponseMapper.Received(1).Convert(Arg.Any<HttpResponseMessage>());
        }

        [Fact]
        public void Validate_WithInteractionsInPactFile_CallsValidateOnTheProviderServiceResponseValidator()
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
            var mockProviderServiceResponseComparer = Substitute.For<IProviderServiceResponseComparer>();
            var fakeHttpClient = new FakeHttpClient();
            var mockHttpRequestMessageMapper = Substitute.For<IHttpRequestMessageMapper>();
            var mockProviderServiceResponseMapper = Substitute.For<IProviderServiceResponseMapper>();

            var providerServiceValidator = new ProviderServiceValidator(
                mockProviderServiceResponseComparer,
                fakeHttpClient,
                mockHttpRequestMessageMapper,
                mockProviderServiceResponseMapper,
                new Reporter(new NoOpReportOutputter()));

            providerServiceValidator.Validate(pact, null);

            mockProviderServiceResponseComparer.Received(1).Compare(Arg.Any<ProviderServiceResponse>(), Arg.Any<ProviderServiceResponse>());
        }

        [Fact]
        public void Validate_WhenProviderServiceResponseValidatorThrowsACompareFailedException_ThrowsACompareFailedException()
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
            var mockProviderServiceResponseComparer = Substitute.For<IProviderServiceResponseComparer>();
            var fakeHttpClient = new FakeHttpClient();
            var mockHttpRequestMessageMapper = Substitute.For<IHttpRequestMessageMapper>();
            var mockProviderServiceResponseMapper = Substitute.For<IProviderServiceResponseMapper>();

            mockProviderServiceResponseComparer
                .When(x => x.Compare(Arg.Any<ProviderServiceResponse>(), Arg.Any<ProviderServiceResponse>()))
                .Do(x => { throw new CompareFailedException("Expected response cannot be null"); });

            var providerServiceValidator = new ProviderServiceValidator(
                mockProviderServiceResponseComparer,
                fakeHttpClient,
                mockHttpRequestMessageMapper,
                mockProviderServiceResponseMapper,
                new Reporter(new NoOpReportOutputter()));

            Assert.Throws<CompareFailedException>(() => providerServiceValidator.Validate(pact, null));
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

            var mockProviderServiceResponseComparer = Substitute.For<IProviderServiceResponseComparer>();
            var fakeHttpClient = new FakeHttpClient();
            var mockHttpRequestMessageMapper = Substitute.For<IHttpRequestMessageMapper>();
            var mockProviderServiceResponseMapper = Substitute.For<IProviderServiceResponseMapper>();

            var providerServiceValidator = new ProviderServiceValidator(
                mockProviderServiceResponseComparer,
                fakeHttpClient,
                mockHttpRequestMessageMapper,
                mockProviderServiceResponseMapper,
                new Reporter(new NoOpReportOutputter()));

            providerServiceValidator.Validate(pact, providerStates);

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

            var mockProviderServiceResponseComparer = Substitute.For<IProviderServiceResponseComparer>();
            var fakeHttpClient = new FakeHttpClient();
            var mockHttpRequestMessageMapper = Substitute.For<IHttpRequestMessageMapper>();
            var mockProviderServiceResponseMapper = Substitute.For<IProviderServiceResponseMapper>();

            var providerServiceValidator = new ProviderServiceValidator(
                mockProviderServiceResponseComparer,
                fakeHttpClient,
                mockHttpRequestMessageMapper,
                mockProviderServiceResponseMapper,
                new Reporter(new NoOpReportOutputter()));

            providerServiceValidator.Validate(pact, providerStates);

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

            var mockProviderServiceResponseComparer = Substitute.For<IProviderServiceResponseComparer>();
            var fakeHttpClient = new FakeHttpClient();
            var mockHttpRequestMessageMapper = Substitute.For<IHttpRequestMessageMapper>();
            var mockProviderServiceResponseMapper = Substitute.For<IProviderServiceResponseMapper>();

            var providerServiceValidator = new ProviderServiceValidator(
                mockProviderServiceResponseComparer,
                fakeHttpClient,
                mockHttpRequestMessageMapper,
                mockProviderServiceResponseMapper,
                new Reporter(new NoOpReportOutputter()));

            providerServiceValidator.Validate(pact, providerStates);

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

            var mockProviderServiceResponseComparer = Substitute.For<IProviderServiceResponseComparer>();
            var fakeHttpClient = new FakeHttpClient();
            var mockHttpRequestMessageMapper = Substitute.For<IHttpRequestMessageMapper>();
            var mockProviderServiceResponseMapper = Substitute.For<IProviderServiceResponseMapper>();

            mockProviderServiceResponseComparer
                .When(x => x.Compare(Arg.Any<ProviderServiceResponse>(), Arg.Any<ProviderServiceResponse>()))
                .Do(x => { throw new CompareFailedException("Expected response cannot be null"); });

            var providerServiceValidator = new ProviderServiceValidator(
                mockProviderServiceResponseComparer,
                fakeHttpClient,
                mockHttpRequestMessageMapper,
                mockProviderServiceResponseMapper,
                new Reporter(new NoOpReportOutputter()));

            Assert.Throws<CompareFailedException>(() => providerServiceValidator.Validate(pact, providerStates));

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

            var mockProviderServiceResponseComparer = Substitute.For<IProviderServiceResponseComparer>();
            var fakeHttpClient = new FakeHttpClient();
            var mockHttpRequestMessageMapper = Substitute.For<IHttpRequestMessageMapper>();
            var mockProviderServiceResponseMapper = Substitute.For<IProviderServiceResponseMapper>();

            var providerServiceValidator = new ProviderServiceValidator(
                mockProviderServiceResponseComparer,
                fakeHttpClient,
                mockHttpRequestMessageMapper,
                mockProviderServiceResponseMapper,
                new Reporter(new NoOpReportOutputter()));

            providerServiceValidator.Validate(pact, providerStates);

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

            var mockProviderServiceResponseComparer = Substitute.For<IProviderServiceResponseComparer>();
            var fakeHttpClient = new FakeHttpClient();
            var mockHttpRequestMessageMapper = Substitute.For<IHttpRequestMessageMapper>();
            var mockProviderServiceResponseMapper = Substitute.For<IProviderServiceResponseMapper>();

            var providerServiceValidator = new ProviderServiceValidator(
                mockProviderServiceResponseComparer,
                fakeHttpClient,
                mockHttpRequestMessageMapper,
                mockProviderServiceResponseMapper,
                new Reporter(new NoOpReportOutputter()));

            providerServiceValidator.Validate(pact, providerStates);

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

            var mockProviderServiceResponseComparer = Substitute.For<IProviderServiceResponseComparer>();
            var fakeHttpClient = new FakeHttpClient();
            var mockHttpRequestMessageMapper = Substitute.For<IHttpRequestMessageMapper>();
            var mockProviderServiceResponseMapper = Substitute.For<IProviderServiceResponseMapper>();

            mockProviderServiceResponseComparer
                .When(x => x.Compare(Arg.Any<ProviderServiceResponse>(), Arg.Any<ProviderServiceResponse>()))
                .Do(x => { throw new CompareFailedException("Expected response cannot be null"); });

            var providerServiceValidator = new ProviderServiceValidator(
                mockProviderServiceResponseComparer,
                fakeHttpClient,
                mockHttpRequestMessageMapper,
                mockProviderServiceResponseMapper,
                new Reporter(new NoOpReportOutputter()));

            Assert.Throws<CompareFailedException>(() => providerServiceValidator.Validate(pact, providerStates));

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

            var mockProviderServiceResponseComparer = Substitute.For<IProviderServiceResponseComparer>();
            var fakeHttpClient = new FakeHttpClient();
            var mockHttpRequestMessageMapper = Substitute.For<IHttpRequestMessageMapper>();
            var mockProviderServiceResponseMapper = Substitute.For<IProviderServiceResponseMapper>();

            var providerServiceValidator = new ProviderServiceValidator(
                mockProviderServiceResponseComparer,
                fakeHttpClient,
                mockHttpRequestMessageMapper,
                mockProviderServiceResponseMapper,
                new Reporter(new NoOpReportOutputter()));

            Assert.Throws<InvalidOperationException>(() => providerServiceValidator.Validate(pact, providerStates));
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

            var mockProviderServiceResponseComparer = Substitute.For<IProviderServiceResponseComparer>();
            var fakeHttpClient = new FakeHttpClient();
            var mockHttpRequestMessageMapper = Substitute.For<IHttpRequestMessageMapper>();
            var mockProviderServiceResponseMapper = Substitute.For<IProviderServiceResponseMapper>();

            var providerServiceValidator = new ProviderServiceValidator(
                mockProviderServiceResponseComparer,
                fakeHttpClient,
                mockHttpRequestMessageMapper,
                mockProviderServiceResponseMapper,
                new Reporter(new NoOpReportOutputter()));

            Assert.Throws<InvalidOperationException>(() => providerServiceValidator.Validate(pact, null));
        }

        [Fact]
        public void Validate_WhenInteractionDefinesAProviderStateAndProviderStateIsNoFound_ThrowsInvalidOperationException()
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

            var mockProviderServiceResponseComparer = Substitute.For<IProviderServiceResponseComparer>();
            var fakeHttpClient = new FakeHttpClient();
            var mockHttpRequestMessageMapper = Substitute.For<IHttpRequestMessageMapper>();
            var mockProviderServiceResponseMapper = Substitute.For<IProviderServiceResponseMapper>();

            var providerServiceValidator = new ProviderServiceValidator(
                mockProviderServiceResponseComparer,
                fakeHttpClient,
                mockHttpRequestMessageMapper,
                mockProviderServiceResponseMapper,
                new Reporter(new NoOpReportOutputter()));

            Assert.Throws<InvalidOperationException>(() => providerServiceValidator.Validate(pact, providerStates));
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

            var mockProviderServiceResponseComparer = Substitute.For<IProviderServiceResponseComparer>();
            var fakeHttpClient = new FakeHttpClient();
            var mockHttpRequestMessageMapper = Substitute.For<IHttpRequestMessageMapper>();
            var mockProviderServiceResponseMapper = Substitute.For<IProviderServiceResponseMapper>();

            var providerServiceValidator = new ProviderServiceValidator(
                mockProviderServiceResponseComparer,
                fakeHttpClient,
                mockHttpRequestMessageMapper,
                mockProviderServiceResponseMapper,
                new Reporter(new NoOpReportOutputter()));

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

            var mockProviderServiceResponseComparer = Substitute.For<IProviderServiceResponseComparer>();
            var fakeHttpClient = new FakeHttpClient();
            var mockHttpRequestMessageMapper = Substitute.For<IHttpRequestMessageMapper>();
            var mockProviderServiceResponseMapper = Substitute.For<IProviderServiceResponseMapper>();

            var providerServiceValidator = new ProviderServiceValidator(
                mockProviderServiceResponseComparer,
                fakeHttpClient,
                mockHttpRequestMessageMapper,
                mockProviderServiceResponseMapper,
                new Reporter(new NoOpReportOutputter()));

            providerServiceValidator.Validate(pact, providerStates);

            Assert.Equal(4, actionInvocationLog.Count());
            Assert.Equal(providerState1 + setUpSuffix, actionInvocationLog.First());
            Assert.Equal(providerState1 + tearDownSuffix, actionInvocationLog.Skip(1).First());
            Assert.Equal(providerState2 + setUpSuffix, actionInvocationLog.Skip(2).First());
            Assert.Equal(providerState2 + tearDownSuffix, actionInvocationLog.Last());
        }

        [Fact]
        public void Validate_WhenReporterHasErrors_ThrowsCompareFailedException()
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

            var mockProviderServiceResponseComparer = Substitute.For<IProviderServiceResponseComparer>();
            var fakeHttpClient = new FakeHttpClient();
            var mockHttpRequestMessageMapper = Substitute.For<IHttpRequestMessageMapper>();
            var mockProviderServiceResponseMapper = Substitute.For<IProviderServiceResponseMapper>();
            var mockReporter = Substitute.For<IReporter>();

            mockReporter
                .When(x => x.ThrowIfAnyErrors())
                .Do(info => { throw new CompareFailedException("Compare failed"); });

            var providerServiceValidator = new ProviderServiceValidator(
                mockProviderServiceResponseComparer,
                fakeHttpClient,
                mockHttpRequestMessageMapper,
                mockProviderServiceResponseMapper,
                mockReporter);

            Assert.Throws<CompareFailedException>(() => providerServiceValidator.Validate(pact, null));
            mockReporter.Received(1).ThrowIfAnyErrors();
        }
    }
}
