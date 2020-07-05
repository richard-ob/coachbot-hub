using CoachBot.Domain.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace CoachBot.Model
{
    public class Lineup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? ChannelId { get; set; }

        public Channel Channel { get; set; }

        [InverseProperty("LineupHome")]
        public Matchup HomeMatchup { get; set; }

        [InverseProperty("LineupAway")]
        public Matchup AwayMatchup { get; set; }

        public Matchup Matchup => HomeMatchup ?? AwayMatchup;

        public MatchTeamType TeamType { get; set; }

        public ICollection<PlayerLineupPosition> PlayerLineupPositions { get; set; }

        public ICollection<PlayerLineupSubstitute> PlayerSubstitutes { get; set; }

        [JsonIgnore]
        [NotMapped]
        public List<Position> OccupiedPositions
        {
            get
            {
                if (PlayerLineupPositions != null)
                {
                    return PlayerLineupPositions.Select(ptp => ptp.Position).ToList();
                }
                else
                {
                    return new List<Position>();
                }
            }
        }

        [JsonIgnore]
        public bool HasGk => !Channel.ChannelPositions.Any(cp => cp.Position.Name.ToUpper() == "GK") || OccupiedPositions.Any(p => p.Name.ToUpper() == "GK");

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }
    }
}