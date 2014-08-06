using System;
using System.Collections.Generic;
using NSubstitute;
using PactNet.Mocks.MockHttpService.Comparers;
using PactNet.Mocks.MockHttpService.Models;
using PactNet.Reporters;
using Xunit;

namespace PactNet.Tests.Mocks.MockHttpService.Comparers
{
    public class ProviderServiceResponseComparerTests
    {
        private IReporter _mockReporter;

        private IProviderServiceResponseComparer GetSubject()
        {
            _mockReporter = Substitute.For<IReporter>();
            return new ProviderServiceResponseComparer(_mockReporter);
        }

        [Fact]
        public void Compare_WithMatchingStatusCodes_NoExceptionsAreThrown()
        {
            var expected = new ProviderServiceResponse
            {
                Status = 201
            };

            var actual = new ProviderServiceResponse
            {
                Status = 201
            };

            var comparer = GetSubject();

            comparer.Compare(expected, actual);
        }

        [Fact]
        public void Compare_WithNonMatchingStatusCodes_ReportErrorIsCalled()
        {
            var expected = new ProviderServiceResponse
            {
                Status = 201
            };

            var actual = new ProviderServiceResponse
            {
                Status = 400
            };

            var comparer = GetSubject();

            comparer.Compare(expected, actual);
            _mockReporter.Received(1).ReportError(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<object>());
        }

        [Fact]
        public void Compare_WithMatchingHeaders_NoExceptionsAreThrown()
        {
            var expected = new ProviderServiceResponse
            {
                Status = 201,
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                }
            };

            var actual = new ProviderServiceResponse
            {
                Status = 201,
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                }
            };

            var comparer = GetSubject();

