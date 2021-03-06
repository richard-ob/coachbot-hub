﻿using CoachBot.Database;
using CoachBot.Domain.Model;
using CoachBot.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace CoachBot.Domain.Services
{
    public class PlayerProfileService
    {
        private readonly CoachBotContext _coachBotContext;

        public PlayerProfileService(CoachBotContext coachBotContext)
        {
            _coachBotContext = coachBotContext;
        }

        public PlayerProfile GeneratePlayerProfile(int playerId)
        {
            var player = _coachBotContext.Players.Include(p => p.Country).Single(p => p.Id == playerId);

            var clubPlayerTeam = GetTeam(playerId, TeamType.Club);
            var nationalPlayerTeam = GetTeam(playerId, TeamType.National);

            var playerProfile = new PlayerProfile()
            {
                Position = GetMostCommonPosition(playerId),
                Rating = player.Rating,
                ClubTeam = clubPlayerTeam?.Team,
                ClubTeamRole = clubPlayerTeam?.TeamRole,
                NationalTeam = nationalPlayerTeam?.Team,
                Country = player.Country,
                Name = player.DisplayName
            };

            return playerProfile;
        }

        private PlayerTeam GetTeam(int playerId, TeamType teamType)
        {
            return _coachBotContext.PlayerTeams
                .AsQueryable()
                .Where(pt => pt.PlayerId == playerId)
                .Where(pt => pt.Team.TeamType == teamType)
                .Where(pt => pt.LeaveDate == null)
                .Where(pt => pt.IsPending == false)
                .OrderByDescending(pt => pt.JoinDate)
                .Include(t => t.Team)
                .ThenInclude(t => t.BadgeImage)
                .FirstOrDefault();
        }

        public Position GetMostCommonPosition(int playerId, int? teamId = null)
        {
            var topPosition = _coachBotContext.PlayerPositionMatchStatistics
                 .AsNoTracking()
                 .Where(p => p.PlayerId == playerId)
                 .Where(p => p.Match.KickOff > DateTime.UtcNow.AddMonths(-6))
                 .Where(p => teamId == null || p.TeamId == teamId)
                 .GroupBy(p => new { p.Position.Id, p.Position.Name })
                 .Select(p => new PositionAppearances()
                 {
                     Appearances = p.Count(),
                     Position = new Position()
                     {
                         Name = p.Key.Name,
                         Id = p.Key.Id
                     }
                 })
                 .OrderByDescending(p => p.Appearances)
                 .FirstOrDefault();

            return topPosition.Position;
        }

        private struct PositionAppearances
        {
            public int Appearances;
            public Position Position;
        }
    }
}