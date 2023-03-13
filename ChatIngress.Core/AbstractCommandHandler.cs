using SlackNet;
using SlackNet.Blocks;
using SlackNet.Interaction;
using SlackNet.WebApi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatIngress
{
    public abstract class AbstractCommandHandler : ISlashCommandHandler
    {
        public virtual Message NotAuthorizedError =>
            new()
            {
                Blocks = new[] { new Markdown("Sorry, you are not authorized to use administrator commands.").ToSectionBlock() }
            };

        public Message InvalidCommand(string command = null) => new()
        {
            Blocks = new[] { new Markdown($"Sorry, that is not a valid command. Try `/{Config.Cmd} help{(string.IsNullOrEmpty(command) ? null : $" {command.Replace($"/{Config.Cmd} ", "")}")}` to see a list of options.").ToSectionBlock() }
        };

        public virtual async Task<SlashCommandResponse> Handle(SlashCommand command) => IsAuthorized(command)
            ? await HandleAuthorized(command)
            : new SlashCommandResponse { Message = NotAuthorizedError };

        public virtual Task<SlashCommandResponse> HandleAuthorized(SlashCommand command) => throw new NotImplementedException();

        public virtual Message Help => null;

        public virtual bool IsAuthorized(SlashCommand command) => true;

        public virtual List<string> Administrators => Config.Administrators;
    }
}