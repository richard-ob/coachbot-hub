using CoachBot.Domain.Model;

namespace CoachBot.Models.Dto
{
    public class MatchStatisticsDto
    {
        public MatchData MatchData { get; set; }
        public string Access_Token { get; set; }
    }
}