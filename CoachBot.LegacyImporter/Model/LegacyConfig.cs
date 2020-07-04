using System.Collections.Generic;

namespace CoachBot.LegacyImporter.Model
{
    public class LegacyConfig
    {
        public string BotToken { get; set; }

        public string OAuth2Id { get; set; }

        public string OAuth2Secret { get; set; }

        public string ClientUrl { get; set; }

        public ulong OwnerGuildId { get; set; }

        public List<LegacyServer> Servers { get; set; }

        public List<LegacyChannel> Channels { get; set; }

        public List<LegacyRegion> Regions { get; set; }
    }
}