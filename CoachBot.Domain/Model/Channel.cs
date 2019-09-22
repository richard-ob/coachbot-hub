using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace CoachBot.Model
{
    public class Channel
    {
        [Key]
        public ulong Id { get; set; }

        public List<Position> Positions { get; set; }

        public string Name { get; set; }

        public string GuildName { get; set; }

        public int? TeamId1 { get; set; }

        public int? TeamId2 { get; set; }

        [ForeignKey("TeamId1")]
        public Team Team1 { get; set; }

        [ForeignKey("TeamId2")]
        public Team Team2 { get; set; }

        public Formation Formation { get; set; }

        public bool ClassicLineup { get; set; }

        public bool IsMixChannel { get; set; }

        public bool IsSearching { get; set; }

        public DateTime? LastSearch { get; set; }

        public bool DisableSearchNotifications { get; set; }

        public int RegionId { get; set; }

        public Region Region { get; set; }

        // TODO: Create custom serializer to serialize ulong's as strings, as Javascript cannot handle large ulong values
        [NotMapped]
        public string IdString { get { return Id.ToString(); } }

        [NotMapped]
        public IEnumerable<KeyValuePair<string, string>> Emotes { get; set; }

        [JsonIgnore]
        [NotMapped]
        public DateTime? LastHereMention { get; set; }

        [JsonIgnore]
        public List<Player> SignedPlayers
        {
            get
            {
                if (Team1.Players.Any() && Team2.Players.Any())
                {
                    return Team1.Players.Concat(Team2.Players).ToList();
                }
                else if (Team1.Players.Any())
                {
                    return Team1.Players;
                }
                else
                {
                    return Team2.Players;
                }
            }
        }
    }
}
