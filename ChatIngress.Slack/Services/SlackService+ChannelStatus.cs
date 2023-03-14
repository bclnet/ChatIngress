using Dapper;
using Microsoft.Extensions.Logging;
using SlackNet;
using SlackNet.WebApi;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace ChatIngress.Slack.Services
{
    public partial class SlackService
    {
        public async Task<string> StatusChannelAsync(ISlackApiClient slack, IDbConnection db, ILogger log, Conversation channel, bool archive, bool bump = true, CancellationToken? cancellationToken = null)
        {
            if (string.IsNullOrEmpty(channel.Id)) throw new ArgumentNullException(nameof(channel.Id));
            else if (string.IsNullOrEmpty(channel.Name)) throw new ArgumentNullException(nameof(channel.Name));
            var id = channel.Id;
            if (archive) await slack.Conversations.Archive(id, cancellationToken);
            else
            {
                await slack.Conversations.Unarchive(id, cancellationToken);

                // add a "bump" so channel is not archived again
                if (bump) await slack.Chat.PostMessage(new Message { Channel = id, Text = "Bump" }, cancellationToken);
            }
            channel.IsArchived = archive;
            await LogUpdateChannelStatus(db, channel);
            await db.ExecuteAsync("Update [ops].[SlackChannel] Set Archived = @archive Where Id = @id", new { id, archive });
            return null;
        }
    }
}