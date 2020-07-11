using CoachBot.Model;
using System;
using System.Collections.Generic;

namespace CoachBot.Domain.Model
{
    public class PlayerStatisticFilters
    {
        public StatisticsTimePeriod TimePeriod { get; set; }

        public int? PlayerId { get; set; }

        public int? TeamId { get; set; }

        public int? ChannelId { get; set; }

        public MatchTeamType? MatchTeamType { get; set; }

        public int? PositionId { get; set; }

        public int? MatchId { get; set; }

        public int? RegionId { get; set; }

        public int? TournamentId { get; set; }

        public bool IncludeSubstituteAppearances { get; set; } = true;

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public int? MinimumSecondsPlayed { get; set; }

        public float? MinimumRating { get; set; }

        public float? MaximumRating { get; set; }

        public PositionGroup? PositionGroup { get; set; }

        public string PlayerName { get; set; }

        public string PositionName { get; set; }

        public List<int> ExcludePlayers { get; set; } = new List<int>();
    }
}