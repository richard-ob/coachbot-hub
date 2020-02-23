using CoachBot.Database;
using CoachBot.Model;
using System.Linq;
using Discord;
using CoachBot.Domain.Model.Dtos;
using CoachBot.Domain.Extensions;

namespace CoachBot.Domain.Services
{
    public class PlayerService
    {
        private readonly CoachBotContext _coachBotContext;

        public PlayerService(CoachBotContext coachBotContext)
        {
            _coachBotContext = coachBotContext;
        }

        public Player GetPlayer(int playerId)
        {
            return _coachBotContext.Players.Single(p => p.Id == playerId);
        }

        public PagedResult<Player> GetPlayers(int page, int pageSize, string sortOrder)
        {
            return _coachBotContext.Players
                .Where(p => p.DiscordUserId != null)
                .GetPaged(page, pageSize, sortOrder);
        }

        public Player GetPlayer(IUser user, bool createIfNotExists = false)
        {
            var player = _coachBotContext.Players.FirstOrDefault(p => p.DiscordUserId == user.Id);

            if (createIfNotExists && player == null)
            {
                player = CreatePlayer(user.Username, user.Id, user.Mention);
            }

            return player;
        }

        public Player GetPlayer(string playerName, bool createIfNotExists = false)
        {
            var player = _coachBotContext.Players.FirstOrDefault(p => string.Equals(p.Name, playerName, System.StringComparison.CurrentCultureIgnoreCase));

            if (createIfNotExists && player == null && !playerName.StartsWith('@'))
            {
                player = CreatePlayer(playerName);
            }

            return player;
        }

        public Player GetPlayer(ulong discordUserId, bool createIfNotExists = false, string playerName = null)
        {
            var player = _coachBotContext.Players.FirstOrDefault(p => p.DiscordUserId == discordUserId);

            if (createIfNotExists && player == null)
            {
                player = CreatePlayer(playerName);
            }

            return player;
        }

        public void UpdatePlayerSteamID(ulong discordUserId, ulong steamId, string playerName)
        {
            // TODO: Ensure player is not an admin for security purposes
            var player = GetPlayer(discordUserId, createIfNotExists: true, playerName: playerName);
            player.SteamID = steamId;
            _coachBotContext.SaveChanges();
        }

        private Player CreatePlayer(string playerName, ulong? discordUserId = null, string discordUserMention = null, ulong? steamId = null)
        {
            var player = new Player()
            {
                Name = playerName,
                DiscordUserId = discordUserId,
                SteamID = steamId
            };

            _coachBotContext.Players.Add(player);
            _coachBotContext.SaveChanges();

            return player;
        }
    }
}
