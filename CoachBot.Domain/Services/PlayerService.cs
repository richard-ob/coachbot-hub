using CoachBot.Database;
using CoachBot.Model;
using System.Linq;
using Discord;

namespace CoachBot.Domain.Services
{
    public class PlayerService
    {
        private readonly CoachBotContext _coachBotContext;

        public PlayerService(CoachBotContext coachBotContext)
        {
            _coachBotContext = coachBotContext;
        }

        public Player GetPlayer(IUser user)
        {
            var player = _coachBotContext.Players.FirstOrDefault(p => p.DiscordUserId == user.Id);

            if (player == null)
            {
                player = CreatePlayer(user.Username, user.Id, user.Mention);
            }

            return player;
        }

        public Player GetPlayer(string playerName)
        {
            var player = _coachBotContext.Players.FirstOrDefault(p => string.Equals(p.Name, playerName, System.StringComparison.CurrentCultureIgnoreCase));

            if (player == null)
            {
                player = CreatePlayer(playerName);
            }

            return player;
        }

        private Player CreatePlayer(string playerName, ulong? discordUserId = null, string discordUserMention = null)
        {
            var player = new Player()
            {
                Name = playerName,
                DiscordUserId = discordUserId,
                DiscordUserMention = discordUserMention
            };

            _coachBotContext.Players.Add(player);
            _coachBotContext.SaveChanges();

            return player;
        }
    }
}
