using SlackNet;
using SlackNet.Blocks;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;
using SlackNet.WebApi;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace ChatIngress
{
    public class HelpCommandHandler : AbstractCommandHandler, IBlockActionHandler<ButtonAction>
    {
        public const string Command = $"/{Config.Cmd} help";
        public static string HELP(string ctx) => $"{ctx}-help";

        readonly ISlackApiClient _slack;
        readonly Dictionary<string, IAsyncSlashCommandHandler> _handlers;

        public HelpCommandHandler(ISlackApiClient slack, IAsyncSlashCommandHandler commandHandler)
        {
            _slack = slack;
            _handlers = (commandHandler.GetType().GetField("_handlers", BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new Exception("Unable to find _handlers on commandHandler"))
                .GetValue(commandHandler) as Dictionary<string, IAsyncSlashCommandHandler>;
        }

        public override async Task<SlashCommandResponse> Handle(SlashCommand command)
        {
            if (string.IsNullOrEmpty(command.Text))
                return new SlashCommandResponse
                {
                    Message = Help.AddDismissAttachment()
                };
            command.Command = command.Command.Replace("help", command.Text);
            command.Text = "help";
            if (!_handlers.TryGetValue(command.Command, out var handler))
                return new SlashCommandResponse
                {
                    Message = InvalidCommand(command.Text).AddDismissAttachment()
                };
            SlashCommandResponse res = null;
            await handler.Handle(command, x => { res = x; return Task.CompletedTask; });
            return res;
        }

        public Task Handle(ButtonAction action, BlockActionRequest request) => Task.CompletedTask;
        //{
        //    var actionId = action.ActionId.Replace("-help", string.Empty);
        //    var helpMessage = _contexts.GetHelp(actionId, request.User.Id);
        //    helpMessage.Attachments = _messageBuilder.Attachments(_contexts.GetContexts(request.User.Id), actionId);
        //    await _slack.PostEphemeralMessageUpdate(request.ResponseUrl, helpMessage);
        //}

        // https://www.webfx.com/tools/emoji-cheat-sheet/
        public override Message Help => new Message
        {
            Blocks = new[] { new Markdown(
$@":dizzy: *Kudo*
`/{Config.Cmd} kudo @name [reason sentence]` gives kudos to a person. `/{Config.Cmd} kudo help` for more information.

:notebook: *Planning*
`/{Config.Cmd} planning` for build planning excel. `/{Config.Cmd} planning help` for more information.

:bar_chart: *Revenue*
`/{Config.Cmd} revenue` for revenue reports. `/{Config.Cmd} revenue help` for more information.

:slack: *Slack*
`/{Config.Cmd} slack` creates or requests a new slack channel. `/{Config.Cmd} slack help` for more information.
").ToSectionBlock() },

        };
    }
}