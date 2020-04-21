using CoachBot.Model;
using System;
using System.ComponentModel.DataAnnotations;

namespace CoachBot.Domain.Model
{
    public class PlayerRating
    {
        [Key]
        public int Id { get; set; }

        public double Rating { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int PlayerId { get; set; }

        public Player Player { get; set; }

    }
}
