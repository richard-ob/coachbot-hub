using CoachBot.Domain.Model;
using CoachBot.Shared.Model;
using CoachBot.Tools;
using Discord.Commands;
using System.Threading.Tasks;

namespace CoachBot.Modules
{
    public class UtilityModule : ModuleBase
    {
        private readonly Config _config;

        public UtilityModule(Config config)
        {
            _config = config;
        }

        [Command("!help")]
        public async Task HelpAsync()
        {
            await ReplyAsync("", embed: DiscordEmbedHelper.GenerateEmbed($"A full list of Coach commands is available at https://{_config.WebServerConfig.ClientUrl}bot-manual.", ServiceResponseStatus.Info));
        }
    }
}
