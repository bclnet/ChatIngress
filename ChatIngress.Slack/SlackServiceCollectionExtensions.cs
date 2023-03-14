using ChatIngress.Slack;
using ChatIngress.Slack.Services;
using SlackNet;
using System;
using System.Security;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// SlackServiceCollectionExtensions
    /// </summary>
    public static class SlackServiceCollectionExtensions
    {
        class ParsedSlackOptions : ISlackOptions
        {
            readonly ParsedConnectionString _parsedConnectionString;
            public ParsedSlackOptions(string connectionString) => _parsedConnectionString = new ParsedConnectionString(connectionString);
            public string Token => _parsedConnectionString.Params.TryGetValue("token", out var z) ? z : _parsedConnectionString.Credential?.Password;
        }

        /// <summary>
        /// Adds the Slack backend.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="config">The configuration.</param>
        /// <param name="name">The name.</param>
        /// <returns>IServiceCollection.</returns>
        /// <exception cref="ArgumentNullException">services</exception>
        public static IServiceCollection AddSlackContext(this IServiceCollection services, ISlackConnectionString config, string name = null)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            services.AddTransient<ISlackService, SlackService>();
            Default.RegisterServices((serviceType, createService) => services.AddTransient(serviceType, c => createService(c.GetService)));
            services.Add(ServiceDescriptor.Singleton<ISlackContext>(c => new SlackContext(c.GetService<IHttp>(), c.GetService<ISlackUrlBuilder>(), c.GetService<SlackJsonSettings>(), new ParsedSlackOptions(config[name]))));
            return services;
        }
        /// <summary>
        /// Adds the Slack backend.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="options">The configuration.</param>
        /// <returns>IServiceCollection.</returns>
        /// <exception cref="ArgumentNullException">services</exception>
        public static IServiceCollection AddSlackContext(this IServiceCollection services, ISlackOptions options)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            services.AddTransient<ISlackService, SlackService>();
            Default.RegisterServices((serviceType, createService) => services.AddTransient(serviceType, c => createService(c.GetService)));
            services.Add(ServiceDescriptor.Singleton<ISlackContext>(c => new SlackContext(c.GetService<IHttp>(), c.GetService<ISlackUrlBuilder>(), c.GetService<SlackJsonSettings>(), options)));
            return services;
        }
    }
}
