using System;
using System.Collections.Generic;
using System.Text;

namespace CoachBot.LegacyImporter.Model
{
    public class LegacyChannel
    {
        
        public ulong Id { get; set; }

        public string IdString { get { return Id.ToString(); } }

        public List<LegacyPosition> Positions { get; set; }

        public string Name { get; set; }

        public string GuildName { get; set; }

        public int RegionId { get; set; }

        public IEnumerable<KeyValuePair<string, string>> Emotes { get; set; }

        public LegacyTeam Team1 { get; set; }

        public LegacyTeam Team2 { get; set; }

        public LegacyFormation Formation { get; set; }

        public bool ClassicLineup { get; set; }

        public bool IsMixChannel { get; set; }

        public bool IsSearching { get; set; }

        public DateTime? LastSearch { get; set; }

        public bool DisableSearchNotifications { get; set; }

        public bool EnableUnsignWhenPlayerStartsOtherGame { get; set; }

        public LegacyRegion Region { get; set; }

    }
}
