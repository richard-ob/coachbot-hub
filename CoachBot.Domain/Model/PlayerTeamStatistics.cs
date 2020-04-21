using CoachBot.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoachBot.Domain.Model
{
    public class PlayerTeamStatisticsTotals : StatisticTotals
    {
        public PlayerTeam PlayerTeam { get; set; }

        public Position Position { get; set; }

    }
}
