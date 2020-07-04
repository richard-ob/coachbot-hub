using CoachBot.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace CoachBot.Domain.Model
{
    public class Match
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

        public int? TeamHomeId { get; set; }

        [ForeignKey("TeamHomeId")]
        public Team TeamHome { get; set; }

        public int? TeamAwayId { get; set; }

        [ForeignKey("TeamAwayId")]
        public Team TeamAway { get; set; }

        public int? ServerId { get; set; }

        public Server Server { get; set; }

        public int? MatchStatisticsId { get; set; }

        [ForeignKey("MatchStatisticsId")]
        public MatchStatistics MatchStatistics { get; set; }

        public DateTime? ReadiedDate { get; set; }

        public DateTime? ScheduledKickOff { get; set; }

        public DateTime? KickOff { get; set; }

        public MatchFormat Format { get; set; } = MatchFormat.EightVsEight;

        public MatchType MatchType { get; set; } = MatchType.RankedFriendly;

        public string Map { get; set; }

        public MatchPeriod FinalMatchPeriod { get; set; }

        public int? TournamentId { get; set; }

        public Tournament Tournament { get; set; }

        public ICollection<PlayerMatchStatistics> PlayerMatchStatistics { get; set; }

        public ICollection<PlayerPositionMatchStatistics> PlayerPositionMatchStatistics { get; set; }

        public ICollection<TeamMatchStatistics> TeamMatchStatistics { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }

        public bool IsMixMatch => LineupHome?.ChannelId == LineupAway?.ChannelId;

        public Lineup GetTeam(MatchTeamType teamType) => teamType == MatchTeamType.Home ? LineupHome : LineupAway;

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