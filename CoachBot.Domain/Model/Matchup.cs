using CoachBot.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace CoachBot.Domain.Model
{
    public class Matchup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? LineupHomeId { get; set; }

        public int? LineupAwayId { get; set; }

        [ForeignKey("LineupHomeId")]
        public Lineup LineupHome { get; set; }

        [ForeignKey("LineupAwayId")]
        public Lineup LineupAway { get; set; }

        public int? MatchId { get; set; }

        public Match Match { get; set; }

        public DateTime? ReadiedDate { get; set; }

        public bool IsMixMatch => LineupHome?.ChannelId == LineupAway?.ChannelId;

        public Lineup GetLineup(MatchTeamType teamType) => teamType == MatchTeamType.Home ? LineupHome : LineupAway;

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }

        [JsonIgnore]
        [NotMapped]
        public List<Player> SignedPlayers
        {
            get
            {
                var players = new List<Player>();

                if (LineupHome != null && LineupHome.PlayerLineupPositions != null)
                {
                    players.AddRange(LineupHome.PlayerLineupPositions.Select(ptp => ptp.Player));
                }

                if (LineupAway != null && LineupAway.PlayerLineupPositions != null)
                {
                    players.AddRange(LineupAway.PlayerLineupPositions.Select(ptp => ptp.Player));
                }

                return players;
            }
        }

        [JsonIgnore]
        [NotMapped]
        public List<Player> SignedSubstitutes
        {
            get
            {
                var players = new List<Player>();
                if (LineupHome != null && LineupHome.PlayerSubstitutes != null)
                {
                    players.AddRange(LineupHome.PlayerSubstitutes.Select(ps => ps.Player));
                }
                if (LineupAway != null && LineupAway.PlayerSubstitutes != null)
                {
                    players.AddRange(LineupAway.PlayerSubstitutes.Select(ps => ps.Player));
                }

                return players;
            }
        }

        [JsonIgnore]
        [NotMapped]
        public List<Player> SignedPlayersAndSubs
        {
            get
            {
                var players = new List<Player>();
                players.AddRange(SignedPlayers);
                players.AddRange(SignedSubstitutes);

                return players;
            }
        }
    }
}
