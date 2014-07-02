using System;
using System.Collections.Generic;
using System.Linq;
using Dynamitey;

namespace Concord
{
    public static class PactAssert
    {
        private const string MessagePrefix = "\t- Returns a response which";

        public static void Equal(PactProviderResponse expectedResponse, PactProviderResponse actualResponse)
        {
            Console.WriteLine("{0} has status code of {1}", MessagePrefix, expectedResponse.Status);
            if (!expectedResponse.Status.Equals(actualResponse.Status))
            {
                throw new PactAssertException(expectedResponse.Status, actualResponse.Status);
            }

            if (expectedResponse.Headers != null && expectedResponse.Headers.Any())
            {
                foreach (var header in expectedResponse.Headers)
                {
                    Console.WriteLine("{0} includes header {1} with value {2}", MessagePrefix, header.Key, header.Value);

                    if (actualResponse.Headers == null)
                    {
                        throw new PactAssertException("Headers is null in response");
                    }

                    //TODO: This is a hack, come up with a better way or just make sure the pact generates header casing correctly
                    var caseInsensitiveHeaders = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
                    foreach (var actualResponseHeader in actualResponse.Headers)
                    {
                        caseInsensitiveHeaders.Add(actualResponseHeader.Key, actualResponseHeader.Value);
                    }

                    actualResponse.Headers = caseInsensitiveHeaders;

                    string headerValue;

                    if (actualResponse.Headers.TryGetValue(header.Key, out headerValue))
                    {
                        if (!header.Value.Equals(headerValue, StringComparison.InvariantCultureIgnoreCase))
                        {
                            throw new PactAssertException(header.Value, headerValue);
                        }
                    }
                    else
                    {
                        throw new PactAssertException("Header does not exist in response");
                    }
                }
            }

            if (expectedResponse.Body != null)
            {
                var expectedItemsEnumerable = expectedResponse.Body as IEnumerable<dynamic>;

                if (expectedItemsEnumerable != null)
                {
                    //Support for collection response body
                    Console.WriteLine("{0} has a matching body", MessagePrefix);

                    var actualItemsEnumerable = actualResponse.Body as IEnumerable<dynamic>;

                    if (actualItemsEnumerable == null)
                    {
                        throw new PactAssertException("Body is null in response");
                    }

                    var expectedItemsArr = expectedItemsEnumerable.ToArray();
                    var actualItemsArr = actualItemsEnumerable.ToArray();

                    for (var i = 0; i < expectedItemsArr.Length; i++)
                    {
                        var expectedItem = expectedItemsArr[i];
                        var actualItem = actualItemsArr[i];

                        AssertPropertyValuesMatch(expectedItem, actualItem);
                    }
                }
                else
                {
                    //Support for object response body
                    AssertPropertyValuesMatch(expectedResponse.Body, actualResponse.Body);
                }
                //TODO: Add Support for primitate response body
            }
        }

        private static void AssertPropertyValuesMatch(dynamic expected, dynamic actual)
        {
            var customPropertiesOnObject = Dynamic.GetMemberNames(expected, true);

            foreach (var propertyName in customPropertiesOnObject)
            {
                var expectedValue = Dynamic.InvokeGet(expected, propertyName);
                var actualValue = Dynamic.InvokeGet(actual, propertyName);

                if (!expectedValue.Equals(actualValue))
                {
                    throw new PactAssertException(String.Format("Body.{0}", propertyName), expectedValue, actualValue);
                }
            }
        }
    }
}