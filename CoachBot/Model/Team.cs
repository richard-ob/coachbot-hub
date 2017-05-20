using System.Collections.Generic;

namespace CoachBot.Model
{
    public class Team
    {
        public string Name { get; set; }

        public bool IsMix { get; set; }

        public Dictionary<Player, string> Players { get; set; }

    }
}
