namespace CoachBot.Domain.Model
{
    public class MatchStatistics
    {
        public int MatchId { get; set; }

        public Match Match { get; set; }

        public MatchData MatchData { get; set; }

    }
}
