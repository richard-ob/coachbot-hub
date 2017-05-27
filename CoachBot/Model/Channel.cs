using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace CoachBot.Model
{
    public class Channel
    {
        public ulong Id { get; set; }

        public List<string> Positions { get; set; }

        public Team Team1 { get; set; }

        public Team Team2 { get; set; }

        public bool UseFormation { get; set; }

        public bool ClassicLineup { get; set; }

        [JsonIgnore]
        public IEnumerable<Player> SignedPlayers
        {
            get
            {
                if (Team1.Players.Any() && Team2.Players.Any())
                {
                    return Team1.Players.Keys.Concat(Team2.Players.Keys);
                }
                else if (Team1.Players.Any())
                {
                    return Team1.Players.Keys;
                }
                else
                {
                    return Team2.Players.Keys;
                }
            }
        }
    }
}
