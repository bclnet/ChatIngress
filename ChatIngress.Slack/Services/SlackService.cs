using Microsoft.Extensions.Logging;
using SlackNet;
using System;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ChatIngress.Slack.Services
{
    public interface ISlackService
    {
        Task<(bool exists, string parentName)> HasParentChannelAsync(ISlackApiClient slack, string channelName, CancellationToken? cancellationToken = null);
        Task<(Nameable<object> id, object id2, string channelName)?> GetAccountChannelNameAsync(string accountId, CancellationToken? cancellationToken = null);
        Task<string> ArchiveChannelAsync(ISlackApiClient slack, IDbConnection db, ILogger log, CancellationToken? cancellationToken = null);
        Task<string> StatusChannelAsync(ISlackApiClient slack, IDbConnection db, ILogger log, Conversation channel, bool archive, bool bump = true, CancellationToken? cancellationToken = null);
        Task<string> CreateChannelAsync(ISlackApiClient slack, IDbConnection db, ILogger log, Nameable<(string id, string email)> requester, Conversation s, Nameable<object> accountId, object accountId2, CancellationToken? cancellationToken = null);
        Task<Stream> ReportAsync(IDbConnection db, DateTime from, DateTime till, CancellationToken? cancellationToken = null);
    }

    public partial class SlackService : ISlackService
    {
        const string SlackMgmtEmail = "e6m8i3m1o5p5o7u1@degdigital.slack.com";
        const string SlackLogEmail = "c2q2l2x5k3i6n8g7@degdigital.slack.com";
        const string SlackRootUrl = "https://degdigital.slack.com/";
        public static string DbName = "Main";

        public static Func<string, CancellationToken?, Task<(Nameable<object> id, object id2, string abbreviation)?>> LookupAccountNameAsync { get; set; }
        public static Func<(Nameable<(string id, string email)> to, string cc, string subject, string body), Task> SendEmailAsync { get; set; }

        static readonly Regex _channelNamePattern = new(@"[^A-Za-z0-9\-_]");
        readonly ISlackContext _slack;

        public SlackService(ISlackContext slack) => _slack = slack;

        public async Task<(bool exists, string parentName)> HasParentChannelAsync(ISlackApiClient slack, string name, CancellationToken? cancellationToken = null)
        {
            var idx = name.IndexOf("-");
            if (idx == -1) return (true, name);
            name = name[..idx];
            return (await slack.Undocumented().ConversationsExists(name), name);
        }

        public async Task<(Nameable<object> id, object id2, string channelName)?> GetAccountChannelNameAsync(string accountId, CancellationToken? cancellationToken = null)
        {
            if (LookupAccountNameAsync == null) throw new InvalidOperationException(nameof(LookupAccountNameAsync));
            var accountName = await LookupAccountNameAsync(accountId, cancellationToken);
            if (accountName == null) return null;
            var (id, id2, abbreviation) = accountName.Value;
            var newName = (!string.IsNullOrEmpty(abbreviation) ? abbreviation : id.Name).ToLowerInvariant();
            newName = string.Join(string.Empty, _channelNamePattern.Split(newName));
            return (id, id2, newName.Length <= 15 ? newName : newName.Substring(0, 15));
        }

        async Task EmailChannelAsync(Nameable<(string id, string email)> requester, Conversation s, Nameable<object> accountId)
        {
            if (SendEmailAsync == null) throw new InvalidOperationException(nameof(SendEmailAsync));
            var body = BuildChannelEmail(requester, s, accountId);
            var cc = s.Creator != "Official" && s.Creator != "Initiative" ? SlackLogEmail : SlackMgmtEmail;
            await SendEmailAsync((to: requester, cc, subject: "New Channel", body));
        }

        static XAttribute XAttribute(string name, bool value) => value ? new XAttribute(name, "1") : null;
        static XAttribute XAttribute(string name, string value) => !string.IsNullOrEmpty(value) ? new XAttribute(name, value) : null;
    }
}