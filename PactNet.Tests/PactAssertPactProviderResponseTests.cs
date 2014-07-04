using System;
using System.Collections.Generic;
using System.Dynamic;
using Xunit;

namespace PactNet.Tests
{
    public class PactAssertPactProviderResponseTests
    {
        [Fact]
        public void Equal_WithMatchingStatusCodes_NoExceptionsAreThrown()
        {
            var expected = new PactProviderResponse
            {
                Status = 201
            };

            var actual = new PactProviderResponse
            {
                Status = 201
            };

            new PactAssert().Equal(expected, actual);
        }

        [Fact]
        public void Equal_WithNonMatchingStatusCodes_ThrowsPactAssertException()
        {
            var expected = new PactProviderResponse
            {
                Status = 201
            };

            var actual = new PactProviderResponse
            {
                Status = 400
            };

            Assert.Throws<PactAssertException>(() => new PactAssert().Equal(expected, actual));
        }

        [Fact]
        public void Equal_WithMatchingHeaders_NoExceptionsAreThrown()
        {
            var expected = new PactProviderResponse
            {
                Status = 201,
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                }
            };

            var actual = new PactProviderResponse
            {
                Status = 201,
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                }
            };

            new PactAssert().Equal(expected, actual);
        }

        [Fact]
        public void Equal_WithMatchingHeadersButWithDifferentCasingOnName_ThrowsPactAssertException()
        {
            var expected = new PactProviderResponse
            {
                Status = 201,
                Headers = new Dictionary<string, string>
                {
                    { "content-Type", "application/json" }
                }
            };

            var actual = new PactProviderResponse
            {
                Status = 201,
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                }
            };

            Assert.Throws<PactAssertException>(() => new PactAssert().Equal(expected, actual));
        }

        [Fact]
        public void Equal_WithMatchingHeadersButWithDifferentCasingOnValue_ThrowsPactAssertException()
        {
            var expected = new PactProviderResponse
            {
                Status = 201,
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "Application/Json" }
                }
            };

            var actual = new PactProviderResponse
            {
                Status = 201,
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                }
            };

            Assert.Throws<PactAssertException>(() => new PactAssert().Equal(expected, actual));
        }

        [Fact]
        public void Equal_WithMatchingHeadersButResponseHasAdditionalHeaders_NoExceptionsAreThrown()
        {
            var expected = new PactProviderResponse
            {
                Status = 201,
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                }
            };

            var actual = new PactProviderResponse
            {
                Status = 201,
                Headers = new Dictionary<string, string>
                {
                    { "X-Test", "MyCustomThing" },
                    { "X-Test-2", "MyCustomThing2" },
                    { "Content-Type", "application/json" }
                }
            };

            new PactAssert().Equal(expected, actual);
        }

        [Fact]
        public void Equal_WithNonMatchingHeadersValues_ThrowsPactAssertException()
        {
            var expected = new PactProviderResponse
            {
                Status = 201,
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                }
            };

            var actual = new PactProviderResponse
            {
                Status = 201,
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "text/plain" }
                }
            };

            Assert.Throws<PactAssertException>(() => new PactAssert().Equal(expected, actual));
        }

        [Fact]
        public void Equal_WithNonMatchingHeaderNames_ThrowsPactAssertException()
        {
            var expected = new PactProviderResponse
            {
                Status = 201,
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                }
            };

            var actual = new PactProviderResponse
            {
                Status = 201,
                Headers = new Dictionary<string, string>
                {
                    {"X-Test", "Tester"}
                }
            };

            Assert.Throws<PactAssertException>(() => new PactAssert().Equal(expected, actual));
        }

        [Fact]
        public void Equal_WithResponseThatHasNoHeaders_ThrowsPactAssertException()
        {
            var expected = new PactProviderResponse
            {
                Status = 201,
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", "application/json"}
                }
            };

            var actual = new PactProviderResponse
            {
                Status = 201
            };

            Assert.Throws<PactAssertException>(() => new PactAssert().Equal(expected, actual));
        }

        [Fact]
        public void Equal_WithMatchingObjectBody_NoExceptionsAreThrown()
        {
            var expected = new PactProviderResponse
            {
                Status = 201,
                Body = new
                {
                    myString = "Tester",
                    myInt = 1,
                    myGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C")
                }
            };

            var actual = new PactProviderResponse
            {
                Status = 201,
                Body = new
                {
                    myString = "Tester",
                    myInt = 1,
                    myGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C")
                }
            };

            new PactAssert().Equal(expected, actual);
        }

        [Fact]
        public void Equal_WithMatchingObjectBodyOutOfOrder_NoExceptionsAreThrown()
        {
            var expected = new PactProviderResponse
            {
                Status = 201,
                Body = new
                {
                    myString = "Tester",
                    myInt = 1,
                    myGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C")
                }
            };

            var actual = new PactProviderResponse
            {
                Status = 201,
                Body = new
                {
                    myGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C"),
                    myInt = 1,
                    myString = "Tester"
                }
            };

            new PactAssert().Equal(expected, actual);
        }

        [Fact]
        public void Equal_WithMatchingObjectBodyButResponseHasAdditionalProperties_NoExceptionsAreThrown()
        {
            var expected = new PactProviderResponse
            {
                Status = 201,
                Body = new
                {
                    myString = "Tester",
                    myInt = 1,
                    myGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C")
                }
            };

            var actual = new PactProviderResponse
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

            new PactAssert().Equal(expected, actual);
        }

        [Fact]
        public void Equal_WithNonMatchingObject_ThrowsPactAssertException()
        {
            var expected = new PactProviderResponse
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

            var actual = new PactProviderResponse
            {
                Status = 201,
                Body = new
                {
                    myString = "Tester",
                    MyInt = 1,
                    MyGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C")
                }
            };

            Assert.Throws<PactAssertException>(() => new PactAssert().Equal(expected, actual));
        }

        [Fact]
        public void Equal_WithMatchingObjectAndANonMatchingValue_ThrowsPactAssertException()
        {
            var expected = new PactProviderResponse
            {
                Status = 201,
                Body = new
                {
                    myString = "Tester",
                    myInt = 1,
                    myGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C")
                }
            };

            var actual = new PactProviderResponse
            {
                Status = 201,
                Body = new
                {
                    myString = "Tester2",
                    myInt = 1,
                    myGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C")
                }
            };

            Assert.Throws<PactAssertException>(() => new PactAssert().Equal(expected, actual));
        }

        [Fact]
        public void Equal_WithMatchingObjectHoweverPropertyNameCasingIsDifferent_ThrowsPactAssertException()
        {
            var expected = new PactProviderResponse
            {
                Status = 201,
                Body = new
                {
                    MyString = "Tester",
                    MyInt = 1,
                    MyGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C")
                }
            };

            var actual = new PactProviderResponse
            {
                Status = 201,
                Body = new
                {
                    myString = "Tester",
                    myInt = 1,
                    myGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C")
                }
            };

            Assert.Throws<PactAssertException>(() => new PactAssert().Equal(expected, actual));
        }

        [Fact]
        public void Equal_WithNullBodyInResponse_ThrowsPactAssertException()
        {
            var expected = new PactProviderResponse
            {
                Status = 201,
                Body = new
                {
                    myString = "Tester",
                    myInt = 1,
                    myGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C")
                }
            };

            var actual = new PactProviderResponse
            {
                Status = 201
            };

            Assert.Throws<PactAssertException>(() => new PactAssert().Equal(expected, actual));
        }

        [Fact]
        public void Equal_WithMatchingCollection_NoExceptionsAreThrown()
        {
            var expected = new PactProviderResponse
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

            var actual = new PactProviderResponse
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

            new PactAssert().Equal(expected, actual);
        }

        [Fact]
        public void Equal_WithNonMatchingCollection_ThrowsPactAssertException()
        {
            var expected = new PactProviderResponse
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

            var actual = new PactProviderResponse
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

            Assert.Throws<PactAssertException>(() => new PactAssert().Equal(expected, actual));
        }
    }
}
