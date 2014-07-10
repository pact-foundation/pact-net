using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using PactNet.Tests.Specification.Models;
using Xunit;

namespace PactNet.Tests.Specification
{
    public class SpecificationTests
    {
        private const string RequestTestCaseBasePath = "..\\..\\Specification\\testcases\\request";

        [Fact]
        public void Request()
        {
            var failedTestCases = new List<string>();

            foreach (var testCaseSubDirectory in Directory.EnumerateDirectories(RequestTestCaseBasePath))
            {
                var testCaseFileNames = Directory.GetFiles(testCaseSubDirectory);
                foreach (var testCaseFileName in testCaseFileNames)
                {
                    Console.WriteLine();

                    var testCaseJson = File.ReadAllText(testCaseFileName);
                    var testCase = JsonConvert.DeserializeObject<RequestTestCase>(testCaseJson);

                    if (!testCase.Verify())
                    {
                        failedTestCases.Add(String.Format("[Failed] {0}", testCaseFileName));
                    }
                }
            }

            if (failedTestCases.Any())
            {
                Console.WriteLine("### FAILED ###");
                foreach (var failedTestCase in failedTestCases)
                {
                    Console.WriteLine(failedTestCase);
                }
            }

            Assert.Equal(0, failedTestCases.Count);
        }
    }
}