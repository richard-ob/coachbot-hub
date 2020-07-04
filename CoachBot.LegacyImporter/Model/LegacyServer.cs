namespace CoachBot.LegacyImporter.Model
{
    public class LegacyServer
    {
        public string Name { get; set; }

        public string Address { get; set; }

        public int RegionId { get; set; }

        public LegacyRegion Region { get; set; }

        public string RconPassword { get; set; }
    }
}