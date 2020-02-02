using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachBot.Domain.Model
{
    public class TeamStatisticTotals
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ChannelId { get; set; }

        public Channel Channel { get; set; }

        public StatisticTotals StatisticTotals { get; set; } = new StatisticTotals();

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }

    }
}
