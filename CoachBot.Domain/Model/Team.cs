using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachBot.Model
{
    public class Team
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public ulong? ChannelId { get; set; }

        public string Name { get; set; }

        public TeamType TeamType { get; set; }

        public string KitEmote { get; set; }

        public string BadgeEmote { get; set; }

        public string Color { get; set; }

        public bool IsMix { get; set; }

        public List<Player> Substitutes { get; set; }

        public List<Player> Players { get; set; }

    }
}
