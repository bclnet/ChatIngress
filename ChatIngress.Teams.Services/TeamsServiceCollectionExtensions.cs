using Azure.Core;
using ChatIngress.Teams;
using ChatIngress.Teams.Services;
using Microsoft.Extensions.Logging;
using Microsoft.TeamsFx;
using System;
using System.Security;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// TeamsServiceCollectionExtensions
    /// </summary>
    public static class TeamsServiceCollectionExtensions
    {
        class ParsedTeamsOptions : ITeamsOptions
        {
            readonly ParsedConnectionString _parsedConnectionString;
            public ParsedTeamsOptions(string connectionString) => _parsedConnectionString = new ParsedConnectionString(connectionString);
            public TokenCredential Token => default; // _parsedConnectionString.Params.TryGetValue("token", out var z) ? z : _parsedConnectionString.Credential?.Password;
        }

        /// <summary>
        /// Adds the Teams backend.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="config">The configuration.</param>
        /// <param name="name">The name.</param>
        /// <returns>IServiceCollection.</returns>
        /// <exception cref="ArgumentNullException">services</exception>
        public static IServiceCollection AddSlackContext(this IServiceCollection services, ITeamsConnectionString config, string name = null)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            services.AddTransient<ITeamsService, TeamsService>();
            services.Add(ServiceDescriptor.Singleton<ITeamsContext>(c => new TeamsContext(c.GetService<ILogger<TeamsFx.TeamsFx>>(), c.GetService<ILogger<MsGraphAuthProvider>>(), new ParsedTeamsOptions(config[name]))));
            return services;
        }
        /// <summary>
        /// Adds the Teams backend.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="options">The configuration.</param>
        /// <returns>IServiceCollection.</returns>
        /// <exception cref="ArgumentNullException">services</exception>
        public static IServiceCollection AddTeamsContext(this IServiceCollection services, ITeamsOptions options)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            services.AddTransient<ITeamsService, TeamsService>();
            services.Add(ServiceDescriptor.Singleton<ITeamsContext>(c => new TeamsContext(c.GetService<ILogger<TeamsFx.TeamsFx>>(), c.GetService<ILogger<MsGraphAuthProvider>>(), options)));
            return services;
        }
    }
}
