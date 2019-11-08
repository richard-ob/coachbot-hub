using CoachBot.Model;

namespace CoachBot.Domain.Model
{
    public class PlayerTeamSubstitute
    {
        public int PlayerId { get; set; }

        public Player Player { get; set; }

        public int TeamId { get; set; }

        public Team Team { get; set; }
    }
}
