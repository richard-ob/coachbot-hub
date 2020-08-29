using CoachBot.Database;
using CoachBot.Tools;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CoachBot.Domain.Services
{
    public class AnnouncementService
    {
        private readonly CoachBotContext coachBotContext;
        private readonly DiscordNotificationService discordNotificationService;

        public AnnouncementService(CoachBotContext coachBotContext, DiscordNotificationService discordNotificationService)
        {
            this.coachBotContext = coachBotContext;
            this.discordNotificationService = discordNotificationService;
        }

        public async Task SendGlobalMessage(string title, string message, int? regionId)
        {
            var channels = this.coachBotContext.Channels
                .Where(c => this.coachBotContext.Matchups.Any(m => c.Id == m.LineupHome.ChannelId || c.Id == m.LineupAway.ChannelId && m.CreatedDate > DateTime.UtcNow.AddMonths(-1))) // INFO: Active channels only
                .Where(c => regionId == null || c.Team.RegionId == regionId)
                .Select(c => c.DiscordChannelId)
                .ToList();

            var embed = DiscordEmbedHelper.GenerateSimpleEmbed(message, $":loudspeaker: {title}");

            await discordNotificationService.SendChannelMessage(channels, embed);
        }
    }
}
