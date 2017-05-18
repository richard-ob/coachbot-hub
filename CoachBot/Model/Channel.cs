using System.Collections.Generic;

namespace CoachBot.Model
{
    public class Channel
    {
        public ulong Id { get; set; }

        public List<string> Positions { get; set; }

        public Team Team1 { get; set; }

        public Team Team2 { get; set; }

    }
}
