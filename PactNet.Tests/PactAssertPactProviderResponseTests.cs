using System;
using System.Collections.Generic;
using System.Dynamic;
using Xunit;

namespace PactNet.Tests
{
    public class PactAssertPactProviderResponseTests
    {
        //NOTE: For testing the .Body property only use ExpandoObject as that implements GetDynamicMemberNames, however dynamic does not!

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
                Body = new ExpandoObject()
            };

            expected.Body.MyString = "Tester";
            expected.Body.MyInt = 1;
            expected.Body.MyGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C");

            var actual = new PactProviderResponse
            {
                Status = 201,
                Body = new ExpandoObject()
            };

            actual.Body.MyString = "Tester";
            actual.Body.MyInt = 1;
            actual.Body.MyGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C");

            PactAssert.Equal(expected, actual);
        }

        [Fact]
        public void Equal_WithMatchingObjectBodyOutOfOrder_NoExceptionsAreThrown()
        {
            var expected = new PactProviderResponse
            {
                Status = 201,
                Body = new ExpandoObject()
            };

            expected.Body.MyString = "Tester";
            expected.Body.MyInt = 1;
            expected.Body.MyGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C");

            var actual = new PactProviderResponse
            {
                Status = 201,
                Body = new ExpandoObject()
            };

            actual.Body.MyGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C");
            actual.Body.MyInt = 1;
            actual.Body.MyString = "Tester";

            PactAssert.Equal(expected, actual);
        }

        [Fact]
        public void Equal_WithMatchingObjectBodyButResponseHasAdditionalProperties_NoExceptionsAreThrown()
        {
            var expected = new PactProviderResponse
            {
                Status = 201,
                Body = new ExpandoObject()
            };

            expected.Body.MyString = "Tester";
            expected.Body.MyInt = 1;
            expected.Body.MyGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C");

            var actual = new PactProviderResponse
            {
                Status = 201,
                Body = new ExpandoObject()
            };

            actual.Body.MyString = "Tester";
            actual.Body.MyInt = 1;
            actual.Body.MyGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C");
            actual.Body.Additional = "Hello";
            
            PactAssert.Equal(expected, actual);
        }

        [Fact]
        public void Equal_WithNonMatchingObject_ThrowsPactAssertException()
        {
            var expected = new PactProviderResponse
            {
                Status = 201,
                Body = new ExpandoObject()
            };

            expected.Body.MyString = "Tester";
            expected.Body.MyInt = 1;
            expected.Body.MyGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C");
            expected.Body.MyDouble = 2.0;

            var actual = new PactProviderResponse
            {
                Status = 201,
                Body = new ExpandoObject()
            };

            actual.Body.MyString = "Tester";
            actual.Body.MyInt = 1;
            actual.Body.MyGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C");

            Assert.Throws<PactAssertException>(() => PactAssert.Equal(expected, actual));
        }

        [Fact]
        public void Equal_WithMatchingObjectAndANonMatchingValue_ThrowsPactAssertException()
        {
            var expected = new PactProviderResponse
            {
                Status = 201,
                Body = new ExpandoObject()
            };

            expected.Body.MyString = "Tester";
            expected.Body.MyInt = 1;
            expected.Body.MyGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C");

            var actual = new PactProviderResponse
            {
                Status = 201,
                Body = new ExpandoObject()
            };

            actual.Body.MyString = "Tester2";
            actual.Body.MyInt = 1;
            actual.Body.MyGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C");

            Assert.Throws<PactAssertException>(() => PactAssert.Equal(expected, actual));
        }

        [Fact]
        public void Equal_WithNullBodyInResponse_ThrowsPactAssertException()
        {
            var expected = new PactProviderResponse
            {
                Status = 201,
                Body = new ExpandoObject()
            };

            expected.Body.MyString = "Tester";
            expected.Body.MyInt = 1;
            expected.Body.MyGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C");

            var actual = new PactProviderResponse
            {
                Status = 201
            };

            Assert.Throws<PactAssertException>(() => PactAssert.Equal(expected, actual));
        }

        [Fact]
        public void Equal_WithMatchingCollection_NoExceptionsAreThrown()
        {
            var expected = new PactProviderResponse
            {
                Status = 201,
                Body = new List<dynamic>()
            };

            dynamic expectedBodyItem = new ExpandoObject();
            expectedBodyItem.MyString = "Tester";
            expectedBodyItem.MyInt = 1;
            expectedBodyItem.MyGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C");
            expected.Body.Add(expectedBodyItem);

            var actual = new PactProviderResponse
            {
                Status = 201,
                Body = new List<dynamic>()
            };

            dynamic actualBodyItem = new ExpandoObject();
            actualBodyItem.MyString = "Tester";
            actualBodyItem.MyInt = 1;
            actualBodyItem.MyGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C");
            actual.Body.Add(actualBodyItem);

            PactAssert.Equal(expected, actual);
        }

        [Fact]
        public void Equal_WithNonMatchingCollection_ThrowsPactAssertException()
        {
            var expected = new PactProviderResponse
            {
                Status = 201,
                Body = new List<dynamic>()
            };

            dynamic expectedBodyItem = new ExpandoObject();
            expectedBodyItem.MyString = "Tester";
            expectedBodyItem.MyInt = 1;
            expectedBodyItem.MyGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C");
            expected.Body.Add(expectedBodyItem);

            var actual = new PactProviderResponse
            {
                Status = 201,
                Body = new List<dynamic>()
            };

            dynamic actualBodyItem = new ExpandoObject();
            actualBodyItem.MyString = "Tester2";
            actualBodyItem.MyInt = 1;
            actualBodyItem.MyGuid = Guid.Parse("EEB517E6-AC8B-414A-A0DB-6147EAD9193C");
            actual.Body.Add(actualBodyItem);

            Assert.Throws<PactAssertException>(() => PactAssert.Equal(expected, actual));
        }
    }
}
