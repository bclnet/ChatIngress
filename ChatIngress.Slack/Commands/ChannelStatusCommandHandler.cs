using ChatIngress.Slack.Services;
using Contoso.Extensions.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using SlackNet;
using System.Threading;
using System.Threading.Tasks;

namespace ChatIngress.Slack.Commands
{
    public class ChannelStatusCommandHandler : IRequestHandler<ChannelStatusCommand, string>
    {
        static readonly IDbService _dbService = new DbService();
        readonly ISlackContext _slack;
        readonly ISlackService _service;
        readonly ILogger _log;

        public ChannelStatusCommandHandler(ISlackContext slack, ISlackService service, ILogger<ChannelStatusCommandHandler> log)
        {
            _slack = slack;
            _service = service;
            _log = log;
        }

        public async Task<string> Handle(ChannelStatusCommand s, CancellationToken cancellationToken)
        {
            var slack = _slack.Connect();
            using var conn = _dbService.GetConnection(SlackService.DbName);
            return await _service.StatusChannelAsync(slack, conn, _log, new Conversation { Id = s.ChannelId, Name = s.ChannelName }, s.Archive);
        }
    }
}
