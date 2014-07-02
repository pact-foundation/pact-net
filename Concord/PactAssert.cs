using System;
using System.Collections.Generic;
using System.Linq;
using Dynamitey;

namespace Concord
{
    public static class PactAssert
    {
        public static void Equal(PactProviderResponse expectedResponse, PactProviderResponse actualResponse)
        {
            if (actualResponse == null)
                throw new Exception("Response does not match");

            if (!expectedResponse.Status.Equals(actualResponse.Status))
                throw new Exception("Response does not match");

            if (expectedResponse.Headers != null && expectedResponse.Headers.Any())
            {
                foreach (var header in expectedResponse.Headers)
                {
                    string headerValue;

                    if (actualResponse.Headers == null || !actualResponse.Headers.Any())
                    {
                        throw new Exception("Response does not match");
                    }

                    if (actualResponse.Headers.TryGetValue(header.Key, out headerValue))
                    {
                        if (!header.Value.Equals(headerValue, StringComparison.InvariantCultureIgnoreCase))
                        {
                            throw new Exception("Response does not match");
                        }
                    }
                    else
                    {
                        throw new Exception("Response does not match");
                    }
                }
            }

            //TODO: Refactor the names here, as they are kind of rubbish atm
            //TODO: Does not support nested objects, without equality operators
            var leftItemsEnumerable = expectedResponse.Body as IEnumerable<dynamic>;
            var rightItemsEnumerable = actualResponse.Body as IEnumerable<dynamic>;

            if (leftItemsEnumerable != null && rightItemsEnumerable != null)
            {
                var leftItemsArr = leftItemsEnumerable.ToArray();
                var rightItemsArr = rightItemsEnumerable.ToArray();

                for (var i = 0; i < leftItemsArr.Length; i++)
                {
                    var leftItem = leftItemsArr[i];
                    var rightItem = rightItemsArr[i];

                    if (!DoOrderedPropertyValuesMatch(leftItem, rightItem))
                    {
                        throw new Exception("Response does not match");
                    }
                }
            }
            else
            {
                if (!DoOrderedPropertyValuesMatch(expectedResponse.Body, actualResponse.Body))
                {
                    throw new Exception("Response does not match");
                }
            }
        }

        private static bool DoOrderedPropertyValuesMatch(dynamic leftObject, dynamic rightObject)
        {
            var customPropertiesOnObject = Dynamic.GetMemberNames(leftObject, true);

            foreach (var propertyName in customPropertiesOnObject)
            {
                var leftValue = Dynamic.InvokeGet(leftObject, propertyName);
                var rightValue = Dynamic.InvokeGet(rightObject, propertyName);

                if (!leftValue.Equals(rightValue)) //TODO: This will fail if there is a casing mismatch
                {
                    return false;
                }
            }

            return true;
        }
    }
}