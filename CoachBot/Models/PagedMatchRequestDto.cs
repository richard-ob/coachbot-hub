using CoachBot.Domain.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoachBot.Models
{
    public class PagedMatchRequestDto: PagedRequest
    {
        public int RegionId { get; set; }
    }
}
