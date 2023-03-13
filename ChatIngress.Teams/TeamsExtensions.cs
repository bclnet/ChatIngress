using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace ChatIngress.Teams
{
    /// <summary>
    /// TeamsExtensions
    /// </summary>
    public static class TeamsExtensions
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
