using CoachBot.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoachBot.Domain.Model
{
    public class PlayerProfile
    {
        public string Name { get; set; }

        public Country Country { get; set; }

        public Team ClubTeam { get; set; }

        public Team NationalTeam { get; set; }

        public Position Position { get; set; }

    }
}
