using CoachBot.Domain.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachBot.Model
{
    public class Position
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<ChannelPosition> ChannelPositions { get; set; }

        public ICollection<PlayerTeamPosition> PlayerTeamPositions { get; set; }

    }
}
