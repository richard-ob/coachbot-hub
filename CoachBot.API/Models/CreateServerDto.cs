using System;
using System.Collections.Generic;
using System.Text;

namespace CoachBot.Models
{
    public class CreateServerDto
    {
        public string Name { get; set; }

        public string Address { get; set; }

        public string RconPassword { get; set; }

        public int RegionId { get; set; }

        public int CountryId { get; set; }

    }
}
