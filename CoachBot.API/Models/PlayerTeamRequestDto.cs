using System;
using System.Collections.Generic;
using System.Text;

namespace CoachBot.Models
{
    public class PlayerTeamRequestDto
    {
        public int Id { get; set; }

        public bool IncludeInactive { get; set; } = false;
    }
}
