﻿using System;

namespace CoachBot.Domain.Model
{
    public class MatchFilters
    {
        public StatisticsTimePeriod TimePeriod { get; set; }

        public MatchType? MatchType { get; set; }

        public MatchFormat? MatchFormat { get; set; }

        public int? RegionId { get; set; }

        public int? PlayerId { get; set; }

        public int? TeamId { get; set; }

        public int? TournamentId { get; set; }

        public bool IncludePast { get; set; } = false;

        public bool IncludeUpcoming { get; set; } = false;

        public bool IncludeUnpublished { get; set; } = false;

        public bool IncludePlaceholders { get; set; } = false;

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }
    }
}