using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using PactNet.Tests.Specification.Models;
using Xunit;

namespace PactNet.Tests.Specification
{
    //TODO: Move and rename these as they are actually MockHttpServiceSpecificationTests
    public class SpecificationTests
    {
        [Fact]
        public void Request()
        {
            var failedTestCases = RunPactSpecificationTests<RequestTestCase>("..\\..\\Specification\\testcases\\request");

            if (failedTestCases.Any())
            {
                Console.WriteLine("### FAILED ###");
                foreach (var failedTestCase in failedTestCases)
                {
                    Console.WriteLine(failedTestCase);
                }
            }

            Assert.Empty(failedTestCases);
        }

        [Fact]
        public void Response()
        {
            return; //TODO: Remove. For now there are no response tests
            var failedTestCases = RunPactSpecificationTests<ResponseTestCase>("..\\..\\Specification\\testcases\\response");

            if (failedTestCases.Any())
            {
                Console.WriteLine("### FAILED ###");
                foreach (var failedTestCase in failedTestCases)
                {
                    Console.WriteLine(failedTestCase);
                }
            }

            Assert.Empty(failedTestCases);
        }

        private IEnumerable<string> RunPactSpecificationTests<T>(string pathToTestCases)
            where T : class, IVerifiable
        {
            var failedTestCases = new List<string>();

            foreach (var testCaseSubDirectory in Directory.EnumerateDirectories(pathToTestCases))
            {
                var testCaseFileNames = Directory.GetFiles(testCaseSubDirectory);
                foreach (var testCaseFileName in testCaseFileNames)
                {
                    Console.WriteLine();

                    var testCaseJson = File.ReadAllText(testCaseFileName);
                    var testCase = (T) JsonConvert.DeserializeObject(testCaseJson, typeof (T));
                    
                    if (!testCase.Verified())
                    {
                        failedTestCases.Add(String.Format("[Failed] {0}", testCaseFileName));
                    }
                }
            }

           return failedTestCases;
        }
    }
}