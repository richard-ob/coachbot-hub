using CoachBot.Database;
using CoachBot.Domain.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoachBot.Domain.Services
{
    public class FantasyService
    {
        private readonly CoachBotContext _coachBotContext;

        public FantasyService(CoachBotContext coachBotContext)
        {
            _coachBotContext = coachBotContext;
        }

        public IEnumerable<FantasyTeam> GetFantasyTeams(int tournamentEditionId)
        {
            return _coachBotContext.FantasyTeams.Where(ft => ft.TournamentEditionId == tournamentEditionId).ToList();
        }

        public FantasyTeam GetFantasyTeam(int fantasyTeamId)
        {
            return _coachBotContext.FantasyTeams
                .Include(ft => ft.FantasyTeamSelections)
                    .ThenInclude(ft => ft.FantasyPlayer)
                    .ThenInclude(ft => ft.Player)
                    .ThenInclude(ft => ft.Teams)
                    .ThenInclude(ft => ft.Team)
                    .ThenInclude(ft => ft.BadgeImage)
                .Include(ft => ft.FantasyTeamSelections)
                    .ThenInclude(ft => ft.FantasyPlayer)
                    .ThenInclude(ft => ft.Player.Country)
                .First(ft => ft.Id == fantasyTeamId);
        }

        public void CreateFantasyTeam(FantasyTeam fantasyTeam)
        {
            CheckHasTournamentStarted((int)fantasyTeam.TournamentEditionId);
            fantasyTeam.IsFinalised = false;
            fantasyTeam.FantasyTeamSelections = null;
            fantasyTeam.TournamentEdition = null;
            _coachBotContext.FantasyTeams.Add(fantasyTeam);
            _coachBotContext.SaveChanges();
        }

        public void UpdateFantasyTeam(FantasyTeam fantasyTeam)
        {
            CanUpdateTeam(fantasyTeam.Id);
            var existingFantasyTeam = _coachBotContext.FantasyTeams.Find(fantasyTeam.Id);
            existingFantasyTeam.IsFinalised = fantasyTeam.IsFinalised;
            existingFantasyTeam.Name = fantasyTeam.Name;
            _coachBotContext.FantasyTeams.Update(existingFantasyTeam);
            _coachBotContext.SaveChanges();
        }

        public void DeleteFantasyTeam(int fantasyTeamId)
        {
            CanUpdateTeam(fantasyTeamId);
            var fantasyTeam = _coachBotContext.FantasyTeams.Find(fantasyTeamId);
            _coachBotContext.FantasyTeams.Remove(fantasyTeam);
            _coachBotContext.SaveChanges();
        }

        public void AddFantasyTeamSelection(FantasyTeamSelection fantasyTeamSelection)
        {
            CanUpdateTeam((int)fantasyTeamSelection.FantasyTeamId);
            _coachBotContext.FantasyTeamSelections.Add(fantasyTeamSelection);
            _coachBotContext.SaveChanges();
        }

        public void RemoveFantasyTeamSelection(int fantasyTeamSelectionId)
        {
            var selection = _coachBotContext.FantasyTeamSelections.Find(fantasyTeamSelectionId);
            CanUpdateTeam((int)selection.FantasyTeamId);

            _coachBotContext.FantasyTeamSelections.Remove(selection);
            _coachBotContext.SaveChanges();
        }

        public List<FantasyPlayer> GetFantasyPlayers(PlayerStatisticFilters playerStatisticFilters)
        {
            return _coachBotContext
                .FantasyPlayers
                .Include(fp => fp.Player)
                .ThenInclude(p => p.Teams)
                .ThenInclude(t => t.Team)
                .ThenInclude(t => t.BadgeImage)
                .Include(fp => fp.Player)
                .ThenInclude(fp => fp.Country)
                .Where(p => playerStatisticFilters.MaximumRating == null || p.Player.Rating <= playerStatisticFilters.MaximumRating)
                .Where(p => playerStatisticFilters.MinimumRating == null || p.Player.Rating >= playerStatisticFilters.MinimumRating)
                .Where(p => playerStatisticFilters.PositionGroup == null || p.PositionGroup == playerStatisticFilters.PositionGroup)
                .Where(p => playerStatisticFilters.TeamId == null || p.Player.Teams.Any(t => t.TeamId == playerStatisticFilters.TeamId && t.IsCurrentTeam))
                .Where(p => p.TournamentEditionId == playerStatisticFilters.TournamentEditionId)
                .Take(10)
                .ToList();
        }

        public List<FantasyTeam> GetFantasyTeamsForPlayer(ulong discordUserId)
        {
            return _coachBotContext.FantasyTeams
                .Where(t => t.Player.DiscordUserId == discordUserId)
                .ToList();
        }

        public List<TournamentEdition> GetAvailableTournamentsForUser(ulong discordUserId)
        {
            return _coachBotContext.TournamentEditions
                .Include(t => t.Tournament)
                .Where(t => !_coachBotContext.Matches.Any(m => m.TournamentId == t.Id && m.ScheduledKickOff < DateTime.Now))
                .Where(t => !_coachBotContext.FantasyTeams.Any(ft => ft.Player.DiscordUserId == discordUserId))
                .ToList();
        }

        public void GenerateFantasyPlayersSnapshot(int tournamentEditionId)
        {
            if (_coachBotContext.FantasyPlayers.Any(t => t.TournamentEditionId == tournamentEditionId))
            {
                throw new Exception("Fantasy season already seeded");
            }

            foreach(var player in _coachBotContext.Players)
            {
                player.Rating = GetRandomRating();
                var fantasyPlayer = new FantasyPlayer()
                {
                    TournamentEditionId = tournamentEditionId,
                    PlayerId = player.Id,
                    PositionGroup = GetRandomPositionGroup(),
                    Rating = player.Rating
                };
                _coachBotContext.FantasyPlayers.Add(fantasyPlayer);
            }
            _coachBotContext.SaveChanges();
        }

        static float GetRandomRating()
        {
            Random r = new Random();
            string beforePoint = r.Next(4, 9).ToString();
            string afterPoint = r.Next(0, 9).ToString();
            string combined = beforePoint + "." + afterPoint;
            return float.Parse(combined);
        }

        static PositionGroup GetRandomPositionGroup()
        {
            return (PositionGroup)new Random().Next(0, 3);
        }

        private void CanUpdateTeam(int fantasyTeamId)
        {
            var fantasyTeam = _coachBotContext.FantasyTeams.Find(fantasyTeamId);

            if (fantasyTeam.IsFinalised)
            {
                throw new Exception("Teams cannot be updated after they have been finalised");
            }

            CheckHasTournamentStarted((int)fantasyTeam.TournamentEditionId);
        }

        private void CheckHasTournamentStarted(int tournamentEditionId)
        {
            if (_coachBotContext.Matches.Any(m => m.TournamentId == tournamentEditionId && m.ScheduledKickOff < DateTime.Now))
            {
                throw new Exception("The tournament has started. Fantasy teams cannot be updated.");
            }
        }
    }
}
