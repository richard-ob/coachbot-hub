using CoachBot.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CoachBot.Model
{
    public class Channel
    {
        [Key]
        public ulong Id { get; set; }

        // TODO: Create custom serializer to serialize ulong's as strings, as Javascript cannot handle large ulong values
        public string IdString { get { return Id.ToString(); } }

        public List<Position> Positions { get; set; }

        public string Name { get; set; }

        public string GuildName { get; set; }

        public int RegionId { get; set; }

        public IEnumerable<KeyValuePair<string, string>> Emotes { get; set; }

        public Team Team1 { get; set; }

        public Team Team2 { get; set; }

        public Formation Formation { get; set; }

        public bool ClassicLineup { get; set; }

        public bool IsMixChannel { get; set; }

        public bool IsSearching { get; set; }

        public DateTime? LastSearch { get; set; }

        public bool DisableSearchNotifications { get; set; }

        [JsonIgnore]
        public DateTime? LastHereMention { get; set; }

        [JsonIgnore]
        public IEnumerable<Player> SignedPlayers
        {
            get
            {
                if (Team1.Players.Any() && Team2.Players.Any())
                {
                    return Team1.Players.Concat(Team2.Players);
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
