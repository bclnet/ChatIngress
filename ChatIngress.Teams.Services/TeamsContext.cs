using Azure.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.TeamsFx;
using System;

namespace ChatIngress.Teams
{
    /// <summary>
    /// ITeamsContext
    /// </summary>
    public interface ITeamsContext
    {
        /// <summary>
        /// Connects this instance.
        /// </summary>
        /// <returns></returns>
        GraphServiceClient Connect();
    }

    /// <summary>
    /// TeamsContext
    /// </summary>
    public class TeamsContext : ITeamsContext
    {
        readonly ILogger<TeamsFx> _logger;
        readonly ILogger<MsGraphAuthProvider> _authLogger;
        readonly TokenCredential _token;

        /// <summary>
        /// Initializes a new instance of the <see cref="SlackContext"/> class.
        /// </summary>
        /// <param name="options">The configuration.</param>
        public TeamsContext(ILogger<TeamsFx> logger, ILogger<MsGraphAuthProvider> authLogger, ITeamsOptions options) : this(logger, authLogger, options.Token) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="TeamsContext"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="authLogger">The auth logger.</param>
        TeamsContext(ILogger<TeamsFx> logger, ILogger<MsGraphAuthProvider> authLogger, TokenCredential token)
        {
            if (token == null) throw new ArgumentNullException(nameof(token));
            _logger = logger;
            _authLogger = authLogger;
            _token = token;
        }

        /// <summary>
        /// Connects this instance.
        /// </summary>
        /// <returns></returns>
        public GraphServiceClient Connect() => new TeamsFx(_logger, _authLogger).CreateMicrosoftGraphClient(_token);
    }
}
