using CoachBot.Model;

namespace CoachBot.Domain.Model
{
    public class PlayerTeamPosition
    {
        public int PlayerId { get; set; }

        public Player Player { get; set; }
        
        public int TeamId { get; set; }

        public Team Team { get; set; }

        public int PositionId { get; set; }

        public Position Position { get; set; }

    }
}
