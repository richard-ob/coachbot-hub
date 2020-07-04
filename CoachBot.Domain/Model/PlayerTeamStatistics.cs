using CoachBot.Model;

namespace CoachBot.Domain.Model
{
    public class PlayerTeamStatisticsTotals : StatisticTotals
    {
        public PlayerTeam PlayerTeam { get; set; }

        public Position Position { get; set; }
    }
}