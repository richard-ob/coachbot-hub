using CoachBot.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachBot.Domain.Model
{
    public class PlayerStatisticTotals
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int PlayerId { get; set; }

        public Player Player { get; set; }

        public StatisticTotals StatisticTotals { get; set; } = new StatisticTotals();

    }
}
