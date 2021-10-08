using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PactNet.Verifier.ProviderState;

namespace PactNet.AspNetCore.ProviderState
{
    /// <summary>
    /// Defines the provider state middleware extensions
    /// </summary>
    public static class ProviderStateMiddlewareExtensions
    {
        /// <summary>
        /// Register services for Pact provider state middleware
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="configure">Configure options</param>
        /// <returns>Fluent builder</returns>
        public static IServiceCollection AddPactProviderState(this IServiceCollection services, Action<ProviderStateOptions> configure)
        {
            services.Configure(configure);

            return services;
        }

        /// <summary>
        /// Extensions method to add the provider state middleware
        /// </summary>
        /// <param name="builder">the builder</param>
        /// <returns>Fluent builder</returns>
        public static IApplicationBuilder UsePactProviderStates(this IApplicationBuilder builder)
        {
            IProviderStateAccessor providerStateAccessor = new ProviderStateAccessor();
            return builder.UseMiddleware<ProviderStateMiddleware>(providerStateAccessor);
        }
    }
}
