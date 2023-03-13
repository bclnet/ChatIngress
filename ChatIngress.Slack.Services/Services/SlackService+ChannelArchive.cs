using Dapper;
using Microsoft.Extensions.Logging;
using SlackNet;
using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChatIngress.Slack.Services
{
    public partial class SlackService
    {
        class ChannelResult_
        {
            public bool Changed;
            public string Id { get; set; }
            public DateTime CreatedOn { get; set; }
            int _flag; public int Flag { get => _flag; set { if (_flag != value) Changed = true; _flag = value; } }
            public string Name { get; set; }
            bool _archived; public bool Archived { get => _archived; set { if (_archived != value) Changed = true; _archived = value; } }
            DateTime? _historyCheckedOn; public DateTime? HistoryCheckedOn { get => _historyCheckedOn; set { if (_historyCheckedOn != value) Changed = true; _historyCheckedOn = value; } }
            DateTime? _historyMessageOn; public DateTime? HistoryMessageOn { get => _historyMessageOn; set { if (_historyMessageOn != value) Changed = true; _historyMessageOn = value; } }
        }

        public async Task<string> ArchiveChannelAsync(ISlackApiClient slack, IDbConnection conn, ILogger log, CancellationToken? cancellationToken = null)
        {
            // Archive when older than 90-days
            var messagesToPull = 1; //15;
            var archiveOn = DateTime.Today.AddDays(-90);
            log?.LogInformation($"Archive @ {archiveOn:d} - {90:n0} day(s)");
            var entities = await slack.Conversations.List(cancellationToken: cancellationToken).PagedMetadata(x => x.Channels, x => x.ResponseMetadata, cursor => slack.Conversations.List(cursor: cursor, cancellationToken: cancellationToken));
            foreach (var entity in entities.Where(x => !x.IsExtShared && !x.IsArchived && !x.Name.StartsWith("_")))
            {
                Thread.Sleep(750);
                var messages = (await slack.Conversations.History(entity.Id, oldestTs: archiveOn.ToTimestamp(), limit: messagesToPull)).Messages;
                // update
                var channel = await conn.QueryFirstOrDefaultAsync<ChannelResult_>("Select Id, CreatedOn, Flag, Name, Archived, HistoryCheckedOn, HistoryMessageOn From [ops].[SlackChannel] Where Id = @Id", new { entity.Id }, commandType: CommandType.Text);
                if (channel == null)
                    continue;
                channel.Changed = false;
                channel.Flag = entity.IsPrivate() ? 1 : 0;
                var firstMessage = messages
                    //.Where(x => x.Text.IndexOf("> un-archived the channel") < 0 && x.Text.IndexOf("> archived the channel") < 0 && x.Text.IndexOf("> has joined the channel") < 0)
                    .FirstOrDefault();
                channel.HistoryCheckedOn = archiveOn;
                channel.HistoryMessageOn = firstMessage != null ? (DateTime?)firstMessage.Timestamp.ClampSeconds() : null;
                var messageOn = channel.HistoryMessageOn ?? channel.CreatedOn;
                var shouldArchive = messageOn < archiveOn;
                channel.Archived = entity.IsArchived;
                log?.LogInformation($"{channel.Name}: {channel.HistoryMessageOn:d} - {(messageOn - archiveOn).TotalDays:n0} day(s)");

                // check to archive channel
                if (!entity.IsArchived && shouldArchive)
                {
                    channel.Archived = true;
                    await StatusChannelAsync(slack, conn, log, entity, true);
                }
                if (channel.Changed)
                    await conn.ExecuteAsync("Update [ops].[SlackChannel] Set Flag = @Flag, Archived = @Archived, HistoryCheckedOn = @HistoryCheckedOn, HistoryMessageOn = @HistoryMessageOn Where Id = @Id",
                        new { channel.Id, channel.Flag, channel.Archived, channel.HistoryCheckedOn, channel.HistoryMessageOn });
            }
            return null;
        }
    }
}