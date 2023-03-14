using Microsoft.Extensions.Logging;

namespace ChatIngress.Slack
{
    /// <summary>
    /// SlackExtensions
    /// </summary>
    public static partial class SlackExtensions
    {
        /// <summary>
        /// Gets or sets the log.
        /// </summary>
        /// <value>
        /// The log.
        /// </value>
        public static ILogger Log { get; set; }
    }
}
