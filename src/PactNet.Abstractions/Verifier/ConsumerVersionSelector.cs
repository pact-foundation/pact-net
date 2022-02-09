namespace PactNet.Verifier
{
    /// <summary>
    /// Consumer version selector
    /// </summary>
    /// <remarks>See <see href="https://docs.pact.io/pact_broker/advanced_topics/consumer_version_selectors"/></remarks>
    public class ConsumerVersionSelector
    {
        /// <summary>
        /// Select pacts from the main branch of the consumer
        /// </summary>
        public bool MainBranch { get; set; }

        /// <summary>
        /// Select pacts which have the same branch as the current provider branch
        /// </summary>
        public bool MatchingBranch { get; set; }

        /// <summary>
        /// Select pacts from the given branch
        /// </summary>
        public string Branch { get; set; }

        /// <summary>
        /// If the consumer doesn't have a branch which matches the <see cref="Branch"/> property, use this branch instead
        /// </summary>
        public string FallbackBranch { get; set; }

        /// <summary>
        /// Select pacts with the given tag
        /// </summary>
        /// <remarks>It is recommended that the <see cref="Branch"/> property is used instead</remarks>
        public string Tag { get; set; }

        /// <summary>
        /// If the consumer doesn't have a tag which matches the <see cref="Tag"/> property, use this tag instead
        /// </summary>
        /// <remarks>It is recommended that the <see cref="FallbackBranch"/> property is used instead</remarks>
        public string FallbackTag { get; set; }

        /// <summary>
        /// Only return consumer pacts which are deployed to an environment
        /// </summary>
        public bool Deployed { get; set; }

        /// <summary>
        /// Only return consumer pacts which are released and currently supported in any environment
        /// </summary>
        public bool Released { get; set; }

        /// <summary>
        /// Either <see cref="Deployed"/> or <see cref="Released"/> (whereas setting both of the other properties
        /// would mean deployed AND released)
        /// </summary>
        public bool DeployedOrReleased { get; set; }

        /// <summary>
        /// Only return consumer pacts which match this environment, in conjunction with <see cref="Deployed"/> or <see cref="Released"/>
        /// </summary>
        public string Environment { get; set; }

        /// <summary>
        /// Only return the latest pact for a given branch or tag
        /// </summary>
        /// <remarks>This should not be set when used with a branch</remarks>
        public bool? Latest { get; set; }

        /// <summary>
        /// Only return pacts for the given consumer instead of all consumers
        /// </summary>
        public string Consumer { get; set; }
    }
}
