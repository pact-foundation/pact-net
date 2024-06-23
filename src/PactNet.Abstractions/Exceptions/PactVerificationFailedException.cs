using System;
using PactNet.Models.VerifierJson;

namespace PactNet.Exceptions;

[Serializable]
public class PactVerificationFailedException(VerificationExecutionResult executionResult)
    : PactFailureException("Pact verification failed")
{
    public VerificationExecutionResult ExecutionResult { get; } = executionResult;
}
