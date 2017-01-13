using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NSubstitute.Exceptions;
using Newtonsoft.Json;
using PactNet.Tests.IntegrationTests.Specification.Models;
using Xunit;

namespace PactNet.Tests.IntegrationTests.Specification
{
    public class MockHttpServiceSpecificationTests
    {
        [Fact]
        public void ValidateRequestSpecification()
        {
            var failedTestCases = RunPactSpecificationTests<RequestTestCase>("..\\..\\IntegrationTests\\Specification\\pact-specification\\testcases\\request");

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
        public void ValidateResponseSpecification()
        {
            var failedTestCases = RunPactSpecificationTests<ResponseTestCase>("..\\..\\IntegrationTests\\Specification\\pact-specification\\testcases\\response");

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

            if (!Directory.Exists(pathToTestCases))
            {
                throw new InvalidOperationException(
                    $"Specification tests not found in path '{pathToTestCases}'. Please ensure pact-specification git submodule has been pulled (git submodule update --init).");
            }

            foreach (var testCaseSubDirectory in Directory.EnumerateDirectories(pathToTestCases))
            {
                var testCaseFileNames = Directory.GetFiles(testCaseSubDirectory);
                foreach (var testCaseFileName in testCaseFileNames)
                {
                    var testCaseJson = File.ReadAllText(testCaseFileName);
                    var testCase = (T)JsonConvert.DeserializeObject(testCaseJson, typeof(T));

                    try
                    {
                        Console.WriteLine("Running test: " + testCaseFileName);
                        testCase.Verify();
                    }
                    catch (SubstituteException)
                    {
                        failedTestCases.Add($"[Failed] {testCaseFileName}");
                    }
                }
            }

           return failedTestCases;
        }
    }
}