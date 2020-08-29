using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CoachBot.Models
{
    public class AnnouncementDto
    {
        [MinLength(5)]
        public string Title { get; set; }

        [MinLength(15)]
        public string Message { get; set; }

        public int? RegionId { get; set; }
    }
}
