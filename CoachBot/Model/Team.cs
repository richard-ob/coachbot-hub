using Discord;
using System.Collections.Generic;

namespace CoachBot.Model
{
    public class Team
    {
        public string Name { get; set; }

        public bool IsMix { get; set; }

        public Dictionary<IUser, string> Players { get; set; }

    }
}
