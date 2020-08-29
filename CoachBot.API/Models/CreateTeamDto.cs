using CoachBot.Domain.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoachBot.Models
{
    public class CreateTeamDto: Team
    {
        public string Token { get; set; }

    }
}
