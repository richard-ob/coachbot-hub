using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CoachBot.Domain.Model
{
    public class Map
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

    }
}
