using System;
using System.ComponentModel.DataAnnotations;

namespace CoachBot.Model
{
    public class Position
    {
        public Position() {}

        public Position(string position)
        {
            PositionName = position;
        }

        [Key]
        public Guid Id { get; set; }
        public string PositionName { get; set; }
    }
}
