using ChatIngress.Slack.Services;
using Contoso.Extensions.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using SlackNet;
using System.Threading;
using System.Threading.Tasks;

namespace ChatIngress.Slack.Commands
{
    public class ChannelCreateCommandHandler : IRequestHandler<ChannelCreateCommand, string>
    {
        static readonly IDbService _dbService = new DbService();
        readonly ISlackContext _slack;
        readonly ISlackService _service;
        readonly ILogger _log;

        public ChannelCreateCommandHandler(ISlackContext slack, ISlackService service, ILogger<ChannelCreateCommandHandler> log)
        {
            _slack = slack;
            _service = service;
            _log = log;
        }

        public async Task<string> Handle(ChannelCreateCommand s, CancellationToken cancellationToken)
        {
            var slack = _slack.Connect();
            using var conn = _dbService.GetConnection(SlackService.DbName);
            string id = null;
            do
            {
                var newId = await _service.CreateChannelAsync(slack, conn, _log, s.Requester, new Conversation
                {
                    Creator = s.ChannelType,
                    Name = s.ChannelName,
                    IsPrivate = s.Private,
                    Purpose = new Purpose { Value = s.Purpose?.Left(100) },
                }, s.AccountId, s.AccountId2, cancellationToken);
                id ??= newId;
                s = s.NextCommand;
            }
            while (s != null);
            return id;
        }
    }
}
