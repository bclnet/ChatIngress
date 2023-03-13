using System;
using System.Threading;
using System.Threading.Tasks;
using SlackNet;
using SlackNet.WebApi;
using Args = System.Collections.Generic.Dictionary<string, object>;

namespace ChatIngress.Slack.SlackNet.WebApi
{
    /// <summary>
    /// IUndocumentedApi
    /// </summary>
    public interface IUndocumentedApi
    {
        /// <summary>
        /// Deletes a pending scheduled message from the queue.
        /// </summary>
        /// <param name="channelId">The channel the scheduled_message is posting to.</param>
        /// <param name="scheduledMessageId">ScheduledMessageId returned from call to ScheduleMessage.</param>
        /// <param name="cancellationToken"></param>
        /// <seealso cref="https://api.slack.com/methods/chat.deleteScheduledMessage" />
        Task DeleteScheduledMessage(string channelId, string scheduledMessageId, bool? asUser = null, CancellationToken? cancellationToken = null);

        /// <summary>
        /// Conversations: Tests if a public or private channel-based conversation exists.
        /// </summary>
        /// <param name="name">Name of the public or private channel to check.</param>
        /// <param name="asUser">Pass True to create/delete the message as the authed user. Bot users in this context are considered authed users.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<bool> ConversationsExists(string name, bool? asUser = null, CancellationToken? cancellationToken = null);
    }

    /// <summary>
    /// UndocumentedApi
    /// </summary>
    public class UndocumentedApi : IUndocumentedApi
    {
        readonly ISlackApiClient _client;
        public UndocumentedApi(ISlackApiClient client) => _client = client;

        /// <summary>
        /// Deletes a pending scheduled message from the queue.
        /// </summary>
        /// <param name="channelId">The channel the scheduled_message is posting to.</param>
        /// <param name="scheduledMessageId">ScheduledMessageId returned from call to ScheduleMessage.</param>
        /// <param name="cancellationToken"></param>
        /// <seealso cref="https://api.slack.com/methods/chat.deleteScheduledMessage" />
        public Task DeleteScheduledMessage(string channelId, string scheduledMessageId, bool? asUser = null, CancellationToken? cancellationToken = null) =>
            _client.Post("chat.deleteScheduledMessage", new Args
                {
                    { "channel", channelId },
                    { "scheduled_message_id", scheduledMessageId },
                    { "as_user", asUser }
                }, cancellationToken);

        /// <summary>
        /// Conversations: Tests if a public or private channel-based conversation exists.
        /// </summary>
        /// <param name="name">Name of the public or private channel to check.</param>
        /// <param name="asUser">Pass True to create/delete the message as the authed user. Bot users in this context are considered authed users.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<bool> ConversationsExists(string name, bool? asUser = null, CancellationToken? cancellationToken = null)
        {
            try
            {
                var message = await _client.Post<ScheduleMessageResponse>("chat.scheduleMessage", new Args
                    {
                        { "channel", name },
                        { "text", " " },
                        { "as_user", asUser },
                        { "post_at", DateTime.Now.AddMinutes(1).ToTimestamp() }
                    },
                    cancellationToken).ConfigureAwait(false);
                await _client.Get("chat.deleteScheduledMessage", new Args
                    {
                        { "channel", message.Channel },
                        { "scheduled_message_id", message.ScheduledMessageId },
                        { "as_user", asUser }
                    },
                    cancellationToken).ConfigureAwait(false);
                return true;
            }
            catch (SlackException ex)
            {
                return ex.ErrorCode != "channel_not_found";
            }
        }
    }
}
