using CoachBot.Model;

namespace CoachBot.Domain.Model
{
    public class PlayerProfile
    {
        public string Name { get; set; }

        public Country Country { get; set; }

        public double Rating { get; set; }

        public Team ClubTeam { get; set; }

        public TeamRole ClubTeamRole { get; set; }

        public Team NationalTeam { get; set; }

        public Position Position { get; set; }
    }
}