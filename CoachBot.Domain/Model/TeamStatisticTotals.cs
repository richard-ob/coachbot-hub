using System;
using System.Collections.Generic;
using System.Text;

namespace CoachBot.Domain.Model
{
    public class TeamStatisticTotals
    {
        public int ChannelId { get; set; }

        public Channel Channel { get; set; }

        public StatisticTotals StatisticTotals { get; set; }
    }
}
