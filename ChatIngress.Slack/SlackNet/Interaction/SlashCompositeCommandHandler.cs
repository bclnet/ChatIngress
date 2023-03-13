using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace ChatIngress.Slack.SlackNet.Interaction
{
    public class SlashCompositeCommandHandler : ISlashCommandHandler
    {
        readonly Dictionary<string, IAsyncSlashCommandHandler> _handlers;
        readonly IAsyncSlashCommandHandler _commandHandler;
        readonly string _defaultCommand;

        public SlashCompositeCommandHandler(IAsyncSlashCommandHandler commandHandler, string defaultCommand)
        {
            _handlers = (commandHandler.GetType().GetField("_handlers", BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new Exception("Unable to find _handlers on commandHandler"))
                .GetValue(commandHandler) as Dictionary<string, IAsyncSlashCommandHandler>;
            _commandHandler = commandHandler;
            _defaultCommand = defaultCommand;
        }

        public async Task<SlashCommandResponse> Handle(SlashCommand command)
        {
            SlashCommandResponse res = null;
            await _commandHandler.Handle(NextCommand(command), x => { res = x; return Task.CompletedTask; }).ConfigureAwait(false);
            return res;
        }

        protected virtual SlashCommand NextCommand(SlashCommand command)
        {
            var text = command.Text;
            var idx = text.IndexOf(' ');
            if (idx == -1)
            {
                command.Command += $" {text.ToLowerInvariant()}";
                command.Text = null;
            }
            else
            {
                command.Command += $" {text.Substring(0, idx).ToLowerInvariant()}";
                command.Text = text.Substring(idx + 1);
            }
            if (!_handlers.ContainsKey(command.Command))
            {
                command.Command = _defaultCommand;
                command.Text = null;
            }
            return command;
        }
    }
}
