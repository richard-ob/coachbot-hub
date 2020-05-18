using CoachBot.Domain.Model;
using CoachBot.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoachBot.Controllers
{
    public class FantasyPlayerTeamController
    {
        private readonly FantasyService _fantasyService;

        public FantasyPlayerTeamController(FantasyService fantasyService)
        {
            _fantasyService = fantasyService;
        }
    }
}
