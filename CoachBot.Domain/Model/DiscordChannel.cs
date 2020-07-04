namespace CoachBot.Domain.Model
{
    public class DiscordChannel
    {
        public ulong Id { get; set; }

        public string Name { get; set; }

        public bool IsConfigured { get; set; }
    }
}