namespace CoachBot.Shared.Model
{
    public class Config
    {
        public string BotToken { get; set; }

        public string OAuth2Id { get; set; }

        public string OAuth2Secret { get; set; }

        public string SteamApiToken { get; set; }

        public string ClientUrl { get; set; }

        public string HubApiUrl { get; set; }

        public ulong OwnerGuildId { get; set; }

        public ulong AuditChannelId { get; set; } = 642879369366339635;

        public string SqlConnectionString { get; set; }

        public int ApiPort { get; set; } = 80;

        public int BotApiPort { get; set; } = 8080;

        public AzureAssetsConfig AzureAssetsConfig { get; set; }

        public bool BotStealthMode { get; set; } = false;

        public bool EnableBotHubIntegration { get; set; } = false;
    }

    public class AzureAssetsConfig
    {
        public string AccountName { get; set; }

        public string Key { get; set; }

        public string ContainerName { get; set; }
    }
}