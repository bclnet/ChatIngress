using ChatIngress.Slack.SlackNet.Interaction;
using SlackNet;
using SlackNet.Interaction;
using SlackNet.Interaction.Experimental;

namespace ChatIngress
{
    public class PrimaryCommandHandler : SlashCompositeCommandHandler
    {
        public const string CommandSky = "/sky";
        public const string CommandBeta = "/beta-{Config.Cmd}";
        public const string Command = "/{Config.Cmd}";

        public PrimaryCommandHandler(ISlackApiClient slack, IAsyncSlashCommandHandler commandHandler) : base(commandHandler, $"/{Config.Cmd} help") { }

        protected override SlashCommand NextCommand(SlashCommand command)
        {
            command.Command = Command;
            if (command.Text.StartsWith("admin ", System.StringComparison.OrdinalIgnoreCase)) command.Text = $"admin-{command.Text[6..]}";
            return base.NextCommand(command);
        }
    }
}
