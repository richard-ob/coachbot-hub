using CoachBot.Database;
using CoachBot.Model;
using System.Linq;

namespace CoachBot.Domain.Services
{
    public class GuildService
    {
        private readonly CoachBotContext _coachBotContext;
        private readonly DiscordService _discordService;

        public GuildService(CoachBotContext coachBotContext, DiscordService discordService)
        {
            _coachBotContext = coachBotContext;
            _discordService = discordService;
        }

        public Guild GetGuildByDiscordId(ulong discordGuildId, bool createIfNotExists = false)
        {
            var guild = _coachBotContext.Guilds.FirstOrDefault(g => g.DiscordGuildId == discordGuildId);

            if (guild == null && createIfNotExists)
            {
                var discordGuild = _discordService.GetDiscordGuild(discordGuildId);
                guild = new Guild()
                {
                    Name = discordGuild.Name,
                    DiscordGuildId = discordGuildId
                };
            }

            return guild;
        }
    }
}