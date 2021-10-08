using System;

namespace PactNet.Verifier.Messaging
{
    public class MessagingScenarioAccessor : IMessagingScenarioAccessor
    {
        /// <summary>
        /// Get a messaging scenario by description
        /// </summary>
        /// <returns>the messaging scenario object</returns>
        public IScenario GetByDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("Description cannot be null or empty");
            }

            return Scenarios.GetByDescription(description);
        }
    }
}
