namespace CoachBot.Domain.Model
{
    public class ScorePredictionLeaderboardPlayer
    {
        public int PlayerId { get; set; }

        public string PlayerName { get; set; }

        public int Points { get; set; }

        public int Predictions { get; set; }
    }
}