using Microsoft.Extensions.DependencyInjection;
using SlackNet.AspNetCore;
using SlackNet.Blocks;

namespace ChatIngress
{
    public static class CoreConfigurationManager
    {
        public static IServiceCollection RegisterCoreServices(this IServiceCollection c) => c
            .AddSingleton<CoreMessageBuilder>();

        public static SlackServiceConfiguration RegisterCoreServices(this SlackServiceConfiguration c) => c
            .RegisterSlashCommandHandler<HelpCommandHandler>(HelpCommandHandler.Command)
            .RegisterSlashCommandHandler<CoreCommandHandler>(CoreCommandHandler.Command)
            .RegisterSlashCommandHandler<PrimaryCommandHandler>(PrimaryCommandHandler.CommandSky)
            .RegisterSlashCommandHandler<PrimaryCommandHandler>(PrimaryCommandHandler.CommandBeta)
            .RegisterSlashCommandHandler<PrimaryCommandHandler>(PrimaryCommandHandler.Command)
            .RegisterBlockActionHandler<ButtonAction, HelpCommandHandler>(HelpCommandHandler.HELP(HelpCommandHandler.Command))
            .RegisterBlockActionHandler<ButtonAction, DismissActionHandler>(DismissActionHandler.DISMISS)
            .RegisterBlockActionHandler<ButtonAction, DismissActionHandler>(DismissActionHandler.STAMP);
    }
}