            comparer.Compare(expected, actual);
        }

        [Fact]
        public void Compare_WithMatchingHeadersButWithDifferentCasingOnName_NoExceptionsAreThrown()
        {
            var expected = new ProviderServiceResponse
            {
                Status = 201,
                Headers = new Dictionary<string, string>
                {
                    { "content-Type", "application/json" }
                }
            };

            var actual = new ProviderServiceResponse
            {
                Status = 201,
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                }
            };

            var comparer = GetSubject();

            comparer.Compare(expected, actual);
        }

        [Fact]
        public void Compare_WithMatchingHeadersButWithDifferentCasingOnValue_ReportErrorIsCalled()
        {
            var expected = new ProviderServiceResponse
            {
                Status = 201,
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "Application/Json" }
                }
            };

            var actual = new ProviderServiceResponse
            {
                Status = 201,
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                }
            };

            var comparer = GetSubject();

            comparer.Compare(expected, actual);
            _mockReporter.Received(1).ReportError(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<object>());
        }

        [Fact]
        public void Compare_WithMatchingHeadersButResponseHasAdditionalHeaders_NoExceptionsAreThrown()
        {
            var expected = new ProviderServiceResponse
            {
                Status = 201,
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                }
            };

            var actual = new ProviderServiceResponse
            {
                Status = 201,
                Headers = new Dictionary<string, string>
                {
                    { "X-Test", "MyCustomThing" },
                    { "X-Test-2", "MyCustomThing2" },
                    { "Content-Type", "application/json" }
                }
            };

            var comparer = GetSubject();

            comparer.Compare(expected, actual);
        }

        [Fact]
        public void Compare_WithNonMatchingHeadersValues_ReportErrorIsCalled()
        {
            var expected = new ProviderServiceResponse
            {
                Status = 201,
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                }
            };

            var actual = new ProviderServiceResponse
            {
                Status = 201,
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "text/plain" }
                }
            };

            var comparer = GetSubject();

            comparer.Compare(expected, actual);
            _mockReporter.Received(1).ReportError(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<object>());
        }

        [Fact]
        public void Compare_WithNonMatchingHeaderNames_ReportErrorIsCalled()
        {
            var expected = new ProviderServiceResponse
            {
                Status = 201,
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                }
            };

            var actual = new ProviderServiceResponse
            {
                Status = 201,
                Headers = new Dictionary<string, string>
                {
                    {"X-Test", "Tester"}
                }
            };

            var comparer = GetSubject();

            comparer.Compare(expected, actual);
            _mockReporter.Received(1).ReportError(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<object>());
        }

        [Fact]
        public void Compare_WithResponseThatHasNoHeaders_ReportErrorIsCalled()
        {
            var expected = new ProviderServiceResponse
            {
                Status = 201,
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                }
            };

            var actual = new ProviderServiceResponse
            {
                Status = 201
            };

            var comparer = GetSubject();

            comparer.Compare(expected, actual);
            _mockReporter.Received(1).ReportError(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<object>());
        }

        [Fact]
        public void Compare_WithMatchingObjectBody_NoExceptionsAreThrown()
        {
            var expected = new ProviderServiceResponse
            {
                Status = 201,
                Body = new
                {
                    myString = "Tester",
                    myInt = 1,
                    myGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C")
                }
            };

            var actual = new ProviderServiceResponse
            {
                Status = 201,
                Body = new
                {
                    myString = "Tester",
                    myInt = 1,
                    myGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C")
                }
            };

            var comparer = GetSubject();

            comparer.Compare(expected, actual);
        }

        [Fact]
        public void Compare_WithMatchingObjectBodyOutOfOrder_NoExceptionsAreThrown()
        {
            var expected = new ProviderServiceResponse
            {
                Status = 201,
                Body = new
                {
                    myString = "Tester",
                    myInt = 1,
                    myGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C")
                }
            };

            var actual = new ProviderServiceResponse
            {
                Status = 201,
                Body = new
                {
                    myGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C"),
                    myInt = 1,
                    myString = "Tester"
                }
            };

            var comparer = GetSubject();

            comparer.Compare(expected, actual);
        }

        [Fact]
        public void Compare_WithMatchingObjectBodyButResponseHasAdditionalProperties_NoExceptionsAreThrown()
        {
            var expected = new ProviderServiceResponse
            {
                Status = 201,
                Body = new
                {
                    myString = "Tester",
                    myInt = 1,
                    myGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C")
                }
            };

            var actual = new ProviderServiceResponse
            {
                Status = 201,
                Body = new
                {
                    myString = "Tester",
                    myInt = 1,
                    myGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C"),
                    additional = "Hello"
                }
            };

            var comparer = GetSubject();

            comparer.Compare(expected, actual);
        }

        [Fact]
        public void Compare_WithNonMatchingObject_ReportErrorIsCalled()
        {
            var expected = new ProviderServiceResponse
            {
                Status = 201,
                Body = new
                {
                    myString = "Tester",
                    myInt = 1,
                    myGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C"),
                    myDouble = 2.0
                }
            };

            var actual = new ProviderServiceResponse
            {
                Status = 201,
                Body = new
                {
                    myString = "Tester",
                    MyInt = 1,
                    MyGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C")
                }
            };

            var comparer = GetSubject();

            comparer.Compare(expected, actual);
            _mockReporter.Received(1).ReportError(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<object>());
        }

        [Fact]
        public void Compare_WithMatchingObjectAndANonMatchingValue_ReportErrorIsCalled()
        {
            var expected = new ProviderServiceResponse
            {
                Status = 201,
                Body = new
                {
                    myString = "Tester",
                    myInt = 1,
                    myGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C")
                }
            };

            var actual = new ProviderServiceResponse
            {
                Status = 201,
                Body = new
                {
                    myString = "Tester2",
                    myInt = 1,
                    myGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C")
                }
            };

            var comparer = GetSubject();

            comparer.Compare(expected, actual);
            _mockReporter.Received(1).ReportError(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<object>());
        }

        [Fact]
        public void Compare_WithMatchingObjectHoweverPropertyNameCasingIsDifferent_ReportErrorIsCalled()
        {
            var expected = new ProviderServiceResponse
            {
                Status = 201,
                Body = new
                {
                    MyString = "Tester",
                    MyInt = 1,
                    MyGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C")
                }
            };

            var actual = new ProviderServiceResponse
            {
                Status = 201,
                Body = new
                {
                    myString = "Tester",
                    myInt = 1,
                    myGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C")
                }
            };

            var comparer = GetSubject();

            comparer.Compare(expected, actual);
            _mockReporter.Received(1).ReportError(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<object>());
        }

        [Fact]
        public void Compare_WithNullBodyInResponse_ReportErrorIsCalled()
        {
            var expected = new ProviderServiceResponse
            {
                Status = 201,
                Body = new
                {
                    myString = "Tester",
                    myInt = 1,
                    myGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C")
                }
            };

            var actual = new ProviderServiceResponse
            {
                Status = 201
            };

            var comparer = GetSubject();

            comparer.Compare(expected, actual);
            _mockReporter.Received(1).ReportError(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<object>());
        }

        [Fact]
        public void Compare_WithMatchingCollection_NoExceptionsAreThrown()
        {
            var expected = new ProviderServiceResponse
            {
                Status = 201,
                Body = new List<dynamic>
                {
                    new 
                    {
                        myString = "Tester",
                        myInt = 1,
                        myGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C")
                    }
                }
            };

            var actual = new ProviderServiceResponse
            {
                Status = 201,
                Body = new List<dynamic>
                {
                    new 
                    {
                        myString = "Tester",
                        myInt = 1,
                        myGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C")
                    }
                }
            };

            var comparer = GetSubject();

            comparer.Compare(expected, actual);
        }

        [Fact]
        public void Compare_WithNonMatchingCollection_ReportErrorIsCalled()
        {
            var expected = new ProviderServiceResponse
            {
                Status = 201,
                Body = new List<dynamic>
                {
                    new 
                    {
                        myString = "Tester",
                        myInt = 1,
                        myGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C")
                    }
                }
            };

            var actual = new ProviderServiceResponse
            {
                Status = 201,
                Body = new List<dynamic>
                {
                    new 
                    {
                        myString = "Tester2",
                        myInt = 1,
                        myGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C")
                    }
                }
            };

            var comparer = GetSubject();

            comparer.Compare(expected, actual);
            _mockReporter.Received(1).ReportError(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<object>());
        }
    }
}
