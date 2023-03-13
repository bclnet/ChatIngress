using SlackNet;
using System;

namespace ChatIngress.Slack
{
    /// <summary>
    /// ISlackContext
    /// </summary>
    public interface ISlackContext
    {
        /// <summary>
        /// Connects this instance.
        /// </summary>
        /// <returns></returns>
        ISlackApiClient Connect();
    }

    /// <summary>
    /// SlackContext
    /// </summary>
    public class SlackContext : ISlackContext
    {
        readonly IHttp _http;
        readonly ISlackUrlBuilder _urlBuilder;
        readonly SlackJsonSettings _jsonSettings;
        readonly string _token;

        /// <summary>
        /// Initializes a new instance of the <see cref="SlackContext"/> class.
        /// </summary>
        /// <param name="options">The configuration.</param>
        public SlackContext(IHttp http, ISlackUrlBuilder urlBuilder, SlackJsonSettings jsonSettings, ISlackOptions options) : this(http, urlBuilder, jsonSettings, options.Token) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="SlackContext"/> class.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="credential">The credential.</param>
        /// <exception cref="System.ArgumentNullException">credential</exception>
        /// <exception cref="System.InvalidOperationException">Cannot connect to organization service at {endpoint}</exception>
        SlackContext(IHttp http, ISlackUrlBuilder urlBuilder, SlackJsonSettings jsonSettings, string token)
        {
            if (string.IsNullOrEmpty(token)) throw new ArgumentNullException(nameof(token));
            _http = http;
            _urlBuilder = urlBuilder;
            _jsonSettings = jsonSettings;
            _token = token;
        }

        /// <summary>
        /// Connects this instance.
        /// </summary>
        /// <returns></returns>
        public ISlackApiClient Connect() => new SlackApiClient(_http, _urlBuilder, _jsonSettings, _token);
    }
}
