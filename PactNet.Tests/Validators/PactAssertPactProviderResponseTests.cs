using System;
using System.Collections.Generic;
using PactNet.Validators;
using Xunit;

namespace PactNet.Tests.Validators
{
    public class PactAssertPactProviderResponseTests
    {
        private IPactProviderResponseValidator GetSubject()
        {
            return new PactProviderResponseValidator();
        }

        [Fact]
        public void Validate_WithMatchingStatusCodes_NoExceptionsAreThrown()
        {
            var expected = new PactProviderResponse
            {
                Status = 201
            };

            var actual = new PactProviderResponse
            {
                Status = 201
            };

            var pactProviderResponseValidator = GetSubject();

            pactProviderResponseValidator.Validate(expected, actual);
        }

        [Fact]
        public void Validate_WithNonMatchingStatusCodes_ThrowsPactAssertException()
        {
            var expected = new PactProviderResponse
            {
                Status = 201
            };

            var actual = new PactProviderResponse
            {
                Status = 400
            };

            var pactProviderResponseValidator = GetSubject();

            Assert.Throws<PactAssertException>(() => pactProviderResponseValidator.Validate(expected, actual));
        }

        [Fact]
        public void Validate_WithMatchingHeaders_NoExceptionsAreThrown()
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

            var pactProviderResponseValidator = GetSubject();

            pactProviderResponseValidator.Validate(expected, actual);
        }

        [Fact]
        public void Validate_WithMatchingHeadersButWithDifferentCasingOnName_ThrowsPactAssertException()
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

            var pactProviderResponseValidator = GetSubject();

            Assert.Throws<PactAssertException>(() => pactProviderResponseValidator.Validate(expected, actual));
        }

        [Fact]
        public void Validate_WithMatchingHeadersButWithDifferentCasingOnValue_ThrowsPactAssertException()
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

            var pactProviderResponseValidator = GetSubject();

            Assert.Throws<PactAssertException>(() => pactProviderResponseValidator.Validate(expected, actual));
        }

        [Fact]
        public void Validate_WithMatchingHeadersButResponseHasAdditionalHeaders_NoExceptionsAreThrown()
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

            var pactProviderResponseValidator = GetSubject();

            pactProviderResponseValidator.Validate(expected, actual);
        }

        [Fact]
        public void Validate_WithNonMatchingHeadersValues_ThrowsPactAssertException()
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

            var pactProviderResponseValidator = GetSubject();

            Assert.Throws<PactAssertException>(() => pactProviderResponseValidator.Validate(expected, actual));
        }

        [Fact]
        public void Validate_WithNonMatchingHeaderNames_ThrowsPactAssertException()
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

            var pactProviderResponseValidator = GetSubject();

            Assert.Throws<PactAssertException>(() => pactProviderResponseValidator.Validate(expected, actual));
        }

        [Fact]
        public void Validate_WithResponseThatHasNoHeaders_ThrowsPactAssertException()
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

            var pactProviderResponseValidator = GetSubject();

            Assert.Throws<PactAssertException>(() => pactProviderResponseValidator.Validate(expected, actual));
        }

        [Fact]
        public void Validate_WithMatchingObjectBody_NoExceptionsAreThrown()
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

            var pactProviderResponseValidator = GetSubject();

            pactProviderResponseValidator.Validate(expected, actual);
        }

        [Fact]
        public void Validate_WithMatchingObjectBodyOutOfOrder_NoExceptionsAreThrown()
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

            var pactProviderResponseValidator = GetSubject();

            pactProviderResponseValidator.Validate(expected, actual);
        }

        [Fact]
        public void Validate_WithMatchingObjectBodyButResponseHasAdditionalProperties_NoExceptionsAreThrown()
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

            var pactProviderResponseValidator = GetSubject();

            pactProviderResponseValidator.Validate(expected, actual);
        }

        [Fact]
        public void Validate_WithNonMatchingObject_ThrowsPactAssertException()
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

            var pactProviderResponseValidator = GetSubject();

            Assert.Throws<PactAssertException>(() => pactProviderResponseValidator.Validate(expected, actual));
        }

        [Fact]
        public void Validate_WithMatchingObjectAndANonMatchingValue_ThrowsPactAssertException()
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

            var pactProviderResponseValidator = GetSubject();

            Assert.Throws<PactAssertException>(() => pactProviderResponseValidator.Validate(expected, actual));
        }

        [Fact]
        public void Validate_WithMatchingObjectHoweverPropertyNameCasingIsDifferent_ThrowsPactAssertException()
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

            var pactProviderResponseValidator = GetSubject();

            Assert.Throws<PactAssertException>(() => pactProviderResponseValidator.Validate(expected, actual));
        }

        [Fact]
        public void Validate_WithNullBodyInResponse_ThrowsPactAssertException()
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

            var pactProviderResponseValidator = GetSubject();

            Assert.Throws<PactAssertException>(() => pactProviderResponseValidator.Validate(expected, actual));
        }

        [Fact]
        public void Validate_WithMatchingCollection_NoExceptionsAreThrown()
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

            var pactProviderResponseValidator = GetSubject();

            pactProviderResponseValidator.Validate(expected, actual);
        }

        [Fact]
        public void Validate_WithNonMatchingCollection_ThrowsPactAssertException()
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

            var pactProviderResponseValidator = GetSubject();

            Assert.Throws<PactAssertException>(() => pactProviderResponseValidator.Validate(expected, actual));
        }
    }
}
