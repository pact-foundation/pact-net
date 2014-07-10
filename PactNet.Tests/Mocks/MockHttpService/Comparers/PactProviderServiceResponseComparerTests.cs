using System;
using System.Collections.Generic;
using PactNet.Mocks.MockHttpService.Comparers;
using PactNet.Mocks.MockHttpService.Models;
using Xunit;

namespace PactNet.Tests.Mocks.MockHttpService.Comparers
{
    public class PactProviderServiceResponseComparerTests
    {
        private IPactProviderServiceResponseComparer GetSubject()
        {
            return new PactProviderServiceResponseComparer();
        }

        [Fact]
        public void Compare_WithMatchingStatusCodes_NoExceptionsAreThrown()
        {
            var expected = new PactProviderServiceResponse
            {
                Status = 201
            };

            var actual = new PactProviderServiceResponse
            {
                Status = 201
            };

            var pactProviderServiceResponseComparer = GetSubject();

            pactProviderServiceResponseComparer.Compare(expected, actual);
        }

        [Fact]
        public void Compare_WithNonMatchingStatusCodes_ThrowsPactAssertException()
        {
            var expected = new PactProviderServiceResponse
            {
                Status = 201
            };

            var actual = new PactProviderServiceResponse
            {
                Status = 400
            };

            var pactProviderServiceResponseComparer = GetSubject();

            Assert.Throws<CompareFailedException>(() => pactProviderServiceResponseComparer.Compare(expected, actual));
        }

        [Fact]
        public void Compare_WithMatchingHeaders_NoExceptionsAreThrown()
        {
            var expected = new PactProviderServiceResponse
            {
                Status = 201,
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                }
            };

            var actual = new PactProviderServiceResponse
            {
                Status = 201,
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                }
            };

            var pactProviderServiceResponseComparer = GetSubject();

