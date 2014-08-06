using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PactNet.Reporters;

namespace PactNet.Mocks.MockHttpService.Comparers
{
    public class HttpBodyComparer : IHttpBodyComparer
    {
        private readonly string _messagePrefix;
        private readonly IReporter _reporter;
        private int _comparisonPasses;

        public HttpBodyComparer(string messagePrefix, IReporter reporter)
        {
            _messagePrefix = messagePrefix;
            _reporter = reporter;
        }

        //TODO: Remove boolean and add "matching" functionality
        public void Validate(dynamic body1, dynamic body2, bool useStrict = false)
        {
            _comparisonPasses = 0;

            if (body1 == null)
            {
                return;
            }

            if (body1 != null && body2 == null)
            {
                _reporter.ReportError("Body is null");
                return;
            }

            string body1Json = JsonConvert.SerializeObject(body1);
            string body2Json = JsonConvert.SerializeObject(body2);
            var httpBody1 = JsonConvert.DeserializeObject<JToken>(body1Json);
            var httpBody2 = JsonConvert.DeserializeObject<JToken>(body2Json);

            if (useStrict)
            {
                if (!JToken.DeepEquals(httpBody1, httpBody2))
                {
                    _reporter.ReportError(expected: httpBody1, actual: httpBody2);
                }
                return;
            }

            AssertPropertyValuesMatch(httpBody1, httpBody2);
        }

       
        private bool AssertPropertyValuesMatch(JToken httpBody1, JToken httpBody2)
        {
            _comparisonPasses++;
            if (_comparisonPasses > 200)
            {
                _reporter.ReportError("Too many passes required to compare objects.");
                return false;
            }

            switch (httpBody1.Type)
            {
                case JTokenType.Array: 
                    {
                        if (httpBody1.Count() != httpBody2.Count())
                        {
                            _reporter.ReportError(expected: httpBody1.Root, actual: httpBody2.Root);
                            return false;
                        }

                        for (var i = 0; i < httpBody1.Count(); i++)
                        {
                            if (httpBody2.Count() > i)
                            {
                                var isMatch = AssertPropertyValuesMatch(httpBody1[i], httpBody2[i]);
                                if (!isMatch)
                                {
                                    break;
                                }
                            }
                        }
                        break;
                    }
                case JTokenType.Object:
                    {
                        foreach (JProperty item1 in httpBody1)
                        {
                            var item2 = httpBody2.Cast<JProperty>().SingleOrDefault(x => x.Name == item1.Name);

                            if (item2 != null)
                            {
                                var isMatch = AssertPropertyValuesMatch(item1, item2);
                                if (!isMatch)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                _reporter.ReportError(expected: httpBody1.Root, actual: httpBody2.Root);
                                return false;
                            }
                        }
                        break;
                    }
                case JTokenType.Property: 
                    {
                        var httpBody2Item = httpBody2.SingleOrDefault();
                        var httpBody1Item = httpBody1.SingleOrDefault();

                        if (httpBody2Item == null && httpBody1Item == null)
                        {
                            return true;
                        }

                        if (httpBody2Item != null && httpBody1Item != null)
                        {
                            AssertPropertyValuesMatch(httpBody1Item, httpBody2Item);
                        }
                        else
                        {
                            _reporter.ReportError(expected: httpBody1.Root, actual: httpBody2.Root);
                            return false;
                        }
                        break;
                    }
                case JTokenType.Integer:
                case JTokenType.String: 
                    {
                        if (!httpBody1.Equals(httpBody2))
                        {
                            _reporter.ReportError(expected: httpBody1.Root, actual: httpBody2.Root);
                            return false;
                        }
                        break;
                    }
                default:
                    {
                        if (!JToken.DeepEquals(httpBody1, httpBody2))
                        {
                            _reporter.ReportError(expected: httpBody1.Root, actual: httpBody2.Root);
                            return false;
                        }
                        break;
                    }
            }

            return true;
        }
    }
}