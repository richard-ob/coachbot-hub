using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachBot.Domain.Model
{
    public class TeamStatisticTotals : StatisticTotals
    {
        public int? ChannelId { get; set; }

        public int? TeamId { get; set; }

        public string TeamName { get; set; }

        public string BadgeImage { get; set; }

    }
}
