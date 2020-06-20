using System;
using System.Collections.Generic;
using System.Text;

namespace CoachBot.Domain.Model
{
    public class FantasyTeamRank
    {
        public int Rank { get; set; }

        public int FantasyTeamId { get; set; }

        public string FantasyTeamName { get; set; }

        public ulong SteamID { get; set; }

        public int PlayerId { get; set; }

        public int Points { get; set; }

    }
}
