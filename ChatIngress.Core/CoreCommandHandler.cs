using Microsoft.Extensions.Logging;
using SlackNet;
using SlackNet.Blocks;
using SlackNet.Interaction;
using SlackNet.WebApi;
using System;
using System.Threading.Tasks;

namespace ChatIngress
{
    public class CoreCommandHandler : AbstractCommandHandler
    {
        public const string Command = $"/{Config.Cmd} core";

        readonly ILogger<CoreCommandHandler> _log;
        readonly ISlackApiClient _slack;
        readonly CoreMessageBuilder _messageBuilder;

        public CoreCommandHandler(ILogger<CoreCommandHandler> log, ISlackApiClient slack, CoreMessageBuilder messageBuilder)
        {
            _log = log;
            _slack = slack;
            _messageBuilder = messageBuilder;
        }

        public override Task<SlashCommandResponse> Handle(SlashCommand command)
        {
            try
            {
                Message message = null;
                return Task.FromResult(new SlashCommandResponse
                {
                    Message = message?.AddDismissAttachment()
                });
            }
            catch (Exception e)
            {
                return Task.FromResult(new SlashCommandResponse
                {
                    Message = new Message { Text = e.Message }.AddDismissAttachment()
                });
            }
        }

        public override Message Help => new()
        {
            Blocks = new[] { new Markdown(
$@":slack: *CMD*
WORDS.

*Using /{Config.Cmd} action*
").ToSectionBlock() }
        };
    }
}