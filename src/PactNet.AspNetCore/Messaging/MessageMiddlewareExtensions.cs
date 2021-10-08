using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PactNet.Verifier.Messaging;

namespace PactNet.AspNetCore.Messaging
{
    /// <summary>
    /// Defines the message middleware extensions
    /// </summary>
    public static class MessageMiddlewareExtensions
    {
        /// <summary>
        /// Register services for Pact message verifier middleware
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="configure">Configure options</param>
        /// <returns>Fluent builder</returns>
        public static IServiceCollection AddPactMessaging(this IServiceCollection services, Action<MessagingVerifierOptions> configure)
        {
            services.Configure(configure);

            return services;
        }

        /// <summary>
        /// Extensions method to add the message middleware
        /// </summary>
        /// <param name="builder">the builder</param>
        /// <returns>Fluent builder</returns>
        public static IApplicationBuilder UsePactMessaging(this IApplicationBuilder builder)
        {
            IMessagingScenarioAccessor messagingScenarioAccessor = new MessagingScenarioAccessor();
            return builder.UseMiddleware<MessageMiddleware>(messagingScenarioAccessor);
        }
    }
}
