using CoachBot.Model;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachBot.Domain.Model
{
    public class PlayerStatisticTotals : StatisticTotals
    {
        public int PlayerId { get; set; }

        public string Name { get; set; }

        public ulong? SteamID { get; set; }

    }
}
