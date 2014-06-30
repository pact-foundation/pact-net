using System;
using System.Collections.Generic;
using System.Linq;
using Dynamitey;

namespace Concord
{
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

            //TODO: Ensure headers are all good

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

                if (!leftValue.Equals(rightValue))
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