            pactProviderServiceResponseComparer.Compare(expected, actual);
        }

        [Fact]
        public void Compare_WithMatchingHeadersButWithDifferentCasingOnName_NoExceptionsAreThrown()
        {
            var expected = new PactProviderServiceResponse
            {
                Status = 201,
                Headers = new Dictionary<string, string>
                {
                    { "content-Type", "application/json" }
                }
            };

            var actual = new PactProviderServiceResponse
            {
                Status = 201,
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                }
            };

            var pactProviderServiceResponseComparer = GetSubject();

            pactProviderServiceResponseComparer.Compare(expected, actual);
        }

        [Fact]
        public void Compare_WithMatchingHeadersButWithDifferentCasingOnValue_ThrowsPactAssertException()
        {
            var expected = new PactProviderServiceResponse
            {
                Status = 201,
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "Application/Json" }
                }
            };

            var actual = new PactProviderServiceResponse
            {
                Status = 201,
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                }
            };

            var pactProviderServiceResponseComparer = GetSubject();

            Assert.Throws<CompareFailedException>(() => pactProviderServiceResponseComparer.Compare(expected, actual));
        }

        [Fact]
        public void Compare_WithMatchingHeadersButResponseHasAdditionalHeaders_NoExceptionsAreThrown()
        {
            var expected = new PactProviderServiceResponse
            {
                Status = 201,
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                }
            };

            var actual = new PactProviderServiceResponse
            {
                Status = 201,
                Headers = new Dictionary<string, string>
                {
                    { "X-Test", "MyCustomThing" },
                    { "X-Test-2", "MyCustomThing2" },
                    { "Content-Type", "application/json" }
                }
            };

            var pactProviderServiceResponseComparer = GetSubject();

            pactProviderServiceResponseComparer.Compare(expected, actual);
        }

        [Fact]
        public void Compare_WithNonMatchingHeadersValues_ThrowsPactAssertException()
        {
            var expected = new PactProviderServiceResponse
            {
                Status = 201,
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                }
            };

            var actual = new PactProviderServiceResponse
            {
                Status = 201,
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "text/plain" }
                }
            };

            var pactProviderServiceResponseComparer = GetSubject();

            Assert.Throws<CompareFailedException>(() => pactProviderServiceResponseComparer.Compare(expected, actual));
        }

        [Fact]
        public void Compare_WithNonMatchingHeaderNames_ThrowsPactAssertException()
        {
            var expected = new PactProviderServiceResponse
            {
                Status = 201,
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                }
            };

            var actual = new PactProviderServiceResponse
            {
                Status = 201,
                Headers = new Dictionary<string, string>
                {
                    {"X-Test", "Tester"}
                }
            };

            var pactProviderServiceResponseComparer = GetSubject();

            Assert.Throws<CompareFailedException>(() => pactProviderServiceResponseComparer.Compare(expected, actual));
        }

        [Fact]
        public void Compare_WithResponseThatHasNoHeaders_ThrowsPactAssertException()
        {
            var expected = new PactProviderServiceResponse
            {
                Status = 201,
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                }
            };

            var actual = new PactProviderServiceResponse
            {
                Status = 201
            };

            var pactProviderServiceResponseComparer = GetSubject();

            Assert.Throws<CompareFailedException>(() => pactProviderServiceResponseComparer.Compare(expected, actual));
        }

        [Fact]
        public void Compare_WithMatchingObjectBody_NoExceptionsAreThrown()
        {
            var expected = new PactProviderServiceResponse
            {
                Status = 201,
                Body = new
                {
                    myString = "Tester",
                    myInt = 1,
                    myGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C")
                }
            };

            var actual = new PactProviderServiceResponse
            {
                Status = 201,
                Body = new
                {
                    myString = "Tester",
                    myInt = 1,
                    myGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C")
                }
            };

            var pactProviderServiceResponseComparer = GetSubject();

            pactProviderServiceResponseComparer.Compare(expected, actual);
        }

        [Fact]
        public void Compare_WithMatchingObjectBodyOutOfOrder_NoExceptionsAreThrown()
        {
            var expected = new PactProviderServiceResponse
            {
                Status = 201,
                Body = new
                {
                    myString = "Tester",
                    myInt = 1,
                    myGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C")
                }
            };

            var actual = new PactProviderServiceResponse
            {
                Status = 201,
                Body = new
                {
                    myGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C"),
                    myInt = 1,
                    myString = "Tester"
                }
            };

            var pactProviderServiceResponseComparer = GetSubject();

            pactProviderServiceResponseComparer.Compare(expected, actual);
        }

        [Fact]
        public void Compare_WithMatchingObjectBodyButResponseHasAdditionalProperties_NoExceptionsAreThrown()
        {
            var expected = new PactProviderServiceResponse
            {
                Status = 201,
                Body = new
                {
                    myString = "Tester",
                    myInt = 1,
                    myGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C")
                }
            };

            var actual = new PactProviderServiceResponse
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

            var pactProviderServiceResponseComparer = GetSubject();

            pactProviderServiceResponseComparer.Compare(expected, actual);
        }

        [Fact]
        public void Compare_WithNonMatchingObject_ThrowsPactAssertException()
        {
            var expected = new PactProviderServiceResponse
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

            var actual = new PactProviderServiceResponse
            {
                Status = 201,
                Body = new
                {
                    myString = "Tester",
                    MyInt = 1,
                    MyGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C")
                }
            };

            var pactProviderServiceResponseComparer = GetSubject();

            Assert.Throws<CompareFailedException>(() => pactProviderServiceResponseComparer.Compare(expected, actual));
        }

        [Fact]
        public void Compare_WithMatchingObjectAndANonMatchingValue_ThrowsPactAssertException()
        {
            var expected = new PactProviderServiceResponse
            {
                Status = 201,
                Body = new
                {
                    myString = "Tester",
                    myInt = 1,
                    myGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C")
                }
            };

            var actual = new PactProviderServiceResponse
            {
                Status = 201,
                Body = new
                {
                    myString = "Tester2",
                    myInt = 1,
                    myGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C")
                }
            };

            var pactProviderServiceResponseComparer = GetSubject();

            Assert.Throws<CompareFailedException>(() => pactProviderServiceResponseComparer.Compare(expected, actual));
        }

        [Fact]
        public void Compare_WithMatchingObjectHoweverPropertyNameCasingIsDifferent_ThrowsPactAssertException()
        {
            var expected = new PactProviderServiceResponse
            {
                Status = 201,
                Body = new
                {
                    MyString = "Tester",
                    MyInt = 1,
                    MyGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C")
                }
            };

            var actual = new PactProviderServiceResponse
            {
                Status = 201,
                Body = new
                {
                    myString = "Tester",
                    myInt = 1,
                    myGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C")
                }
            };

            var pactProviderServiceResponseComparer = GetSubject();

            Assert.Throws<CompareFailedException>(() => pactProviderServiceResponseComparer.Compare(expected, actual));
        }

        [Fact]
        public void Compare_WithNullBodyInResponse_ThrowsPactAssertException()
        {
            var expected = new PactProviderServiceResponse
            {
                Status = 201,
                Body = new
                {
                    myString = "Tester",
                    myInt = 1,
                    myGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C")
                }
            };

            var actual = new PactProviderServiceResponse
            {
                Status = 201
            };

            var pactProviderServiceResponseComparer = GetSubject();

            Assert.Throws<CompareFailedException>(() => pactProviderServiceResponseComparer.Compare(expected, actual));
        }

        [Fact]
        public void Compare_WithMatchingCollection_NoExceptionsAreThrown()
        {
            var expected = new PactProviderServiceResponse
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

            var actual = new PactProviderServiceResponse
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

            var pactProviderServiceResponseComparer = GetSubject();

            pactProviderServiceResponseComparer.Compare(expected, actual);
        }

        [Fact]
        public void Compare_WithNonMatchingCollection_ThrowsPactAssertException()
        {
            var expected = new PactProviderServiceResponse
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

            var actual = new PactProviderServiceResponse
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

            var pactProviderServiceResponseComparer = GetSubject();

            Assert.Throws<CompareFailedException>(() => pactProviderServiceResponseComparer.Compare(expected, actual));
        }
    }
}
