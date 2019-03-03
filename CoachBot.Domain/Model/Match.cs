using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoachBot.Model
{
    public class Match
    {
        /*[Key]
        public int Id { get; set; }*/

        public ulong ChannelId { get; set; }

        public string ChannelName { get; set; }

        public List<Player> Players { get; set; }

        public string Team1Name { get; set; }

        public string Team2Name { get; set; }

        public DateTime MatchDate { get; set; }

    }
}
