using CoachBot.Domain.Model;
using System;
using System.Collections.Generic;

namespace CoachBot.Models
{
    public class PlayerProfileUpdateDto
    {
        public string Name { get; set; }

        public bool DisableDMNotifications { get; set; }

        public DateTime? PlayingSince { get; set; }

        public int? CountryId { get; set; }

        public IEnumerable<PlayerPosition> Positions { get; set; }
    }
}
