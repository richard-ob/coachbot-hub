namespace CoachBot.Domain.Model.Dtos
{
    public class RegionDto
    {
        public int RegionId { get; set; }

        public string RegionName { get; set; }

        public string RegionCode { get; set; }

        public int ServerCount { get; set; }

        public int MatchCount { get; set; }

        public int TeamCount { get; set; }

        public MatchFormat MatchFormat { get; set; }
    }
}