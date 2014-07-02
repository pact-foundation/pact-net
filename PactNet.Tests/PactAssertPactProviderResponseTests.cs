using System;
using System.Collections.Generic;
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

            PactAssert.Equal(expected, actual);
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

            Assert.Throws<PactAssertException>(() => PactAssert.Equal(expected, actual));
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

            PactAssert.Equal(expected, actual);
        }

        [Fact]
        public void Equal_WithMatchingHeadersButWithDifferentCasingOnName_NoExceptionsAreThrown()
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

            PactAssert.Equal(expected, actual);
        }

        [Fact]
        public void Equal_WithMatchingHeadersButWithDifferentCasingOnValue_NoExceptionsAreThrown()
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

            PactAssert.Equal(expected, actual);
        }

        [Fact]
        public void Equal_WithMatchingHeadersButResponseHasAdditionalHeaders_NoExceptionsAreThrown()
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
                    { "X-Test", "MyCustomThing" },
                    { "X-Test-2", "MyCustomThing2" },
                    { "Content-Type", "application/json" }
                }
            };

            PactAssert.Equal(expected, actual);
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

            Assert.Throws<PactAssertException>(() => PactAssert.Equal(expected, actual));
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

            Assert.Throws<PactAssertException>(() => PactAssert.Equal(expected, actual));
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

            Assert.Throws<PactAssertException>(() => PactAssert.Equal(expected, actual));
        }

        [Fact]
        public void Equal_WithMatchingObjectBody_NoExceptionsAreThrown()
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
                    MyString = "Tester",
                    MyInt = 1,
                    MyGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C")
                }
            };

            PactAssert.Equal(expected, actual);
        }

        [Fact]
        public void Equal_WithMatchingObjectBodyOutOfOrder_NoExceptionsAreThrown()
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
                    MyGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C"),
                    MyInt = 1,
                    MyString = "Tester"
                }
            };

            PactAssert.Equal(expected, actual);
        }

        [Fact]
        public void Equal_WithMatchingObjectBodyButResponseHasAdditionalProperties_NoExceptionsAreThrown()
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
                    MyString = "Tester",
                    MyInt = 1,
                    MyGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C"),
                    Additional = "Hello"
                }
            };

            PactAssert.Equal(expected, actual);
        }

        [Fact]
        public void Equal_WithMatchingObjectNoMatchingValues_ThrowsPactAssertException()
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
                    MyString = "Tester2",
                    MyInt = 1,
                    MyGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C")
                }
            };

            Assert.Throws<PactAssertException>(() => PactAssert.Equal(expected, actual));
        }
    }
}
