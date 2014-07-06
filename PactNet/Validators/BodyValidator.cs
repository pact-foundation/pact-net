using System;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace PactNet.Validators
{
    public class BodyValidator : IBodyValidator
    {
        private readonly string _messagePrefix;

        public BodyValidator(string messagePrefix)
        {
            _messagePrefix = messagePrefix;
        }

        public void Validate(JToken left, JToken right)
        {
            if (left.Type == JTokenType.Array)
            {
                Console.WriteLine("{0} has a matching body", _messagePrefix);

                if (right == null || !right.Any())
                {
                    throw new PactAssertException("Body is null or empty");
                }

                for (var i = 0; i < left.Count(); i++)
                {
                    var leftItem = left[i];
                    var rightItem = right[i];

                    AssertPropertyValuesMatch(leftItem, rightItem);
                }
            }
            else if (left.Type == JTokenType.Object)
            {
                AssertPropertyValuesMatch(left, right);
            }
            else if (left.Type == JTokenType.Integer)
            {
                if (!left.Equals(right))
                {
                    throw new PactAssertException(left, right);
                }
            }
            else if (left.Type == JTokenType.String)
            {
                if (!left.Equals(right))
                {
                    throw new PactAssertException(left, right);
                }
            }
            else
            {
                throw new NotSupportedException("Type has not been implemented for comparison");
            }
        }

        private static void AssertPropertyValuesMatch(JToken left, JToken right)
        {
            if (right == null || !right.Any())
            {
                throw new PactAssertException("Body is null");
            }

            foreach (var leftItem in left)
            {
                var rightItem = right.FirstOrDefault(x => x.Path == leftItem.Path);

                if (rightItem == null)
                {
                    throw new PactAssertException(String.Format("Body.{0} does not exist", leftItem.Path));
                }

                //TODO: Work on these comparisons
                if(!JToken.DeepEquals(leftItem, rightItem))
                {
                    throw new PactAssertException(String.Format("Body.{0}", leftItem.Path), leftItem, rightItem);
                }
            }
        }
    }
}