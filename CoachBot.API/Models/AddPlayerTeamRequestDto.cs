using CoachBot.Domain.Model;

namespace CoachBot.Models
{
    public class AddPlayerTeamRequestDto
    {
        public int PlayerId { get; set; }

        public int TeamId { get; set; }

        public TeamRole TeamRole { get; set; }
    }
}