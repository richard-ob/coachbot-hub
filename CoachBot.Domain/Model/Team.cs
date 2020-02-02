using CoachBot.Domain.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace CoachBot.Model
{
    public class Team
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? ChannelId { get; set; }

        public Channel Channel { get; set; }

        [InverseProperty("TeamHome")]
        public Match HomeMatch { get; set; }

        [InverseProperty("TeamAway")]
        public Match AwayMatch { get; set; }

        public Match Match => HomeMatch ?? AwayMatch;

        public TeamType TeamType { get; set; }

        public ICollection<PlayerTeamPosition> PlayerTeamPositions { get; set; }

        public ICollection<PlayerTeamSubstitute> PlayerSubstitutes { get; set; }

        [JsonIgnore]
        [NotMapped]
        public List<Position> OccupiedPositions
        {
            get
            {
                if (PlayerTeamPositions != null)
                {
                    return PlayerTeamPositions.Select(ptp => ptp.Position).ToList();
                }
                else
                {
                    return new List<Position>();
                }
            }
        }

    }
}
