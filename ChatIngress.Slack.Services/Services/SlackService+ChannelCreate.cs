using Dapper;
using Microsoft.Extensions.Logging;
using SlackNet;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace ChatIngress.Slack.Services
{
    public partial class SlackService
    {
        public async Task<string> CreateChannelAsync(ISlackApiClient slack, IDbConnection conn, ILogger log, Nameable<(string id, string email)> requester, Conversation s, Nameable<object> accountId, object accountId2, CancellationToken? cancellationToken = null)
        {
            string channelError = null;
            var request = s.Creator == "Official" | s.Creator == "Initiative";
            if (request)
            {
                log?.LogInformation($"Request #{s.Name} - {requester.Name}");
                s.Id = Guid.NewGuid().ToString();
            }
            else
                try
                {
                    log?.LogInformation($"Create #{s.Name} - {requester.Name}");
                    s.Id = (await slack.Conversations.Create(s.Name, s.IsPrivate, cancellationToken)).Id;
                    if (requester.Value.id != null) await slack.Conversations.Invite(s.Id, new[] { requester.Value.id }, cancellationToken);
                    if (!string.IsNullOrEmpty(s.Purpose?.Value)) await slack.Conversations.SetPurpose(s.Id, s.Purpose?.Value, cancellationToken);
                }
                catch (AggregateException ex)
                {
                    log?.LogCritical(ex, "Exception");
                    s.Id = Guid.NewGuid().ToString();
                    channelError = string.Empty;
                    ex.Handle(x =>
                    {
                        if (x is AggregateException exception) exception.Handle(y => { channelError += $"{y.Message}\n"; return true; });
                        else channelError += $"{x.Message}\n";
                        return true;
                    });
                    await InsertChannel(conn, requester, s, accountId, accountId2, channelError);
                    throw ex;
                }
            await InsertChannel(conn, requester, s, accountId, accountId2, channelError);
            await LogInsertChannel(conn, s);
            try
            {
                await EmailChannelAsync(requester, s, accountId);
            }
            catch (Exception ex) { log?.LogCritical(ex, "Email: Exception"); }
            return s.Id;
        }

        async Task InsertChannel(IDbConnection db, Nameable<(string id, string email)> requester, Conversation s, Nameable<object> accountId, object accountId2, string channelError) =>
            await db.ExecuteAsync("Insert [ops].[SlackChannel](Id, Requester, Name, Type, Purpose, BindId, BindIdName, BindId2, Error) Values(@Id, @Requester, @Name, @Type, @Purpose, @BindId, @BindIdName, @BindId2, @Error)", new
            {
                s.Id,
                Requester = requester.Value.id,
                s.Name,
                Type = s.Creator,
                Purpose = s.Purpose?.Value,
                BindId = (Guid?)accountId.Value,
                BindIdName = accountId.Name,
                BindId2 = (string)accountId2,
                Error = channelError,
            });
    }
}