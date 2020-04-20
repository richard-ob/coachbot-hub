﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CoachBot.Domain.Model
{
    public class PlayerStatisticFilters
    {
        public StatisticsTimePeriod TimePeriod { get; set; }

        public int? TeamId { get; set; }

        public int? ChannelId { get; set; }

        public int? PositionId { get; set; }

        public bool IncludeSubstituteAppearances { get; set; } = true;
    }
}