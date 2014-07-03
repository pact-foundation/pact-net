using System;
using System.Collections.Generic;
using System.Linq;
using Dynamitey;
using Microsoft.CSharp.RuntimeBinder;

namespace PactNet
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
                var expectedResponseBodyAsEnumerable = expectedResponse.Body as IEnumerable<dynamic>;
                var expectedResponseBodyAsInt = expectedResponse.Body as int?;
                var expectedResponseBodyAsDouble = expectedResponse.Body as double?;
                var expectedResponseBodyAsString = expectedResponse.Body as string;

                if (expectedResponseBodyAsEnumerable != null)
                {
                    //Support for collection response body
                    Console.WriteLine("{0} has a matching body", MessagePrefix);

                    var actualItemsEnumerable = actualResponse.Body as IEnumerable<dynamic>;

                    if (actualItemsEnumerable == null)
                    {
                        throw new PactAssertException("Body is null in response");
                    }

                    var expectedItemsArr = expectedResponseBodyAsEnumerable.ToArray();
                    var actualItemsArr = actualItemsEnumerable.ToArray();

                    for (var i = 0; i < expectedItemsArr.Length; i++)
                    {
                        var expectedItem = expectedItemsArr[i];
                        var actualItem = actualItemsArr[i];

                        AssertPropertyValuesMatch(expectedItem, actualItem);
                    }
                }
                else if (expectedResponseBodyAsInt != null)
                {
                    var actualResponseBodyAsInt = (int)actualResponse.Body;
                    if (!expectedResponseBodyAsInt.Value.Equals(actualResponseBodyAsInt))
                    {
                        throw new PactAssertException(expectedResponseBodyAsInt, actualResponseBodyAsInt);
                    }
                }
                else if (expectedResponseBodyAsDouble != null)
                {
                    var actualResponseBodyAsDouble = (double)actualResponse.Body;
                    if (!expectedResponseBodyAsDouble.Value.Equals(actualResponseBodyAsDouble))
                    {
                        throw new PactAssertException(expectedResponseBodyAsDouble, actualResponseBodyAsDouble);
                    }
                }
                else if (expectedResponseBodyAsString != null)
                {
                    var actualResponseBodyAsString = (string)actualResponse.Body;
                    if (!expectedResponseBodyAsString.Equals(actualResponseBodyAsString))
                    {
                        throw new PactAssertException(expectedResponseBodyAsString, actualResponseBodyAsString);
                    }
                }
                else
                {
                    //Support for object response body
                    AssertPropertyValuesMatch(expectedResponse.Body, actualResponse.Body);
                }
            }
        }

        private static void AssertPropertyValuesMatch(dynamic expected, dynamic actual)
        {
            var customPropertiesOnObject = Dynamic.GetMemberNames(expected, true);

            foreach (var propertyName in customPropertiesOnObject)
            {
                var expectedValue = Dynamic.InvokeGet(expected, propertyName);

                dynamic actualValue;
                try
                {
                    if (actual == null)
                    {
                        throw new PactAssertException("Body is null in response");
                    }
                    actualValue = Dynamic.InvokeGet(actual, propertyName);
                }
                catch (RuntimeBinderException)
                {
                    //Body is null in response
                    throw new PactAssertException(String.Format("Body.{0} does not exist in response", propertyName));
                }

                if (!expectedValue.Equals(actualValue))
                {
                    throw new PactAssertException(String.Format("Body.{0}", propertyName), expectedValue, actualValue);
                }
            }
        }
    }
}