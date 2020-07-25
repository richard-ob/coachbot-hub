using CoachBot.Database;
using CoachBot.Domain.Extensions;
using CoachBot.Domain.Model;
using CoachBot.Domain.Model.Dtos;
using CoachBot.Model;
using Discord;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public Player GetPlayer(ulong steamId)
        {
            return _coachBotContext.Players.Single(p => p.SteamID == steamId);
        }

        public PagedResult<Player> GetPlayers(int page, int pageSize, string sortOrder)
        {
            return _coachBotContext.Players
                .Where(p => p.DiscordUserId != null)
                .GetPaged(page, pageSize, sortOrder);
        }

        public Player GetPlayer(IUser user, bool createIfNotExists = false)
        {
            var player = _coachBotContext.Players.Include(p => p.Positions).ThenInclude(p => p.Position).FirstOrDefault(p => p.DiscordUserId == user.Id);

            if (createIfNotExists && player == null)
            {
                player = CreatePlayer(user.Username, user.Id, user.Mention);
            }

            return player;
        }

        public Player GetPlayer(string playerName, bool createIfNotExists = false)
        {
            var player = _coachBotContext.Players.Include(p => p.Positions).ThenInclude(p => p.Position).FirstOrDefault(p => string.Equals(p.Name, playerName, System.StringComparison.CurrentCultureIgnoreCase));

            if (createIfNotExists && player == null && !playerName.StartsWith('@'))
            {
                player = CreatePlayer(playerName);
            }

            return player;
        }

        public Player GetPlayer(ulong discordUserId, bool createIfNotExists = false, string playerName = null)
        {
            var player = _coachBotContext.Players
                .Include(p => p.Positions)
                    .ThenInclude(p => p.Position)
                .Include(p => p.Teams)
                    .ThenInclude(t => t.Team)
                    .ThenInclude(t => t.BadgeImage)
                .Include(p => p.Country)
                .FirstOrDefault(p => p.DiscordUserId == discordUserId);

            if (createIfNotExists && player == null)
            {
                player = CreatePlayer(playerName, discordUserId);
            }

            return player;
        }

        public Player GetPlayerBySteamId(ulong steamUserId, bool createIfNotExists = false, string playerName = null)
        {
            var player = _coachBotContext.Players
                .Include(p => p.Positions)
                    .ThenInclude(p => p.Position)
                .Include(p => p.Teams)
                    .ThenInclude(t => t.Team)
                    .ThenInclude(t => t.BadgeImage)
                .Include(p => p.Country)
                .FirstOrDefault(p => p.SteamID == steamUserId);

            if (createIfNotExists && player == null)
            {
                player = CreatePlayer(playerName, steamUserId);
            }

            return player;
        }

        public List<Player> SearchPlayersByName(string playerName)
        {
            return _coachBotContext.Players
                .Where(p => p.Name.Contains(playerName))
                .Take(10)
                .ToList();
        }

        public void UpdateDiscordUserId(ulong discordUserId, ulong steamId)
        {
            var player = GetPlayerBySteamId(steamId);
            if (player.HubRole == PlayerHubRole.Administrator || player.HubRole == PlayerHubRole.Owner)
            {
                throw new Exception("Administrators cannot verify their accounts in this manner");
            }

            player.DiscordUserId = discordUserId;
            _coachBotContext.SaveChanges();
        }

        public void UpdatePlayer(Player player)
        {
            var existingPlayer = _coachBotContext.Players.Single(p => p.Id == player.Id);
            existingPlayer.Name = player.Name;
            existingPlayer.CountryId = player.CountryId;
            existingPlayer.DisableDMNotifications = player.DisableDMNotifications;
            existingPlayer.PlayingSince = player.PlayingSince;
            _coachBotContext.Players.Update(player);
            _coachBotContext.SaveChanges();
        }

        public bool IsAdmin(ulong steamId)
        {
            var player = GetPlayerBySteamId(steamId);
            return player.HubRole.Equals(PlayerHubRole.Administrator);
        }

        public bool IsOwner(ulong steamId)
        {
            var player = GetPlayerBySteamId(steamId);
            return player.HubRole.Equals(PlayerHubRole.Owner);
        }

        public PlayerHubRole GetPlayerHubRole(ulong steamId)
        {
            var player = GetPlayerBySteamId(steamId);
            return player.HubRole;
        }

        private Player CreatePlayer(string playerName, ulong? discordUserId = null, string discordUserMention = null, ulong? steamId = null)
        {
            var player = new Player()
            {
                Name = playerName,
                DiscordUserId = discordUserId,
                SteamID = steamId
            };

            if (steamId.HasValue && _coachBotContext.Players.Any(p => p.SteamID == steamId) || steamId == 0)
            {
                throw new Exception("Player already exists with given SteamID");
            }

            _coachBotContext.Players.Add(player);
            _coachBotContext.SaveChanges();

            return player;
        }
    }
}