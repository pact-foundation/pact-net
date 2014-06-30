using System;
using System.Collections.Generic;
using System.Linq;
using Dynamitey;

namespace Concord
{
    //TODO: Implement fine grain failures

    public class PactProviderResponse : IEquatable<PactProviderResponse>
    {
        public int Status { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public dynamic Body { get; set; }

        public bool Equals(PactProviderResponse other)
        {
            if (other == null)
                return false;

            if (!Status.Equals(other.Status))
                return false;

            if (Headers != null && Headers.Any())
            {
                foreach (var header in Headers)
                {
                    string headerValue;

                    if (other.Headers == null || !other.Headers.Any())
                    {
                        return false;
                    }

                    if (other.Headers.TryGetValue(header.Key, out headerValue))
                    {
                        if (!header.Value.Equals(headerValue, StringComparison.InvariantCultureIgnoreCase))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            //TODO: Refactor the names here, as they are kind of rubbish atm
            //TODO: Does not support nested objects, without equality operators
            var leftItemsEnumerable = Body as IEnumerable<dynamic>;
            var rightItemsEnumerable = other.Body as IEnumerable<dynamic>;

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
                        return false;
                    }
                }
            }
            else
            {
                if (!DoOrderedPropertyValuesMatch(Body, other.Body))
                {
                    return false;
                }
            }

            return true;
        }

        private bool DoOrderedPropertyValuesMatch(dynamic leftObject, dynamic rightObject)
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

        public override int GetHashCode()
        {
            //TODO: implement GetHashCode
            return 0;
            /*unchecked
            {
                int hashCode = Status;
                hashCode = (hashCode * 397) ^ (Headers != null ? Headers.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Body != null ? Body.GetHashCode() : 0);
                return hashCode;
            }*/
        }

        public static bool operator ==(PactProviderResponse a, PactProviderResponse b)
        {
            return Equals(a, b);
        }

        public static bool operator !=(PactProviderResponse a, PactProviderResponse b)
        {
            return !Equals(a, b);
        }
    }
}